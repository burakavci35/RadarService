using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public interface IRoleService
    {
        public IQueryable<ApplicationRole> GetList();
        public Task CreateAsync(ApplicationRole Role);
        public Task UpdateAsync(ApplicationRole Role);
        public Task DeleteAsync(ApplicationRole Role);
        public Task<ApplicationRole> GetByIdAsync(string id);
    }
}
