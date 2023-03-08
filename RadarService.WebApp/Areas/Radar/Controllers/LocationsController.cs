using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using RadarService.WebApp.Areas.Radar.Dtos;

namespace RadarService.WebApp.Areas.Radar.Controllers
{
    [Area("Radar")]
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly IRepository<Location> _repository;
        private readonly IMapper _mapper;

        public LocationsController(IRepository<Location> repository, IMapper mapper)
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
            return Json(_mapper.Map<List<LocationDto>>(await _repository.GetAll().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(LocationDto entityDto)
        {

            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<Location>(entityDto));
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LocationDto entityDto)
        {
            if (ModelState.IsValid)
            {
                var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.Name = entityDto.Name;

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
