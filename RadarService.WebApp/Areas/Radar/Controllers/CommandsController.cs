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
    public class CommandsController : Controller
    {
        private readonly IRepository<Command> _repository;
        private readonly IMapper _mapper;
        public CommandsController(IRepository<Command> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: Commands
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<List<CommandDto>>(await _repository.GetAll().ToListAsync()));
        }


        // GET: Commands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Commands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CommandDto entiyDto)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<Command>(entiyDto));
                await _repository.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(entiyDto);
        }

        // GET: Commands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityDto = _mapper.Map<CommandDto>(await _repository.GetByIdAsync(id.Value));
            if (entityDto == null)
            {
                return NotFound();
            }
            return View(entityDto);
        }

        // POST: Commands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommandDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(_mapper.Map<Command>(entityDto));
                    await _repository.SaveChanges();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CommandExists(entityDto.Id))
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

        // GET: Commands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var command = _mapper.Map<CommandDto>(await _repository.GetByIdAsync(id.Value));
            if (command == null)
            {
                return NotFound();
            }

            return View(command);
        }

        // POST: Commands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var command = _mapper.Map<CommandDto>(await _repository.GetByIdAsync(id));
            if (command != null)
            {
                _repository.Remove(_mapper.Map<Command>(command));
            }

            await _repository.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> CommandExists(int id)
        {
            return _repository.AnyAsync(e => e.Id == id);
        }
    }
}
