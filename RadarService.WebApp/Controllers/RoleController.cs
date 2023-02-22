using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.WebApp.Dtos;

namespace RadarService.WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {

            _roleService = roleService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<RoleDto>>(await _roleService.GetList().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(RoleDto entityDto)
        {
            if (ModelState.IsValid)
            {

                await _roleService.CreateAsync(_mapper.Map<ApplicationRole>(entityDto));
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, RoleDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundUser = await _roleService.GetByIdAsync(id);

                if (foundUser == null) { return NotFound(); }

                foundUser.Name = entityDto.Name;
                foundUser.Description = entityDto.Description;

                await _roleService.UpdateAsync(foundUser);

                return Json(new { Success = true, Message = "" });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var foundUser = await _roleService.GetByIdAsync(id);

                if (foundUser == null) { return NotFound(); }

                await _roleService.DeleteAsync(foundUser);

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message });
            }

        }
    }
}
