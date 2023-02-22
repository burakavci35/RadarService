using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Worker
{
    public class StepOptions
    {

        public List<Step> Steps { get; set; }
    }

    public class FormParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Request
    {

        public string RequestUrl { get; set; }
        public string Type { get; set; }
        public List<FormParameter> FormParameters { get; set; }
    }

    public class WorkerOptions
    {
        public List<Device> Devices { get; set; }
    }
    public class Device
    {
        public string Name { get; set; }
        public string BaseAddress { get; set; }
        public List<Command> Commands { get; set; }
    }

    public class Command
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Result { get; set; }
        public string Response { get; set; }
        public int? NextStep { get; set; }
        public Request Request { get; set; }
    }
}
