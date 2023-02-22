namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class DeviceCommandDto
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int CommandId { get; set; }

        public CommandDto? Command { get; set; }

        public DeviceDto? Device { get; set; }
    }
}
