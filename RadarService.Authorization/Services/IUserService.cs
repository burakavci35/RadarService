using RadarService.Authorization.Dtos;
using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public interface IUserService
    {
        public Task<LoginResult> Login(LoginDto loginDto);
        public Task<LoginResult> ChangePassword(ChangePasswordDto changePasswordDto);
        public Task Logout();
        public Task<LoginResult> Register(RegisterDto user);
        public Task CreateAsync(ApplicationUser user);
        public IQueryable<ApplicationUser> GetAll();
        public Task UpdateAsync(ApplicationUser user);
        public Task DeleteAsync(ApplicationUser user);
        public Task<ApplicationUser> GetByIdAsync(string userId);
        public Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> expression);
    }
}
