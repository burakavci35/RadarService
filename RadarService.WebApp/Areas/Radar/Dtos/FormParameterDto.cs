namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class FormParameterDto
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;

        public RequestDto? Request { get; set; }
    }
}
