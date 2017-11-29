namespace AgentModel.Agent
{
    public interface IAgent
    {
        string TAG { get; set; }
        string IP { get; }
        int Port { get; }

        void Start();
        void Stop();
    }
}