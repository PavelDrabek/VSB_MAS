using Agent.Communication;
using System;
using System.IO;
using System.Threading;

namespace Agent.Commands
{
    public class Duplicate : Command
    {
        public static int maxLength = 1024;
        public static string filename = "dra0042.zip";
        public static string saveFolder = "../";

        public string ip { get; set; }
        public int port { get; set; }

        public Duplicate() : base() { }
        public Duplicate(Agent agent) : base(agent) { }

        public override void Execute()
        {
            string path = saveFolder + filename;
            PackageControl.Zip(".", path);
            string text = Convert.ToBase64String(File.ReadAllBytes(path));
            int count = (int)(text.Length / maxLength) + 1;
            string[] parts = new string[count];

            for(int i = 0; i < count; i++) {
                int index = i * maxLength;
                parts[i] = text.Substring(index, Math.Min(maxLength, text.Length - index));
                Command c = new Package(Agent) { partsCount = count, data = parts[i], fileName = filename, order = i };
                new Sender(Agent, ip, port, CommandHandler.CommandToString(c)).Send();
                Thread.Sleep(1);
                if(i % 5 == 0) {
                }
            }

            // Tady se muze poslat check, jestli soubor dorazil v poradku (command ExistsFile, result true/false)
        }
    }
}
