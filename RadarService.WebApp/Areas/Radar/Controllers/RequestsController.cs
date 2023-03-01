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
    public class RequestsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Request> _repository;

        public RequestsController(IRepository<Request> repository, IMapper mapper)
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
            return Json(_mapper.Map<List<RequestDto>>(await _repository.GetAll().ToListAsync()));
        }

        public IActionResult CreatePartialView() => PartialView();
        [HttpPost]
        public async Task<JsonResult> CreatePartialView(RequestDto entityDto)
        {

            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<Request>(entityDto));
                await _repository.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage))) });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RequestDto entityDto)
        {

           
            if (ModelState.IsValid)
            {
                var foundEntity = await _repository.GetByIdAsync(id);

                if (foundEntity == null) { return NotFound(); }

                foundEntity.Name = entityDto.Name;
                foundEntity.Url = entityDto.Url;
                foundEntity.Type = entityDto.Type;
                foundEntity.ParentId = entityDto.ParentId;
                foundEntity.Response = entityDto.Response;
              

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
