

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RadarService.Authorization.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string EmployeeNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? IdentificationNumber { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? BirthDate { get; set; }
        public DateTime LastLoginDateTime { get; set; }
        public DateTime PasswordUpdateDateTime { get; set; }

        public bool IsLogged { get; set; }

        public bool IsEnabled { get; set; }
    }
}
