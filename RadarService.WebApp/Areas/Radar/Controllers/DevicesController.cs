﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Models;
using RadarService.Authorization.Services;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.WebApp.Areas.Radar.Dtos;
using RadarService.WebApp.Dtos;

namespace RadarService.WebApp.Areas.Radar.Controllers
{
    [Area("Radar")]
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IMapper _mapper;

        public DevicesController(IRepository<Device> deviceRepository, IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<DeviceDto>>(await _deviceRepository.GetAll().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(DeviceDto entityDto)
        {
            if (ModelState.IsValid)
            {
                //return Json();
                entityDto.Status = "UnKnown";
                entityDto.IsActive = false;
                await _deviceRepository.AddAsync(_mapper.Map<Device>(entityDto));
                await _deviceRepository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DeviceDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundEntity = await _deviceRepository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.Name = entityDto.Name;
                foundEntity.BaseAddress = entityDto.BaseAddress;
                foundEntity.IsActive = entityDto.IsActive;
                foundEntity.Status = "UnKnown";

                _deviceRepository.Update(foundEntity);
                await _deviceRepository.SaveChanges();
                return Json(new { Success = true, Message = "" });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var foundEntity = await _deviceRepository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                _deviceRepository.Remove(foundEntity);
                await _deviceRepository.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message });
            }

        }
    }
}
