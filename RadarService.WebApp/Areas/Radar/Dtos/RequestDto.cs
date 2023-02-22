namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class RequestDto
    {
        public int Id { get; set; }

        public string Url { get; set; } = null!;

        public string Type { get; set; } = null!;

    }
}
