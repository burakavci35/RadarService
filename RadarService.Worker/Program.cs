using Microsoft.EntityFrameworkCore;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<RadarDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString(nameof(RadarDbContext))));
        services.AddScoped<DbContext, RadarDbContext>();
        services.AddScoped<IRepository<Device>, Repository<Device>>();
        services.AddScoped<IRepository<Command>, Repository<Command>>();
        services.AddScoped<IRepository<Step>, Repository<Step>>();
        services.AddScoped<IRepository<Request>, Repository<Request>>();
        services.AddScoped<IRepository<FormParameter>, Repository<FormParameter>>();
        services.AddScoped<IRepository<StepRequest>, Repository<StepRequest>>();
        services.AddScoped<IRepository<Scheduler>, Repository<Scheduler>>();
        services.AddScoped<IRepository<DeviceScheduler>, Repository<DeviceScheduler>>();
        services.AddScoped<IRepository<DeviceCommand>, Repository<DeviceCommand>>();


        services.Configure<WorkerOptions>(options => hostContext.Configuration.GetSection("WorkerOptions").Bind(options));
        services.AddOptions<WorkerOptions>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
