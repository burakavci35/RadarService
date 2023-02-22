using RadarService.Entities.Models;

namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class DeviceSchedulerDto
    {
        public int Id { get; set; }

        public int SchedulerId { get; set; }

        public int DeviceId { get; set; }

        public virtual DeviceDto? Device { get; set; }

        public virtual SchedulerDto? Scheduler { get; set; }
    }
}
