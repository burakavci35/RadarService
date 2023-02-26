using RadarService.Authorization.Resources;
using System.ComponentModel.DataAnnotations;

namespace RadarService.Authorization.Dtos
{
    public class LoginDto
    {
        [Required]
        [Display(Name = "EmployeeNumber", ResourceType = typeof(ResourceTexts))]
        [StringLength(6, MinimumLength = 6)]
        public string EmployeeNumber { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ResourceTexts))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!_%.*?&])[A-Za-z\d$@$!_%.*?&]{8,}", ErrorMessage = "Passwords must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; } = null!;
        /// <summary>
        /// Beni hatırla...
        /// </summary>
        [Display(Name = "RememberMe", ResourceType = typeof(ResourceTexts))]
        public bool Persistent { get; set; }
        public bool Lock { get; set; }
        public string? ReturnUrl { get; set; } = "/";
    }
}
