using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using RadarService.WebApp.Resources;
using System.ComponentModel.DataAnnotations;

namespace RadarService.WebApp.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;
        [Display(Name = "Name", ResourceType = typeof(ResourceTexts))]
        public string Name { get; set; } = null!;

        [Display(Name = "Description", ResourceType = typeof(ResourceTexts))]
        public string? Description { get; set; }

        public string? Access { get; set; }
        public MvcController? AllControllers { get; set; }
    }
}
