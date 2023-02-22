using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RadarService.Authorization.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RadarService.WebApp.Models;
using RadarService.Authorization.Dtos;

namespace RadarService.WebApp.Filters
{
    public class DynamicAuthorization : IAsyncAuthorizationFilter
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public DynamicAuthorization(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!IsProtectedController(context))
                return;

            if (!IsUserAuthenticated(context))
                return;


            //var actionId = GetActionId(context);
            var controllerId = GetControllerId(context);

            var userName = context.HttpContext?.User?.Identity?.Name;

            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
            {
                await _signInManager.SignOutAsync();
                return;
            }

            var roleNameList = await _userManager.GetRolesAsync(appUser);

            var list = await _roleManager.Roles.Where(x => roleNameList.Contains(x.Name)).Where(x => !string.IsNullOrEmpty(x.Access))
                .Select(role => JsonConvert.DeserializeObject<List<MvcControllerInfo>>(role!.Access)).ToListAsync();

            var isRoleControllerExist = list.SelectMany(control => control.Select(y => y.Id)).Distinct().Any(ex => ex == controllerId);

            if (isRoleControllerExist)
            {
                return;
            }

            context.Result = new ForbidResult();
            return;
        }

        private static bool IsProtectedAction(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return false;

            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;

            var authorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();
            if (authorizeAttribute != null)
                return true;

            authorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
            return authorizeAttribute != null;
        }

        private bool IsProtectedController(AuthorizationFilterContext context)
        {
            //if(context.ActionDescriptor.EndpointMetadata
            //                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))

            //if (context.ActionDescriptor.EndpointMetadata
            //                     .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            //    return false;
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return false;

            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;

            var authorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();

            return authorizeAttribute != null;

            //authorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
            //return authorizeAttribute != null;
        }

        private bool IsUserAuthenticated(AuthorizationFilterContext context) => context.HttpContext.User.Identity.IsAuthenticated;

        private string GetActionId(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var area = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue;
            var controller = controllerActionDescriptor.ControllerName;
            var action = controllerActionDescriptor.ActionName;

            return $"{area}:{controller}:{action}";
        }

        private string GetControllerId(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var area = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue;
            var controller = controllerActionDescriptor.ControllerName;
            area = string.IsNullOrEmpty(area) ? "Default" : area;
            return $"{area}:{controller}";
        }
    }
}
