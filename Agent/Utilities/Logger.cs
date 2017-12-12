using System;
using System.IO;

namespace Agent.Utilities
{
    public class Logger
    {
        [Flags]
        public enum Level { None = 0, Error = 1, Warning = 2, Debug = 4, Agent = 8, Command = 16, Plan = 32 }

        public string Path { get; private set; }
        public Level LevelStore { get; set; }
        public Level LevelConsole { get; set; }

        public Logger(string path)
        {
            Path = path;
            Write("", false);
            LevelStore = Level.Error | Level.Warning | Level.Plan | Level.Command;
            LevelConsole = Level.Error | Level.Warning | Level.Plan; // | Level.Command;
        }

        public Logger(string path, Level store) : this(path)
        {
            LevelStore = store;
        }

        public void Log(string s, Level l = Level.Debug)
        {
            if((LevelStore & l) != 0) {
                Write(s);
            }
            if((LevelConsole & l) != 0) {
                Console.WriteLine(s);
            }
        }

        private void Write(string s, bool append = true)
        {
            try {
                using(StreamWriter file = new System.IO.StreamWriter(Path, append)) {
                    file.WriteLine(s);
                    file.Close();
                }
            } catch {
                Console.WriteLine(s);
            }
        }
    }
}
