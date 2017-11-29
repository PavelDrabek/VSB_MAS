namespace Agent.Communication
{
    public interface IAckReceiver
    {
        bool ReceiveACK(string sourceIP, int sourcePort, string message);
    }
}
