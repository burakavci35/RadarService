using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SchedulersController : Controller
    {
        private readonly IRepository<Scheduler> _repository;
        private readonly IMapper _mapper;

        public SchedulersController(IRepository<Scheduler> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            return Json(_mapper.Map<List<SchedulerDto>>(await _repository.GetAll().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(SchedulerDto entityDto)
        {
            var StartEndTimeStrings = entityDto.DateRange?.Split(" - ");
            entityDto.StartTime = TimeSpan.Parse(StartEndTimeStrings!?.First());
            entityDto.EndTime = TimeSpan.Parse(StartEndTimeStrings!?.Last());
            entityDto.EndTime = entityDto.StartTime > entityDto.EndTime ? entityDto.EndTime.Add(new TimeSpan(1,0,0,0)) : entityDto.EndTime;
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<Scheduler>(entityDto));
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SchedulerDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.StartTime =entityDto.StartTime;
                foundEntity.EndTime = entityDto.EndTime;
                foundEntity.EndTime =entityDto.StartTime > entityDto.EndTime ? entityDto.EndTime.Add(new TimeSpan(1,0,0,0)) : foundEntity.EndTime;
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
