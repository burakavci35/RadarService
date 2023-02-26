using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Helpers;
using RadarService.Authorization.Services;
using RadarService.WebApp.Areas.Authorization.Dtos;

namespace RadarService.WebApp.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class RoleActionsController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IMvcControllerDiscovery _mvcControllerDiscovery;

        public RoleActionsController(IRoleService roleService, IMapper mapper, IMvcControllerDiscovery mvcControllerDiscovery)
        {
            _roleService = roleService;
            _mapper = mapper;
            _mvcControllerDiscovery = mvcControllerDiscovery;
        }

        public async Task<IActionResult> Index(string id)
        {
            var foundRole = await _roleService.GetByIdAsync(id);

            ViewBag.Role = _mapper.Map<RoleDto>(foundRole);

            ViewData["SelectedControllers"] = string.IsNullOrEmpty(foundRole.Access) ? new List<MvcControllerInfo>() : JsonConvert.DeserializeObject<List<MvcControllerInfo>>(foundRole.Access);

            return View();
        }

        public IActionResult GetControllers()
        {
            return Json(_mvcControllerDiscovery.GetControllerInfos());
        }
        [HttpPost]
        public async Task<IActionResult> AssignActionToRole(string id, IEnumerable<string> selectedControllers)
        {

            if (id != null)
            {
                var foundRole = await _roleService.GetByIdAsync(id);

                var allControllerInfos = _mvcControllerDiscovery.GetControllerInfos();

                var foundSelectedControllers = allControllerInfos.Where(x => selectedControllers.Contains(x.Id)).ToList();

                var accessJson = JsonConvert.SerializeObject(foundSelectedControllers);

                if (foundRole.Access == accessJson) return Ok("Role Actions Updated Successfully(NoChange)");

                foundRole.Access = accessJson;

                await _roleService.UpdateAsync(foundRole);

                return Ok("Role Actions Updated Successfully...");
            }

            return NotFound();
        }
    }
}
