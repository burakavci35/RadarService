using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Models;


namespace RadarService.WebApp.ViewComponents
{
    public class DynamicAuthorizationService
    {
          private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly UserManager<ApplicationUser> _userManager;

        public DynamicAuthorizationService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

         public async Task<bool> IsAreaExist(ApplicationUser user, string areaName)
        {
            if (user == null)
                return false;
            var roleNameList = await _userManager.GetRolesAsync(user);

            var roles = await _roleManager.Roles.Where(x => roleNameList.Contains(x.Name)).ToListAsync();

            var existRoles = roles.Where(x => !string.IsNullOrEmpty(x.Access)).ToList();

            if (!existRoles.Any())
                return false;

            return existRoles.Select(role => JsonConvert.DeserializeObject<List<MvcControllerInfo>>(role.Access)).Any(accessList => accessList.Any(x => x.AreaName == areaName));
        }

         public async Task<bool> IsControllerExist(ApplicationUser user, string controllerName)
        {
            if (user == null)
                return false;
            var roleNameList = await _userManager.GetRolesAsync(user);

            var roles = await _roleManager.Roles.Where(x => roleNameList.Contains(x.Name)).ToListAsync();

            var existRoles = roles.Where(x => !string.IsNullOrEmpty(x.Access)).ToList();

            if (!existRoles.Any())
                return false;
            return existRoles.Select(role => JsonConvert.DeserializeObject<List<MvcControllerInfo>>(role.Access)).Any(accessList => accessList.Any(x => x.Name == controllerName));
        }
       
    }
}
