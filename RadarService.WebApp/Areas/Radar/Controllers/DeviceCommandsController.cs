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
    public class DeviceCommandsController : Controller
    {
        private IRepository<DeviceCommand> _repository;
        private IRepository<Device> _deviceRepository;
        private IRepository<Command> _commandRepository;
        private IMapper _mapper;


        public DeviceCommandsController(IRepository<DeviceCommand> repository, IRepository<Device> deviceRepository, IRepository<Command> commandRepository, IMapper mapper)
        {
            _repository = repository;
            _deviceRepository = deviceRepository;
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        // GET: DeviceSchedulers
        public async Task<IActionResult> Index()
        {

            return View(_mapper.Map<List<DeviceCommandDto>>(await _repository.GetAll().Include(x => x.Device).Include(x => x.Command).ToListAsync()));
        }

        // GET: Steps/Create
        public IActionResult Create()
        {
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name");

            return View();
        }

        // POST: Steps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceCommandDto entityDto)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(_mapper.Map<DeviceCommand>(entityDto));
                await _repository.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name");
            return View(entityDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityDto = _mapper.Map<DeviceCommandDto>(await _repository.GetByIdAsync(id.Value));
            if (entityDto == null)
            {
                return NotFound();
            }
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name");
            return View(entityDto);
        }
        // POST: Steps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceCommandDto entityDto)
        {
            if (id != entityDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _repository.Update(_mapper.Map<DeviceCommand>(entityDto));
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
            ViewData["DeviceId"] = new SelectList(_deviceRepository.GetAll(), "Id", "Name");
            ViewData["CommandId"] = new SelectList(_commandRepository.GetAll(), "Id", "Name");
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
                .Include(s => s.Device).Include(s => s.Command)
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
