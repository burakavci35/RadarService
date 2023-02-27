using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.WebApp.Areas.Radar.Dtos;


namespace RadarService.WebApp.Areas.Radar.Controllers
{
    [Area("Radar")]
    [Authorize]
    public class DeviceRequestsController : Controller
    {
     
        private IRepository<DeviceRequest> _repository;  
        private IRepository<Device> _deviceRepository;   
        private IRepository<Request> _requestRepository;
        private IMapper _mapper;



        public DeviceRequestsController(IRepository<DeviceRequest> deviceRequestRepository, IRepository<Device> deviceRepository, IRepository<Request> requestRepository, IMapper mapper)
        {
            _repository = deviceRequestRepository;
            _deviceRepository = deviceRepository;
            _requestRepository = requestRepository;
            _mapper = mapper;
        }

         public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<DeviceRequestDto>>(await _repository.GetAll().ToListAsync()));
        }

        public async Task<IActionResult> GetDeviceList()
        {
            return Json(await _deviceRepository.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync());
        }

        public async Task<IActionResult> GetRequestList()
        {
            return Json(await _requestRepository.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync());
        }

        public IActionResult CreatePartialView()
        {
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["RequestId"] = new SelectList(_requestRepository.GetAll(), "Id", "Name");

            return PartialView(new DeviceRequestDto());
        }
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(DeviceRequestDto entityDto)
        {
            if (ModelState.IsValid)
            {

                await _repository.AddAsync(_mapper.Map<DeviceRequest>(entityDto));
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DeviceRequestDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.RequestId = entityDto.RequestId;
                foundEntity.DeviceId = entityDto.DeviceId;

                _repository.Update(foundEntity);
                await _repository.SaveChanges();
                return Json(new { Success = true, Message = "" });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
               var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                _repository.Remove(foundEntity);
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message });
            }

        }
    }
}
