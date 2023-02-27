namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class DeviceDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string BaseAddress { get; set; } = null!;

        public string? Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }= null;
    }
}
