using Microsoft.EntityFrameworkCore;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.Scheduler;


IHost host = Host.CreateDefaultBuilder(args).UseWindowsService(options =>
    {
        options.ServiceName = ".NET Radar Service";
    })
    .ConfigureServices((hostContext, services) =>
    {

        services.AddDbContext<RadarDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString(nameof(RadarDbContext))));
        //services.AddScoped<DbContext, RadarDbContext>();
        //services.AddSingleton<IRepository<Device>, Repository<Device>>();
        //services.AddSingleton<IRepository<Command>, Repository<Command>>();
        //services.AddSingleton<IRepository<Step>, Repository<Step>>();
        //services.AddSingleton<IRepository<Request>, Repository<Request>>();
        //services.AddSingleton<IRepository<FormParameter>, Repository<FormParameter>>();
        //services.AddSingleton<IRepository<StepRequest>, Repository<StepRequest>>();
        //services.AddSingleton<IRepository<Scheduler>, Repository<Scheduler>>();
        //services.AddSingleton<IRepository<DeviceScheduler>, Repository<DeviceScheduler>>();
        //services.AddSingleton<IRepository<DeviceCommand>, Repository<DeviceCommand>>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
