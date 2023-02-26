using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.WebApp.Dtos;

namespace RadarService.WebApp.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<UserDto>>(await _userService.GetAll().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(RegisterDto entityDto)
        {
            if (ModelState.IsValid)
            {
              
                var result = await _userService.Register(entityDto);
                return Json(new { Success = result.IsSuccess, result.Message });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors)) });
        }

        public async Task<IActionResult> ChangePasswordPartialViewAsync(string id) => PartialView(new ChangePasswordDto { EmployeeNumber = (await _userService.GetByIdAsync(id)).EmployeeNumber });
        [HttpPost]
        public async Task<JsonResult> ChangePasswordPartialView(ChangePasswordDto entityDto)
        {
            if (ModelState.IsValid)
            {
             
                var result = await _userService.ChangePassword(entityDto);
                return Json(new { Success = result.IsSuccess, result.Message });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors)) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundUser = await _userService.GetByIdAsync(id);

                if (foundUser == null) { return NotFound(); }

                foundUser.EmployeeNumber = entityDto.EmployeeNumber;
                foundUser.FirstName = entityDto.FirstName;
                foundUser.LastName = entityDto.LastName;

                await _userService.UpdateAsync(foundUser);

                return Json(new { Success = true, Message = "" });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var foundUser = await _userService.GetByIdAsync(id);

                if (foundUser == null) { return NotFound(); }

                await _userService.DeleteAsync(foundUser);

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message });
            }

        }


    }
}
