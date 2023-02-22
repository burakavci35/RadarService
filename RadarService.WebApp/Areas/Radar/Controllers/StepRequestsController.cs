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
    public class StepRequestsController : Controller
    {
        private readonly IRepository<StepRequest> _repository;
        private readonly IRepository<Step> _stepRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Command> _commandRepository;
        private readonly IMapper _mapper;
        public StepRequestsController(IRepository<StepRequest> repository, IMapper mapper, IRepository<Request> requestRepository, IRepository<Step> stepRepository, IRepository<Command> commandRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _requestRepository = requestRepository;
            _stepRepository = stepRepository;
            _commandRepository = commandRepository;
        }


        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<List<StepRequestDto>>(await _repository.GetAll().Include(x => x.Step).Include(x => x.Request).Include(x => x.Command).ToListAsync()));
        }

        // GET: Steps/Create
        public IActionResult Create()
        {
            ViewData["StepId"] = new SelectList(_stepRepository.GetAll(), "Id", "Name");
            ViewData["RequestId"] = new SelectList(_requestRepository.GetAll(), "Id", "Url");
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Steps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StepRequestDto entityDto)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<StepRequest>(entityDto));
                await _repository.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StepId"] = new SelectList(_stepRepository.GetAll(), "Id", "Name", entityDto.StepId);
            ViewData["RequestId"] = new SelectList(_requestRepository.GetAll(), "Id", "Url", entityDto.RequestId);
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name", entityDto.CommandId);
            return View(entityDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityDto = _mapper.Map<StepRequestDto>(await _repository.GetByIdAsync(id.Value));
            if (entityDto == null)
            {
                return NotFound();
            }
            ViewData["StepId"] = new SelectList(_stepRepository.GetAll(), "Id", "Name", entityDto.StepId);
            ViewData["RequestId"] = new SelectList(_requestRepository.GetAll(), "Id", "Url", entityDto.RequestId);
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name", entityDto.CommandId);
            return View(entityDto);
        }
        // POST: Steps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StepRequestDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _repository.Update(_mapper.Map<StepRequest>(entityDto));
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
            ViewData["StepId"] = new SelectList(_stepRepository.GetAll(), "Id", "Name", entityDto.StepId);
            ViewData["RequestId"] = new SelectList(_requestRepository.GetAll(), "Id", "Url", entityDto.RequestId);
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name", entityDto.CommandId);
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
                .Include(s => s.Step).Include(s => s.Request)
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
