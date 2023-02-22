using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Models
{
    public class ApplicationRole: IdentityRole
    {
        public List<string> AssignedUsers;

        public string? Description { get; set; }

        public ApplicationRole()
        {
            AssignedUsers = new List<string>();
        }

        public string? Access { get; set; }
    }
}
