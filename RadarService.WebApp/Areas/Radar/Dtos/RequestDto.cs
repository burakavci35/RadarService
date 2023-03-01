namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class RequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Response { get; set; }
        public int? ParentId { get; set; }
    }
}
