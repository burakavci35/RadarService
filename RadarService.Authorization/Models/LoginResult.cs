using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Authorization.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }
    }
}
