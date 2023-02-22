using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.WebApp.Dtos;

namespace RadarService.WebApp.Controllers
{
    public class RoleUsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleService _userRoleService;
        private readonly IMapper _mapper;

        public RoleUsersController(IMapper mapper, IUserService userService, IRoleService roleService, IUserRoleService userRoleService)
        {
            _mapper = mapper;
            _userService = userService;
            _roleService = roleService;
            _userRoleService = userRoleService;
        }

        public async Task<IActionResult> Index(string id)
        {
            ViewBag.Role = _mapper.Map<RoleDto>(await _roleService.GetByIdAsync(id));


            return View();
        }

        public async Task<IActionResult> GetList(string roleId)
        {
            return Json(_mapper.Map<List<UserRoleDto>>(await _userRoleService.GetList().Where(x=>x.RoleId == roleId).ToListAsync()));
        }

        public async Task<IActionResult> GetUserList()
        {
            var list = await _userService.GetAll().Select(x => new SelectListItem() { Text = $"{x.FirstName} {x.LastName} ({x.EmployeeNumber})", Value = x.Id }).ToListAsync();

            return Json(list);
        }

        public async Task<IActionResult> GetRoleList()
        {
            var list = await _roleService.GetList().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id }).ToListAsync();

            return Json(list);
        }

        public async Task<IActionResult> CreatePartialView(string id)
        {
            var existingUsers = await _userRoleService.GetList().Where(x=>x.RoleId == id).Select(x=>x.UserId).ToListAsync();
            ViewData["UserId"] = new SelectList(_userService.GetAll().Where(x=>!existingUsers.Contains(x.Id)).Select(x=>new { x.Id,Name=$"{x.FirstName} {x.LastName} ({x.EmployeeNumber})" }), "Id", "Name");
            ViewData["RoleId"] = new SelectList(_roleService.GetList(),"Id","Name");
            return PartialView(new UserRoleDto(){RoleId = id});
        }
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(UserRoleDto entityDto)
        {
            if (ModelState.IsValid)
            {
            
                await _userRoleService.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>()
                {
                    UserId = entityDto.UserId,
                    RoleId = entityDto.RoleId,
                });
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, string roleId, UserRoleDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundUserRole = await _userRoleService.GetByIdAsync(userId);

                if (foundUserRole == null) { return NotFound(); }


                foundUserRole.UserId = entityDto.UserId;
                foundUserRole.RoleId = entityDto.RoleId;

                await _userRoleService.UpdateAsync(foundUserRole);

                return Json(new { Success = true, Message = "" });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string userId, string roleId)
        {
            try
            {
                var foundUser = await _userRoleService.GetByIdAsync(userId, roleId);

                if (foundUser == null) { return NotFound(); }

                await _userRoleService.DeleteAsync(foundUser);

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message });
            }

        }
    }
}
