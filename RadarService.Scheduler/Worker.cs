using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;

namespace RadarService.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<RadarDbContext>();

                _logger.LogInformation("Worker Started at: {time}", DateTimeOffset.Now);
                using (var deviceRepository = new Repository<Device>(context))
                {
                    var activeDevices = await deviceRepository.GetAll().Where(x => x.IsActive)
                   .Include(x => x.DeviceSchedulers).ThenInclude(x => x.Scheduler)
                   .Include(x => x.DeviceCommands).ThenInclude(x => x.Command).ThenInclude(x => x.StepRequests).ThenInclude(x => x.Step).ToListAsync();

                    foreach (var device in activeDevices)
                    {
                        var deviceExecuter = new DeviceExecuter(_logger,_serviceProvider);

                        await deviceExecuter.Execute(device);

                    }

                }

               
                _logger.LogInformation("Worker Completed at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromSeconds(Convert.ToInt32(_configuration.GetSection("IntervalSeconds").Value)), stoppingToken);
            }
        }

    }
}