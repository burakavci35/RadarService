using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using RadarService.Data.Repositories;
using RadarService.Data.UnitOfWork;
using RadarService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Scheduler
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IConfiguration _configuration;
        private IUnitOfwork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;
        private static string MockDeviceResult = "Passive";

        public DeviceWorker(ILogger<DeviceWorker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _unitOfWork = new UnitOfWork(_serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>());
                var activeDevices = _unitOfWork.Device.GetAll().Where(x => x.IsActive).Include(x => x.DeviceSchedulers).ThenInclude(x => x.Scheduler).ToList();
                foreach (var device in activeDevices)
                {
                    await ExecuteDevice(device);

                }
                _logger.LogInformation("Worker Completed at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromSeconds(Convert.ToInt32(_configuration.GetSection("IntervalSeconds").Value)), stoppingToken);
            }
        }
        private async Task ExecuteDevice(Device device)
        {
            try
            {
                _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Started!");
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(clientHandler) { BaseAddress = new Uri(device.BaseAddress) };
                var resultStatus = await CheckDeviceStatus(client, device, "CheckDeviceStatus");
                using (var unitOfWork = new UnitOfWork(_serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>()))
                {
                    var foundDevice = await _unitOfWork.Device.GetByIdAsync(device.Id);
                    foundDevice.Status = resultStatus;
                    unitOfWork.Device.Update(foundDevice);
                    await unitOfWork.SaveChangesAsync();
                }

                if (device.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
                       && resultStatus == "Passive")
                {
                    await SendDeviceRequest(client, device, "OpenDevice");
                     _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Activated!");

                }
                if (!device.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
                  && device.Status == "Active")
                {
                    await SendDeviceRequest(client, device, "CloseDevice");
                     _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Deactivated!");
                }

                 _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Completed!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Method : {nameof(ExecuteDevice)} Device Name {device.Name} Error : {ex.Message}");

            }


        }

        private async Task<string> CheckDeviceStatus(HttpClient client, Device device, string checkDeviceStatusCommand)
        {
            var foundCommand = await _unitOfWork.DeviceCommand.GetAll()
                .Include(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Request).ThenInclude(x => x.FormParameters)
                .Include(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Request).ThenInclude(x => x.ResponseConditions)
                .Include(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Step)
                .Where(x => x.DeviceId == device.Id)
                .FirstOrDefaultAsync(x => x.Command.Name.Equals(checkDeviceStatusCommand));

            if (foundCommand == null)
            {
                _logger.LogError($"Method : {nameof(CheckDeviceStatus)} Error : Command Name {checkDeviceStatusCommand} Not Found Error!");
                return null;
            }

            var result = string.Empty;

            foreach (var stepRequest in foundCommand.Command.StepRequests)
            {
                if (stepRequest.Step.Name.Equals("CheckStatus"))
                {
                    var expectedResponse = await ExecuteStep(client, stepRequest.Request);

                    var foundResponseCondition = stepRequest.Request.ResponseConditions.FirstOrDefault(x => expectedResponse.Contains(x.Condition));

                    result = foundResponseCondition?.Result;
                }
                else
                {
                    await ExecuteStep(client, stepRequest.Request);
                }
            }

            return result;
        }


        private async Task SendDeviceRequest(HttpClient client, Device device, string commandName)
        {
            var foundCommand = await _unitOfWork.DeviceCommand.GetAll().Include(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Request).Include(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Step)
               .Where(x => x.DeviceId == device.Id)
               .FirstOrDefaultAsync(x => x.Command.Name.Equals(commandName));

            if (foundCommand == null)
            {
                _logger.LogError($"Method : {nameof(SendDeviceRequest)} Error : Command Name {commandName} Not Found Error!");
                return;
            }

            foreach (var stepRequest in foundCommand.Command.StepRequests)
            {
                await ExecuteStep(client, stepRequest.Request);
            }


        }

        //private async Task<string> ExecuteStep(HttpClient client, Request request)
        //{

        //    try
        //    {
        //        if (request.Type == "POST")
        //        {
        //            var formData = request.FormParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
        //            var result = await client.PostAsync(request.Url, new FormUrlEncodedContent(formData));
        //            _logger.LogInformation($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} FormData : {formData}");
        //            if (!result.IsSuccessStatusCode)
        //            {
        //                _logger.LogError($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");

        //            }
        //            var response = await result.Content.ReadAsStringAsync();
        //            _logger.LogInformation($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} ResponseData : {response}");
        //            return response;

        //        }
        //        else
        //        {

        //            var result = await client.GetAsync(request.Url);
        //            _logger.LogInformation($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");
        //            if (!result.IsSuccessStatusCode)
        //            {
        //                _logger.LogError($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");

        //            }
        //            var response = await result.Content.ReadAsStringAsync();
        //            _logger.LogInformation($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} ResponseData : {response}");
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"MethodName : {nameof(ExecuteStep)} Request : {request.Url} Type : {request.Type} Error : {ex.Message}");

        //        throw;
        //    }

        //}

        private async Task<string> ExecuteStep(HttpClient client, Request request)
        {

            try
            {
                if (request.Type == "POST")
                {

                    return MockDeviceResult;
                }
                else
                {
                    if (request.Url.Equals("/system/enforcement/1"))
                        MockDeviceResult = "Active";
                    if (request.Url.Equals("/system/enforcement/0"))
                        MockDeviceResult = "Passive";

                    return MockDeviceResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"MethodName : {nameof(ExecuteStep)} Request : {request.Url} Type : {request.Type} Error : {ex.Message}");

                throw;
            }

        }
    }
}