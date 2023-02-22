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
{[Area("Radar")]
    [Authorize]
    public class FormParametersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<FormParameter> _repository;
        private readonly IRepository<Request> _requestRepository;

        public FormParametersController(IRepository<FormParameter> repository, IMapper mapper, IRepository<Request> requestRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _requestRepository = requestRepository;
        }

        // GET: Steps
        public async Task<IActionResult> Index(int requestId)
        {
            ViewBag.Request = _mapper.Map<RequestDto>(await _requestRepository.GetByIdAsync(requestId));

            return View(_mapper.Map<List<FormParameterDto>>(await _repository.Where(x => x.RequestId == requestId).Include(x => x.Request).ToListAsync()));
        }

        // GET: Steps/Details/5


        // GET: Steps/Create
        public async Task<IActionResult> Create(int requestId)
        {
            ViewBag.Request = _mapper.Map<RequestDto>(await _requestRepository.GetByIdAsync(requestId));



            return View(new FormParameterDto()
            {
                RequestId = requestId,

            });
        }

        // POST: Steps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormParameterDto entityDto)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<FormParameter>(entityDto));
                await _repository.SaveChanges();
                return RedirectToAction(nameof(Index), new { requestId = entityDto.RequestId });
            }

            return View(entityDto);
        }

        // GET: Steps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityDto = _mapper.Map<FormParameterDto>(await _repository.GetFirstOrDefault(x => x.Id == id.Value));
            if (entityDto == null)
            {
                return NotFound();
            }

            return View(entityDto);
        }

        // POST: Steps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormParameterDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(_mapper.Map<FormParameter>(entityDto));
                    await _repository.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await IsExist(entityDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { requestId = entityDto.RequestId });
            }

            return View(entityDto);
        }

        // GET: Steps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityDto = _mapper.Map<FormParameterDto>(await _repository.GetFirstOrDefault(x => x.Id == id.Value));
            if (entityDto == null)
            {
                return NotFound();
            }

            return View(entityDto);
        }

        // POST: Steps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var step = await _repository.GetByIdAsync(id);
            if (step != null)
            {
                _repository.Remove(step);
            }

            await _repository.SaveChanges();
            return RedirectToAction(nameof(Index), new { requestId = step.RequestId });
        }

        private Task<bool> IsExist(int id)
        {
            return _repository.AnyAsync(e => e.Id == id);
        }
    }
}
