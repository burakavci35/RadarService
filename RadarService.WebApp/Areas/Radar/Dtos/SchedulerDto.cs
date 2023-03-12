using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RadarService.WebApp.Areas.Radar.Dtos
{
    public class SchedulerDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [DataType(DataType.Time)]
        [BindProperty, DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan StartTime { get; set; }
        [DataType(DataType.Time)]
        [BindProperty, DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan EndTime { get; set; } 
       
        public string? DateRange { get; set; }

        public string TimeRange => $"{StartTime} - {EndTime}";
    }
}
