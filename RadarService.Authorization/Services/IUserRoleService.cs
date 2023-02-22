using Microsoft.AspNetCore.Identity;
using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public interface IUserRoleService
    {
        public IQueryable<IdentityUserRole<string>> GetList();
        public Task CreateAsync(IdentityUserRole<string> entity);
        public Task UpdateAsync(IdentityUserRole<string> entity);
        public Task DeleteAsync(IdentityUserRole<string> entity);
        public Task<IdentityUserRole<string>?> GetByIdAsync(params object[] keyValues);
    }
}
