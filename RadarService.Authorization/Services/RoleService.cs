using Microsoft.AspNetCore.Identity;
using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CreateAsync(ApplicationRole Role)
        {
            await _roleManager.CreateAsync(Role);
        }
        public async Task UpdateAsync(ApplicationRole Role)
        {
           await _roleManager.UpdateAsync(Role);
        }
        public async Task DeleteAsync(ApplicationRole Role)
        {
            await _roleManager.DeleteAsync(Role);
        }

        public async Task<ApplicationRole> GetByIdAsync(string id) => await _roleManager.FindByIdAsync(id);

        public IQueryable<ApplicationRole> GetList()
        {
            return _roleManager.Roles;
        }


    }
}
