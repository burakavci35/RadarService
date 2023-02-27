using RadarService.Data.Repositories;
using RadarService.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Data.UnitOfWork
{
    public interface IUnitOfwork
    {
        IRepository<Device> Device { get; }
        IRepository<DeviceLog> DeviceLog { get; }
        IRepository<Scheduler> Scheduler { get; }
        IRepository<DeviceScheduler> DeviceScheduler { get; }
        IRepository<FormParameter> FormParameter { get; }
        IRepository<DeviceRequest> DeviceRequest { get; }
        IRepository<Request> Request { get; }
        IRepository<ResponseCondition> ResponseCondition { get; }    
        Task SaveChangesAsync();
        void Dispose();
    }
}
