using Newtonsoft.Json.Linq;

namespace Agent.Commands
{
    public class ResultData : Command
    {
        public string status { get; set; }
        public string[] values { get; set; }
        public JObject message { get; set; }
    }
}
