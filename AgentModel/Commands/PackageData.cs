namespace AgentModel.CommandData
{
    public class PackageData : Command
    {
        public string data { get; set; }
        public int order { get; set; }
        public string fileName { get; set; }
        public int partsCount { get; set; }
    }
}
