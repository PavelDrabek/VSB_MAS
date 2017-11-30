using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Result : Command
    {
        public string status { get; set; }
        public string[] values { get; set; }
        public JObject message { get; set; }

        public override void ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
