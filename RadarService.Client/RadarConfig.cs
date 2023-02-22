using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Client
{
    public class RadarConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseAddress { get; set; }
       
        public RadarLoginConfig LoginConfig { get; set; }
    }
}
