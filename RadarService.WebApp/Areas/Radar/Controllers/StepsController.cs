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
    public class StepsController : Controller
    {
        private readonly IRepository<Step> _repository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Command> _commandReponsitory;
        private readonly IMapper _mapper;
        public StepsController(IRepository<Step> repository, IMapper mapper, IRepository<Command> commandReponsitory)
        {
            _repository = repository;
            _mapper = mapper;
            _commandReponsitory = commandReponsitory;
        }

        // GET: Steps
        public async Task<IActionResult> Index()
        {

            return View(_mapper.Map<List<StepDto>>(await _repository.GetAll().ToListAsync()));
        }

        // GET: Steps/Details/5


        // GET: Steps/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Steps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StepDto entityDto)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<Step>(entityDto));
                await _repository.SaveChanges();
                return RedirectToAction(nameof(Index));
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

            var entityDto = _mapper.Map<Step>(await _repository.GetByIdAsync(id.Value));
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
        public async Task<IActionResult> Edit(int id, StepDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _repository.Update(_mapper.Map<Step>(entityDto));
                    await _repository.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await StepExists(entityDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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

            var step = await _repository.GetAll()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (step == null)
            {
                return NotFound();
            }

            return View(step);
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
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> StepExists(int id)
        {
            return _repository.AnyAsync(e => e.Id == id);
        }
    }
}
