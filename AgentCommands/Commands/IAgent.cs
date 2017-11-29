namespace Agent.Commands
{
    public interface IAgent
    {
        string TAG { get; set; }
        string IP { get; set; }
        int Port { get; set; }
    }
}