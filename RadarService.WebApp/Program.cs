using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using RadarService.Authorization.Helpers;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.WebApp.Filters;
using RadarService.WebApp.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.Filters.Add(typeof(DynamicAuthorization))).AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = null).AddViewLocalization().AddDataAnnotationsLocalization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext))));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddDbContext<RadarDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(RadarDbContext))));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/User/AccessDenied";
});

builder.Services.AddScoped<DbContext, RadarDbContext>();
builder.Services.AddScoped<IRepository<Device>, Repository<Device>>();
builder.Services.AddScoped<IRepository<Command>, Repository<Command>>();
builder.Services.AddScoped<IRepository<Step>, Repository<Step>>();
builder.Services.AddScoped<IRepository<Request>, Repository<Request>>();
builder.Services.AddScoped<IRepository<FormParameter>, Repository<FormParameter>>();
builder.Services.AddScoped<IRepository<StepRequest>, Repository<StepRequest>>();
builder.Services.AddScoped<IRepository<Scheduler>, Repository<Scheduler>>();
builder.Services.AddScoped<IRepository<DeviceScheduler>, Repository<DeviceScheduler>>();
builder.Services.AddScoped<IRepository<DeviceCommand>, Repository<DeviceCommand>>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}



app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();





app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
         );

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


        });

app.Run();
