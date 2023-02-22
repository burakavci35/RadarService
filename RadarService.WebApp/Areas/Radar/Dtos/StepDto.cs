using RadarService.Entities.Models;

namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class StepDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public CommandDto? Command { get; set; }
    }
}
