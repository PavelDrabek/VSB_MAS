namespace Agent.Commands
{
    public class SendData : Command
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string message { get; set; }
    }
}
