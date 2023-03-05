using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Helpers;
using RadarService.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RadarService.Authorization.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMvcControllerDiscovery _mvcControllerDiscovery;

        public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMvcControllerDiscovery mvcControllerDiscovery)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mvcControllerDiscovery = mvcControllerDiscovery;
        }

        public async Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> expression)
        {
           return await _userManager.Users.AnyAsync(expression);
        }

        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _userManager.Users;
        }

        public async Task<ApplicationUser> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

       

        public async Task<LoginResult> Login(LoginDto loginDto)
        {
            var foundUser = await _userManager.FindByNameAsync(loginDto.EmployeeNumber);

            if (foundUser == null) return new LoginResult() { IsSuccess = false, Message = "Invalid UserName or Password" };

            if (!await _userManager.CheckPasswordAsync(foundUser, loginDto.Password)) return new LoginResult() { IsSuccess = false, Message = "Invalid UserName or Password" };

          
            var signInResult = await _signInManager.PasswordSignInAsync(foundUser, loginDto.Password, loginDto.Persistent, true);

            if (signInResult.Succeeded)
            {
              
                return new LoginResult() { IsSuccess = true, Message = "Login Successfully" };
            }

            if (signInResult.IsLockedOut) return new LoginResult() { IsSuccess = false, Message = "Account is locked please contact with Administrator" };

            return new LoginResult() { IsSuccess = false, Message = "Login Failed Unknown Result" };
        }

         public async Task<LoginResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var foundUser = await _userManager.FindByNameAsync(changePasswordDto.EmployeeNumber);

            if (foundUser == null) return new LoginResult() { IsSuccess = false, Message = "User Not Found!" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(foundUser);

            var result = await _userManager.ResetPasswordAsync(foundUser, token,changePasswordDto.Password);

            if (!result.Succeeded) return new LoginResult() { IsSuccess = false, Message = "Login Failed Unknown Result" };
            
            return new LoginResult() { IsSuccess = true, Message = $"User : {foundUser.EmployeeNumber} Password Changed Successfully!" };
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<LoginResult> Register(RegisterDto registerDto)
        {
            var loginResult = new LoginResult();
            var userExist = await _userManager.FindByNameAsync(registerDto.EmployeeNumber);
            if (userExist != null)
                return new LoginResult() { IsSuccess = false, Message = "User already Exist" };

            var newUser = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                EmployeeNumber = registerDto.EmployeeNumber,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.EmployeeNumber,
                EmailConfirmed = true,
                IsLogged = false,
                IsEnabled = true,
                PasswordUpdateDateTime= DateTime.Now,
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.ConfirmPassword);

            if (!result.Succeeded) return new LoginResult() { IsSuccess = false, Message = "User Creation Failed!" };


            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = "Admin", Access = JsonSerializer.Serialize(_mvcControllerDiscovery.GetControllerInfos())});
                await _userManager.AddToRoleAsync(newUser, "Admin");
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = "User", Access = JsonSerializer.Serialize(_mvcControllerDiscovery.GetControllerInfos().Where(x => x.Id.Contains("Home"))) });
                await _userManager.AddToRoleAsync(newUser, "User");
            }
            //else
            //{
            //    if (!await _userManager.IsInRoleAsync(newUser, "User"))
            //        await _userManager.AddToRoleAsync(newUser, "User");
            //}

            loginResult.IsSuccess = true;
            loginResult.Message = "User Registered Successfully";
            return loginResult;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
           await _userManager.UpdateAsync(user);
        }
    }
}
