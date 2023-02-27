using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

using RadarService.Authorization.Helpers;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.WebApp.Filters;
using RadarService.WebApp.Resources;
using RadarService.WebApp.ViewComponents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.Filters.Add(typeof(DynamicAuthorization)))
    .AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = null).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(
                    o => { o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(ResourceTexts)); }
                );

var supportedCultures = new[] { "en", "tr" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[1])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

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

builder.Services.AddScoped<DynamicAuthorizationService>();

builder.Services.AddScoped<DbContext, RadarDbContext>();
builder.Services.AddScoped<IRepository<Device>, Repository<Device>>();
builder.Services.AddScoped<IRepository<DeviceRequest>, Repository<DeviceRequest>>();
builder.Services.AddScoped<IRepository<Request>, Repository<Request>>();
builder.Services.AddScoped<IRepository<FormParameter>, Repository<FormParameter>>();
builder.Services.AddScoped<IRepository<Scheduler>, Repository<Scheduler>>();
builder.Services.AddScoped<IRepository<DeviceScheduler>, Repository<DeviceScheduler>>();




builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();

builder.Services.AddLocalization(options =>
           {
               // Resource (kaynak) dosyalarýmýzý ana dizin altýnda "Resources" klasorü içerisinde tutacaðýmýzý belirtiyoruz.
               options.ResourcesPath = "Resources";
           });


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


app.UseRequestLocalization(localizationOptions);


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
