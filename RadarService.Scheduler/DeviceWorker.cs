using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using RadarService.Data.Repositories;
using RadarService.Data.UnitOfWork;
using RadarService.Entities.Models;
using RadarService.Scheduler.Bussiness;
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
        private static Dictionary<int, string> MockDevices = new Dictionary<int, string>() { { 1, "Passive" }, { 2, "Passive" } };

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
                try
                {
               
                    _logger.LogInformation("Worker Started at: {time} Version : 1.0.0", DateTimeOffset.Now);

                    _unitOfWork = new UnitOfWork(_serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>());
                    var activeDevices = await _unitOfWork.Device.GetAll().Where(x => x.IsActive).ToListAsync();

                    ParallelOptions parallelOptions = new()
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount - 1
                    };

                    await Parallel.ForEachAsync(activeDevices,parallelOptions, async (device, stoppingToken) =>
                    {
                        var scanner = new DeviceScanner(new UnitOfWork(_serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>()), device.Id, _logger);
                        await scanner.ExecuteDevice();
                    });

                    _logger.LogInformation("Worker Completed at: {time}", DateTimeOffset.Now);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Worker Error : {ex.Message}");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(Convert.ToInt32(_configuration.GetSection("IntervalSeconds").Value)), stoppingToken);
                }

            }
        }
     
    }
}