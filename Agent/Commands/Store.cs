﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Store : Command
    {
        public string value { get; set; }

        public Store()
        {
        }

        public override void ExecuteCommand()
        {
            using(StreamWriter file = new System.IO.StreamWriter("log.txt")) {
                file.WriteLine(value);
                file.Close();
            }
        }

        ~Store() 
        {
        }
    }
}
