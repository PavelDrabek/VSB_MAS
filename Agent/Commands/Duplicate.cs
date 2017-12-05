using Agent.Communication;
using Agent.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Agent.Commands
{
    public class Duplicate : Command
    {
        public static int maxLength = 1024;
        public static string filename = "dra0042.zip";

        public string ip { get; set; }
        public int port { get; set; }

        public Duplicate() : base() { }
        public Duplicate(Agent agent) : base(agent) { }

        public override void ExecuteCommand()
        {
            PrepareConfig();

            string path = filename;
            PackageControl.Zip(".", path);
            string text = Convert.ToBase64String(File.ReadAllBytes(path));
            int count = (int)(text.Length / maxLength) + 1;
            string[] parts = new string[count];

            for(int i = 0; i < count; i++) {
                int index = i * maxLength;
                parts[i] = text.Substring(index, Math.Min(maxLength, text.Length - index));
                Command c = new Package(Agent) { partsCount = count, data = parts[i], fileName = filename, order = i };
                new Sender(Agent, sourceIp, sourcePort, CommandHandler.CommandToString(c)).Send();
                Thread.Sleep(1);
            }

            // Tady se muze poslat check, jestli soubor dorazil v poradku (command ExistsFile, result true/false)
        }

        private void PrepareConfig()
        {
            ConfigData config = new ConfigData(Agent.Config) {
                Contacts = new List<AgentContact>() { Agent.Contact },
                Port = 0
            };

            ConfigData.Serialize(config, "tmp/config.xml");
        }
    }
}
