using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadarService.Data.UnitOfWork;
using RadarService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Scheduler.Bussiness
{
    public class DeviceScanner
    {

        private IUnitOfwork _unitOfWork;
        private int _deviceId;
        private readonly ILogger<DeviceWorker> _logger;
        private static Dictionary<int, string> MockDevices = new Dictionary<int, string>() { { 1, "Passive" }, { 2, "Passive" } };
        public DeviceScanner(IUnitOfwork unitOfwork, int deviceId, ILogger<DeviceWorker> logger)
        {

            _unitOfWork = unitOfwork;
            _deviceId = deviceId;
            _logger = logger;

        }

        public async Task ExecuteDevice()
        {
            try
            {
                var device = await _unitOfWork.Device.GetAll().Include(x => x.DeviceSchedulers).ThenInclude(x => x.Scheduler).FirstOrDefaultAsync(x => x.Id == _deviceId);


                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(clientHandler) { BaseAddress = new Uri(device.BaseAddress) };

                try
                {

                    if (!await LoginDevice(client, device, "Login"))
                    {
                        _logger.LogError($"Method : {nameof(ExecuteDevice)} Device Name {device.Name} Login Error!");
                        await LogoutDevice(client, device, "Logout");
                        return;
                    }

                    var resultStatus = await CheckDeviceStatus(client, device, "CheckDeviceStatus");

                    var schedulers = new List<Entities.Models.Scheduler>();

                    foreach (var deviceScheduler in device.DeviceSchedulers)
                    {
                        if (deviceScheduler.Scheduler.EndTime.Days == 1)
                        {
                            schedulers.Add(new Entities.Models.Scheduler() { StartTime = deviceScheduler.Scheduler.StartTime, EndTime = new TimeSpan(23, 59, 59) });
                            schedulers.Add(new Entities.Models.Scheduler() { StartTime = new TimeSpan(0, 0, 0), EndTime = deviceScheduler.Scheduler.EndTime.Subtract(new TimeSpan(1, 0, 0, 0)) });
                        }
                        else
                        {
                            schedulers.Add(deviceScheduler.Scheduler);
                        }
                    }

                    var currentTimeSpan = DateTime.Now.TimeOfDay;


                    if (schedulers.Any(x => x.StartTime.Ticks <= currentTimeSpan.Ticks && currentTimeSpan.Ticks <= x.EndTime.Ticks)
                           && resultStatus.Equals("Passive"))
                    {
                        await SendDeviceRequest(client, device, "Open");
                        _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Activated!");

                        await _unitOfWork.DeviceLog.AddAsync(new DeviceLog()
                        {
                            DeviceId = device.Id,
                            LogDateTime = DateTime.Now,
                            Type = "Information",
                            Message = $"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Activated!"
                        });


                    }
                    if (!schedulers.Any(x => x.StartTime.Ticks <= currentTimeSpan.Ticks && currentTimeSpan.Ticks <= x.EndTime.Ticks)
                      && resultStatus.Equals("Active"))
                    {
                        await SendDeviceRequest(client, device, "Close");
                        _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Deactivated!");


                        await _unitOfWork.DeviceLog.AddAsync(new DeviceLog()
                        {
                            DeviceId = device.Id,
                            LogDateTime = DateTime.Now,
                            Type = "Information",
                            Message = $"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Deactivated!"
                        });

                    }

                    if (resultStatus.Equals("Configuration"))
                    {
                        if (!await ConfigureDevice(client, device, "Configuration"))
                        {
                            _logger.LogError($"Method : {nameof(ExecuteDevice)} Device Name {device.Name} Configuration Error!");
                            await LogoutDevice(client, device, "Logout");
                            return;
                        }
                        _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} ReConfigurated!");


                        await _unitOfWork.DeviceLog.AddAsync(new DeviceLog()
                        {
                            DeviceId = device.Id,
                            LogDateTime = DateTime.Now,
                            Type = "Information",
                            Message = $"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} ReConfigurated!"
                        });


                    }

                    var foundDevice = await _unitOfWork.Device.GetByIdAsync(device.Id);
                    foundDevice.Status = resultStatus ?? "Unknown";
                    foundDevice.LastUpdateDateTime = DateTime.Now;
                    _unitOfWork.Device.Update(foundDevice);
                    await _unitOfWork.SaveChangesAsync();



                    await LogoutDevice(client, device, "Logout");

                    _logger.LogInformation($"MethodName : {nameof(ExecuteDevice)} Device : {device.Name} Completed!");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Method : {nameof(ExecuteDevice)} Device Name {device.Name} Error : {ex.Message}");

                }
                finally
                {
                    await LogoutDevice(client, device, "Logout");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Method : {nameof(ExecuteDevice)} Device Name {_deviceId} Error : {ex.Message}");
            }

        }

        private async Task<bool> LoginDevice(HttpClient client, Device device, string loginRequestName)
        {
            var foundDeviceRequest = await _unitOfWork.DeviceRequest.GetAll()
                .Include(x => x.Device).Include(x => x.Request).ThenInclude(x => x.FormParameters)
               .Where(x => x.DeviceId == device.Id)
               .FirstOrDefaultAsync(x => x.Request.Name.Equals(loginRequestName));

            if (foundDeviceRequest == null) throw new Exception($"Method : {nameof(LoginDevice)} Error : Name {loginRequestName} Not Found Error!");

            var expectedResult = await ExecuteStep(client, foundDeviceRequest.Request);

            if (expectedResult == null) throw new Exception($"Method : {nameof(LoginDevice)} Error :  Name {loginRequestName} Not Found Error!");

            var isMatch = expectedResult.Contains(foundDeviceRequest.Request.Response);

            if (!isMatch)
            {
                _logger.LogError($"MethodName : {nameof(LoginDevice)} Device : {device.Name} : {expectedResult} != {foundDeviceRequest.Request.Response}");
            }

            return isMatch;
        }

        private async Task<bool> ConfigureDevice(HttpClient client, Device device, string configurationName)
        {
            var foundDeviceRequest = await _unitOfWork.DeviceRequest.GetAll()
                .Include(x => x.Device).Include(x => x.Request).ThenInclude(x => x.FormParameters)
               .Where(x => x.DeviceId == device.Id)
               .FirstOrDefaultAsync(x => x.Request.Name.Equals(configurationName));

            if (foundDeviceRequest == null) throw new Exception($"Method : {nameof(ConfigureDevice)} Error : Name {configurationName} Not Found Error!");

            var expectedResult = await ExecuteStep(client, foundDeviceRequest.Request);

            if (expectedResult == null) throw new Exception($"Method : {nameof(ConfigureDevice)} Error :  Name {configurationName} Not Found Error!");

            return expectedResult.Contains(foundDeviceRequest.Request.Response);
        }


        private async Task LogoutDevice(HttpClient client, Device device, string logoutRequestName)
        {
            var foundDeviceRequest = await _unitOfWork.DeviceRequest.GetAll()
                .Include(x => x.Device).Include(x => x.Request).ThenInclude(x => x.FormParameters)
               .Where(x => x.DeviceId == device.Id)
               .FirstOrDefaultAsync(x => x.Request.Name.Equals(logoutRequestName));

            if (foundDeviceRequest == null) throw new Exception($"Method : {nameof(LogoutDevice)} Error : Name {logoutRequestName} Not Found Error!");

            await ExecuteStep(client, foundDeviceRequest.Request);
        }


        private async Task<string> CheckDeviceStatus(HttpClient client, Device device, string checkDeviceStatusName)
        {
            var foundDeviceRequest = await _unitOfWork.DeviceRequest.GetAll()
                .Include(x => x.Request).ThenInclude(x => x.ResponseConditions)
                .Where(x => x.DeviceId == device.Id)
                .FirstOrDefaultAsync(x => x.Request.Name.Equals(checkDeviceStatusName));

            if (foundDeviceRequest == null)
            {
                throw new Exception($"Method : {nameof(CheckDeviceStatus)} Error : Name {checkDeviceStatusName} Not Found Error!");
            }

            var expectedResponse = await ExecuteStep(client, foundDeviceRequest.Request);

            var foundResponseCondition = foundDeviceRequest.Request.ResponseConditions.FirstOrDefault(x => expectedResponse.Contains(x.Condition));

            if (foundResponseCondition == null) return "Unknown";

            return foundResponseCondition.Result;
        }


        private async Task SendDeviceRequest(HttpClient client, Device device, string requestName)
        {
            var foundDeviceRequest = await _unitOfWork.DeviceRequest.GetAll()
                 .Include(x => x.Request).ThenInclude(x => x.ResponseConditions)
                 .Where(x => x.DeviceId == device.Id)
                 .FirstOrDefaultAsync(x => x.Request.Name.Equals(requestName));

            if (foundDeviceRequest == null)
            {
                throw new Exception($"Method : {nameof(SendDeviceRequest)} Error : Name {requestName} Not Found Error!");
            }

            await ExecuteStep(client, foundDeviceRequest.Request);

        }

        private async Task<string> ExecuteStep(HttpClient client, Request request)
        {

            try
            {
                if (request.Type == "POST")
                {
                    var formData = request.FormParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));

                    var result = await client.PostAsync(request.Url, new FormUrlEncodedContent(formData));

                    _logger.LogInformation($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} FormData : {formData}");

                    if (!result.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");

                    }

                    var response = await result.Content.ReadAsStringAsync();

                    _logger.LogInformation($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} ResponseData : {response}");

                    if (request.InverseParent.Any())
                        return await ExecuteStep(client, request.InverseParent.First());
                    return response;

                }
                else
                {

                    var result = request.Url == "/" ? await client.GetAsync($"/getSystemStatus?_={DateTimeOffset.Now.ToUnixTimeSeconds()}") : await client.GetAsync(request.Url);

                    _logger.LogInformation($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");

                    if (!result.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Request : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode}");

                    }

                    var response = await result.Content.ReadAsStringAsync();

                    _logger.LogInformation($"Response : {request.Url} Type : {request.Type} StatusCode : {result.StatusCode} ResponseData : {response}");


                    if (request.InverseParent.Any())
                        return await ExecuteStep(client, request.InverseParent.First());
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"MethodName : {nameof(ExecuteStep)} Request : {request.Url} Type : {request.Type} Error : {ex.Message} Inner Exception : {ex.InnerException?.Message}");

                throw;
            }

        }
        //private async Task<string> ExecuteStep(HttpClient client, Request request)
        //{

        //    try
        //    {
        //        if (request.Type == "POST")
        //        {
        //            if (request.Url.Equals("/authentication/authenticate_form"))
        //                return "result : true";
        //            if (request.Url.Equals("/maintenance/alignment/set/confirm"))
        //                return "success : true";

        //            return "";
        //        }
        //        else
        //        {
        //            if (client.BaseAddress.ToString().Contains(".68"))
        //            {
        //                if (request.Url.Equals("/system/enforcement/1"))
        //                    MockDevices[1] = "Active";
        //                if (request.Url.Equals("/system/enforcement/0"))
        //                    MockDevices[1] = "Passive";

        //                return MockDevices[1];
        //            }
        //            if (client.BaseAddress.ToString().Contains(".69"))
        //            {
        //                if (request.Url.Equals("/system/enforcement/1"))
        //                    MockDevices[2] = "Active";
        //                if (request.Url.Equals("/system/enforcement/0"))
        //                    MockDevices[2] = "Passive";

        //                return MockDevices[2];
        //            }

        //            return "Unknown";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"MethodName : {nameof(ExecuteStep)} Request : {request.Url} Type : {request.Type} Error : {ex.Message}");

        //        throw;
        //    }

        //}
    }
}
