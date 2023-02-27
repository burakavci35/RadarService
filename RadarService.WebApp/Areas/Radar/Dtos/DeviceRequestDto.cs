namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class DeviceRequestDto
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int RequestId { get; set; }

        public CommandDto? Command { get; set; }

        public DeviceDto? Device { get; set; }
    }
}
