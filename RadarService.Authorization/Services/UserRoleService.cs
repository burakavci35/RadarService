using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        public UserRoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
        }

        public async Task CreateAsync(IdentityUserRole<string> entity)
        {
             _applicationDbContext.UserRoles.Add(entity);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IdentityUserRole<string> entity)
        {
           _applicationDbContext.UserRoles.Remove(entity);
           await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<IdentityUserRole<string>?> GetByIdAsync(params object[] keyValues)
        {
            return await _applicationDbContext.UserRoles.FindAsync(keyValues);
        }

        public IQueryable<IdentityUserRole<string>> GetList()
        {
            return _applicationDbContext.UserRoles;
        }

        public async Task UpdateAsync(IdentityUserRole<string> entity)
        {
             _applicationDbContext.UserRoles.Update(entity);
            
             await _applicationDbContext.SaveChangesAsync();
        }
    }
}
