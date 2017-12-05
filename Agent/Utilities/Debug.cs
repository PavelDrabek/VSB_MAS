using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Utilities
{
    public class Debug
    {
        private static Logger logger;
        private static Logger Logger { get { if(logger == null) logger = new Logger("log.txt"); return logger; } }

        public static Logger.Level LevelStore { get { return Logger.LevelStore; } set { Logger.LevelStore = value; } }
        public static Logger.Level LevelConsole { get { return Logger.LevelConsole; } set { Logger.LevelConsole = value; } }

        public static void Log(string s, Logger.Level l = Logger.Level.Debug)
        {
            Logger.Log(s, l);
        }
    }
}
