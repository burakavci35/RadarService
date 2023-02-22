using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RadarService.Authorization.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Dtos
{
    public class RegisterDto
    {
        [Display(Name = "EmployeeNumber", ResourceType = typeof(Validation))]
        [StringLength(6, MinimumLength = 6)]
        public string EmployeeNumber { get; set; } = null!;

        [Display(Name = "FirstName", ResourceType = typeof(Validation))]
        public string FirstName { get; set; } = null!;

        [Display(Name = "LastName", ResourceType = typeof(Validation))]
        public string LastName { get; set; } = null!;

        [RegularExpression("^[1-9]{1}[0-9]{9}[02468]{1}$", ErrorMessageResourceName = "IdentificationNumberErrorMessage", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "IdentificationNumber", ResourceType = typeof(Validation))]
        public string? IdentificationNumber { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(Validation))]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Validation))]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!_%.*?&])[A-Za-z\d$@$!_%.*?&]{8,}", ErrorMessage = "Passwords must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Validation))]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Confirm password doesn't match, Type again !")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!_%.*?&])[A-Za-z\d$@$!_%.*?&]{8,}", ErrorMessage = "Passwords must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
