using Agent.Communication;
using System;
using System.IO;
using System.Threading;
using AgentModel.CommandData;

namespace Agent.Commands
{
    public class Duplicate : DuplicateData
    {
        public static int maxLength = 1024;
        public static string filename = "dra0042.zip";

        public override void ExecuteCommand()
        {
            string path = filename;
            PackageControl.Zip(".", path);
            string text = Convert.ToBase64String(File.ReadAllBytes(path));
            int count = (int)(text.Length / maxLength) + 1;
            string[] parts = new string[count];

            for(int i = 0; i < count; i++) {
                int index = i * maxLength;
                parts[i] = text.Substring(index, Math.Min(maxLength, text.Length - index));
                Command c = new Package() { partsCount = count, data = parts[i], fileName = filename, order = i };
                c.SetFromAgent(Agent);
                new Sender(Agent, ip, port, CommandHandler.CommandToString(c)).Send();
                Thread.Sleep(1);
            }

            // Tady se muze poslat check, jestli soubor dorazil v poradku (command ExistsFile, result true/false)
        }
    }
}
