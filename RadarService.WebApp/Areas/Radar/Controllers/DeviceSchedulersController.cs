using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadarService.Entities.Models;
using RadarService.WebApp.Areas.Radar.Dtos;

using RadarService.Data.Repositories;

namespace RadarService.WebApp.Areas.Radar.Controllers
{
    [Area("Radar")]
    [Authorize]
    public class DeviceSchedulersController : Controller
    {

        private readonly IRepository<DeviceScheduler> _repository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Scheduler> _schedulerRepository;
        private readonly IMapper _mapper;

        public DeviceSchedulersController(IRepository<DeviceScheduler> repository, IMapper mapper, IRepository<Device> deviceRepository, IRepository<Scheduler> schedulerRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _deviceRepository = deviceRepository;
            _schedulerRepository = schedulerRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<DeviceSchedulerDto>>(await _repository.GetAll().ToListAsync()));
        }

        public async Task<IActionResult> GetDeviceList()
        {
            return Json(await _deviceRepository.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToListAsync());
        }

        public async Task<IActionResult> GetSchedulerList()
        {
            return Json(_mapper.Map<List<SchedulerDto>>(await _schedulerRepository.GetAll().ToListAsync()).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
        }

        public IActionResult CreatePartialView()
        {
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["SchedulerId"] = new SelectList(_schedulerRepository.GetAll(), "Id", "Name");

            return PartialView(new DeviceSchedulerDto());
        }
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(DeviceSchedulerDto entityDto)
        {
            if (ModelState.IsValid)
            {

                await _repository.AddAsync(_mapper.Map<DeviceScheduler>(entityDto));
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DeviceSchedulerDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.SchedulerId = entityDto.SchedulerId;
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
