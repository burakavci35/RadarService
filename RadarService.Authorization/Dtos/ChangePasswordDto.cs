using RadarService.Authorization.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Dtos
{
    public class ChangePasswordDto
    {
        [Display(Name = "EmployeeNumber", ResourceType = typeof(ResourceTexts))]
        [StringLength(6, MinimumLength = 6)]
        public string EmployeeNumber { get; set; } = null!;

        [Display(Name = "Password", ResourceType = typeof(ResourceTexts))]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!_%.*?&])[A-Za-z\d$@$!_%.*?&]{8,}", ErrorMessage = "Passwords must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResourceTexts))]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Confirm password doesn't match, Type again !")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!_%.*?&])[A-Za-z\d$@$!_%.*?&]{8,}", ErrorMessage = "Passwords must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
