using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using RadarService.Entities.Models;
using RadarService.Data.Repositories;

namespace RadarService.WebApp.Jobs
{
    public class CheckStatusJob : IJob
    {
        private IRepository<Device> _deviceRepository;
        private readonly ILogger<CheckStatusJob> _logger;
        public CheckStatusJob(IRepository<Device> deviceRepository, ILogger<CheckStatusJob> logger)
        {
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var activeDevices = await _deviceRepository.GetAll().Where(x => x.IsActive)
                .Include(x => x.DeviceCommands).ThenInclude(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Request).ThenInclude(x => x.FormParameters)
                .Include(x => x.DeviceCommands).ThenInclude(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Request).ThenInclude(x => x.ResponseConditions)
                .Include(x => x.DeviceCommands).ThenInclude(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Step)
                .Include(x => x.DeviceSchedulers).ThenInclude(x => x.Scheduler).AsNoTracking().ToListAsync();

            foreach (var device in activeDevices)
            {
                await ExecuteDevice(device);

            }

            _deviceRepository.UpdateRange(activeDevices);
            await _deviceRepository.SaveChanges();

            //await Task.FromResult(true);
            await Task.CompletedTask;
        }

        private async Task ExecuteDevice(Device device)
        {

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler) { BaseAddress = new Uri(device.BaseAddress) };

            var foundResponseCondition = await CheckDeviceStatus(client, device);

            if (foundResponseCondition == null) return;



            if (device.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
                && foundResponseCondition.Result == "Passive" && device.Status != foundResponseCondition.Result)
            {
                var executeCommand = device.DeviceCommands.FirstOrDefault(x => x.Command.Name.Equals(foundResponseCondition.CommandName));

                if (executeCommand == null)
                {
                    _logger.LogError($"Device : {device.Name} executeCommand Found Error!");
                    return;
                }

                var executestepRequestList = executeCommand.Command.StepRequests.ToList();

                await ExecuteSteps(client, executestepRequestList);
            }

            if (!device.DeviceSchedulers.Any(x => x.Scheduler.StartTime.Ticks <= DateTime.Now.TimeOfDay.Ticks && DateTime.Now.TimeOfDay.Ticks <= x.Scheduler.EndTime.Ticks)
               && foundResponseCondition.Result == "Active" && device.Status != foundResponseCondition.Result)
            {
                var executeCommand = device.DeviceCommands.FirstOrDefault(x => x.Command.Name.Equals(foundResponseCondition.CommandName));

                if (executeCommand == null)
                {
                    _logger.LogError($"Device : {device.Name} executeCommand Found Error!");
                    return;
                }

                var executestepRequestList = executeCommand.Command.StepRequests.ToList();

                await ExecuteSteps(client, executestepRequestList);
            }

            device.Status = foundResponseCondition.Result;



        }

        private async Task<ResponseCondition?> CheckDeviceStatus(HttpClient client, Device device)
        {
            var checkStatusCommand = device.DeviceCommands.FirstOrDefault(x => x.Command.Name == "CheckDeviceStatus");
            if (checkStatusCommand == null)
            {
                _logger.LogError($"Device : {device.Name} CheckDeviceStatus Command Not Found Error!");
                return null;
            }
            var stepRequestList = checkStatusCommand.Command.StepRequests.ToList();

            await ExecuteSteps(client, stepRequestList);

            var foundStep = stepRequestList.FirstOrDefault(x => x.Step.Name == "CheckStatus");
            if (foundStep == null)
            {
                _logger.LogError($"Device : {device.Name} CheckStatus Step Not Found Error!");
                return null;
            }

            var foundResponseCondition = foundStep.Request.ResponseConditions.FirstOrDefault(x => foundStep.Request.Response.Contains(x.Condition));

            if (foundResponseCondition == null)
            {
                _logger.LogError($"Device : {device.Name} foundResponseCondition Found Error!");
                return null;
            }

            return foundResponseCondition;
        }


        private async Task ExecuteSteps(HttpClient client, List<StepRequest> stepRequests)

        {

            foreach (var stepRequest in stepRequests)
            {
                try
                {
                    if (stepRequest.Request.Type == "POST")
                    {
                        //var formData = stepRequest.Request.FormParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
                        //var result = await client.PostAsync(stepRequest.Request.Url, new FormUrlEncodedContent(formData));
                        //if (!result.IsSuccessStatusCode)
                        //{
                        //    _logger.LogError($"Step : {stepRequest.Step.Name} Request : {stepRequest.Request.Url} Type : {stepRequest.Request.Type} StatusCode : {result.StatusCode}");
                        //    continue;
                        //}
                        //stepRequest.Request.Response = await result.Content.ReadAsStringAsync();

                        stepRequest.Request.Response = "result : true";

                    }
                    if (stepRequest.Request.Type == "GET")
                    {

                        //var result = await client.GetAsync(stepRequest.Request.Url);
                        //if (!result.IsSuccessStatusCode)
                        //{
                        //    _logger.LogError($"Step : {stepRequest.Step.Name} Request : {stepRequest.Request.Url} Type : {stepRequest.Request.Type} StatusCode : {result.StatusCode}");
                        //    continue;
                        //}
                        //stepRequest.Request.Response = await result.Content.ReadAsStringAsync();
                        stepRequest.Request.Response = "Active";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Step : {stepRequest.Step.Name} Request : {stepRequest.Request.Url} Type : {stepRequest.Request.Type} Error : {ex.Message}");
                }
            }

        }



    }
}
