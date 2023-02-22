using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using RadarService.WebApp.Resources;

namespace RadarService.WebApp.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = null!;

        [Display(Name = "EmployeeNumber", ResourceType = typeof(ResourceTexts))]
        [StringLength(6, MinimumLength = 6)]
        public string EmployeeNumber { get; set; } = null!;

        [Display(Name = "FirstName", ResourceType = typeof(ResourceTexts))]
        public string FirstName { get; set; } = null!;

        [Display(Name = "LastName", ResourceType = typeof(ResourceTexts))]
        public string LastName { get; set; } = null!;

        [RegularExpression("^[1-9]{1}[0-9]{9}[02468]{1}$", ErrorMessageResourceName = "IdentificationNumberErrorMessage", ErrorMessageResourceType = typeof(ResourceTexts))]
        [Display(Name = "IdentificationNumber", ResourceType = typeof(ResourceTexts))]
        public string? IdentificationNumber { get; set; }

    }
}
