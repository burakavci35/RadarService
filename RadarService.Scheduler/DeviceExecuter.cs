using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Scheduler
{
    public class DeviceExecuter
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DeviceExecuter(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(Device device)
        {
            try
            {
                _logger.LogInformation($"MethodName : {nameof(Execute)} Device : {device.Name} Started!");
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(clientHandler) { BaseAddress = new Uri(device.BaseAddress) };

                var deviceStatus = await CheckDeviceStatus(client, device);
                if (deviceStatus == null) return;
                var context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();
                using (var deviceRepositoy = new Repository<Device>(context))
                {
                    var foundDevice = await deviceRepositoy.GetByIdAsync(device.Id);
                    foundDevice.Status = deviceStatus;
                    deviceRepositoy.Update(foundDevice);
                    await deviceRepositoy.SaveChanges();
                    await SendDeviceRequest(client, foundDevice);
                }




                _logger.LogInformation($"MethodName : {nameof(Execute)} Device : {device.Name} completed!");

            }
            catch (Exception ex)
            {
                _logger.LogError($"MethodName : {nameof(Execute)} Device : {device.Name} Error :{ex.Message}!");
            }


        }

        private async Task<string?> CheckDeviceStatus(HttpClient client, Device device)
        {
            _logger.LogInformation($"MethodName : {nameof(CheckDeviceStatus)} Device : {device.Name} Started!");

            var checkStatusCommand = device.DeviceCommands.FirstOrDefault(x => x.Command.Name == "CheckDeviceStatus");

            if (checkStatusCommand == null)
            {
                _logger.LogError($"Method : {nameof(CheckDeviceStatus)} Device : {device.Name} CheckDeviceStatus Command Not Found Error!");
                return null;
            }
             var context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();
            using (var requestRepository = new Repository<Request>(context))
            {
                var requestList = await requestRepository.GetAll().Include(x => x.ResponseConditions).Where(x => checkStatusCommand.Command.StepRequests.Select(y => y.RequestId).Contains(x.Id)).ToListAsync();

                foreach (var request in requestList)
                {
                    request.Response = await ExecuteStep(client, request);
                }
                requestRepository.UpdateRange(requestList);
                await requestRepository.SaveChanges();

                var foundStep = checkStatusCommand.Command.StepRequests.FirstOrDefault(x => x.Step.Name == "CheckStatus");
                if (foundStep == null)
                {
                    _logger.LogError($"Method : {nameof(CheckDeviceStatus)} Device : {device.Name} CheckStatus Step Not Found Error!");
                    return null;
                }

                var foundRequest = requestList.FirstOrDefault(x => x.Id == foundStep.RequestId);


                var foundResponseCondition = foundRequest.ResponseConditions.FirstOrDefault(x => foundRequest.Response.Contains(x.Condition));

                //var foundResponseCondition = foundStep.Request.ResponseConditions.FirstOrDefault(x => foundStep.Request.Response.Contains(x.Condition));

                if (foundResponseCondition == null)
                {
                    _logger.LogError($"Method : {nameof(CheckDeviceStatus)} Device : {device.Name} foundResponseCondition Found Error!");
                    return null;
                }
                _logger.LogInformation($"MethodName : {nameof(CheckDeviceStatus)} Device : {device.Name} completed!");
                return foundResponseCondition.Result;
            }




        }
        private async Task SendDeviceRequest(HttpClient client, Device device)
        {
            _logger.LogInformation($"MethodName : {nameof(SendDeviceRequest)} Device : {device.Name} Started!");
            var context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();
            using (var deviceRepositoy = new Repository<Device>(context))
            {
                var foundDevice = await deviceRepositoy.GetAll().Include(x => x.DeviceSchedulers).ThenInclude(x => x.Scheduler)
                                                                    .Include(x => x.DeviceCommands).ThenInclude(x => x.Command).ThenInclude(x => x.StepRequests).FirstOrDefaultAsync(x => x.Id == device.Id);


                if (foundDevice.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
                       && foundDevice.Status == "Passive")
                {
                    var executeCommand = foundDevice.DeviceCommands.FirstOrDefault(x => x.Command.Name.Equals("OpenDevice"));

                    if (executeCommand == null)
                    {
                        _logger.LogError($"Device : {device.Name} executeCommand Found Error!");
                        return;
                    }

                    context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();
                    using (var requestRepository = new Repository<Request>(context))
                    {
                        var requestList = await requestRepository.GetAll().Include(x => x.ResponseConditions).Where(x => executeCommand.Command.StepRequests.Select(y => y.RequestId).Contains(x.Id)).ToListAsync();

                        foreach (var request in requestList)
                        {
                            request.Response = await ExecuteStep(client, request);
                            requestRepository.UpdateRange(requestList);
                            await requestRepository.SaveChanges();
                        }
                    }

                     _logger.LogInformation($"MethodName : {nameof(SendDeviceRequest)} Device : {device.Name} Activated!");

                }

                if (!foundDevice.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
                   && foundDevice.Status == "Active")
                {
                    var executeCommand = foundDevice.DeviceCommands.FirstOrDefault(x => x.Command.Name.Equals("CloseDevice"));

                    if (executeCommand == null)
                    {
                        _logger.LogError($"Device : {device.Name} executeCommand Found Error!");
                        return;
                    }
                    context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();
                    using (var requestRepository = new Repository<Request>(context))
                    {
                        var requestList = await requestRepository.GetAll().Include(x => x.ResponseConditions).Where(x => executeCommand.Command.StepRequests.Select(y => y.RequestId).Contains(x.Id)).ToListAsync();

                        foreach (var request in requestList)
                        {
                            request.Response = await ExecuteStep(client, request);
                            requestRepository.UpdateRange(requestList);
                            await requestRepository.SaveChanges();
                        }
                    }
                     _logger.LogInformation($"MethodName : {nameof(SendDeviceRequest)} Device : {device.Name} Deactivated!");
                }

            }
            _logger.LogInformation($"MethodName : {nameof(SendDeviceRequest)} Device : {device.Name} completed!");
        }
        private async Task<string> ExecuteStep(HttpClient client, Request request)

        {

            try
            {
                if (request.Type == "POST")
                {
                    //var formData = stepRequest.Request.FormParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
                    //var result = await client.PostAsync(stepRequest.Request.Url, new FormUrlEncodedContent(formData));
                    //if (!result.IsSuccessStatusCode)
                    //{
                    //    _logger.LogError($"Step : {stepRequest.Step.Name} Request : {stepRequest.Request.Url} Type : {stepRequest.Request.Type} StatusCode : {result.StatusCode}");
                    //    continue;
                    //}
                    //stepRequest.Request.Response = await result.Content.ReadAsStringAsync();

                    return "result : true";

                }
                else
                {

                    //var result = await client.GetAsync(stepRequest.Request.Url);
                    //if (!result.IsSuccessStatusCode)
                    //{
                    //    _logger.LogError($"Step : {stepRequest.Step.Name} Request : {stepRequest.Request.Url} Type : {stepRequest.Request.Type} StatusCode : {result.StatusCode}");
                    //    continue;
                    //}
                    //stepRequest.Request.Response = await result.Content.ReadAsStringAsync();
                    return new Random().Next(2) == 1 ? "Active" : "Passive";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Request : {request.Url} Type : {request.Type} Error : {ex.Message}");

                return ex.Message;
            }

        }
    }
}
