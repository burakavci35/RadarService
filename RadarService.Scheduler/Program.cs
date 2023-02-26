using Microsoft.EntityFrameworkCore;
using RadarService.Data.Repositories;
using RadarService.Data.UnitOfWork;
using RadarService.Entities.Models;
using RadarService.Scheduler;


IHost host = Host.CreateDefaultBuilder(args).UseWindowsService(options =>
    {
        options.ServiceName = ".NET Radar Service";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<RadarDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString(nameof(RadarDbContext))));

        services.AddScoped<IUnitOfwork, UnitOfWork>();

        services.AddHostedService<DeviceWorker>();
    })
    .Build();

await host.RunAsync();
