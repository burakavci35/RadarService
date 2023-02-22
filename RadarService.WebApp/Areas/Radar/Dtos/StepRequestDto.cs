namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class StepRequestDto
    {
        public int Id { get; set; }

        public int StepId { get; set; }

        public int RequestId { get; set; }

        public int CommandId { get; set; }

        public CommandDto? Command { get; set; }

        public RequestDto? Request { get; set; }

        public StepDto? Step { get; set; }
    }
}
