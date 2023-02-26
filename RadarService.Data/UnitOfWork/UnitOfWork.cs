using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RadarService.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfwork, IDisposable
    {
        private readonly RadarDbContext _context;

        public UnitOfWork(RadarDbContext context)
        {
            _context = context;
        }
        private IRepository<Device> _device;
        private IRepository<DeviceLog> _deviceLog;
        private IRepository<Scheduler> _scheduler;
        private IRepository<DeviceScheduler> _deviceScheduler;
        private IRepository<DeviceCommand> _deviceCommand;
        private IRepository<Command> _command;
        private IRepository<FormParameter> _formParameter;
        private IRepository<Request> _request;
        private IRepository<ResponseCondition> _responseCondition;
        private IRepository<Step> _step;
        private IRepository<StepRequest> _stepRequest;


        public IRepository<Device> Device => _device ??= new Repository<Device>(_context);

        public IRepository<DeviceLog> DeviceLog => _deviceLog ??= new Repository<DeviceLog>(_context);

        public IRepository<Scheduler> Scheduler => _scheduler ??= new Repository<Scheduler>(_context);

        public IRepository<DeviceScheduler> DeviceScheduler => _deviceScheduler ??= new Repository<DeviceScheduler>(_context);

        public IRepository<DeviceCommand> DeviceCommand => _deviceCommand ??= new Repository<DeviceCommand>(_context);

        public IRepository<Command> Command => _command ??= new Repository<Command>(_context);

        public IRepository<FormParameter> FormParameter => _formParameter ??= new Repository<FormParameter>(_context);

        public IRepository<Request> Request => _request ??= new Repository<Request>(_context);

        public IRepository<ResponseCondition> ResponseCondition => _responseCondition ??= new Repository<ResponseCondition>(_context);

        public IRepository<Step> Step => _step ??= new Repository<Step>(_context);

        public IRepository<StepRequest> StepRequest => _stepRequest ??= new Repository<StepRequest>(_context);

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

      
    }
}
