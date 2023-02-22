using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Client
{
    public class RadarLoginConfig
    {
         public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string SuccessCondition { get; set; }
    }
}
