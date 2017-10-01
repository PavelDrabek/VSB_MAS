using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class CommandHandler
    {
        public static bool HandleUnparsedCommand(Agent agent, string text)
        {
            //string[] parts = text.Split(' ');
            //dynamic data = JsonConvert.DeserializeObject(text);
            //string command = data[0];

            JArray parts = JsonConvert.DeserializeObject<JArray>(text);
            
            if(parts.Count > 0) {
                string command = parts[0].ToString().ToLower();
                if(parts.Count > 1) {
                    string[] args = new string[parts.Count - 1];

                    for(int i = 0; i < parts.Count - 1; i++)
                        args[i] = parts[i + 1].ToString();

                    return agent.HandleCommand(command, args);
                } else {
                    return agent.HandleCommand(command);
                }
            }

            return false;
        }

        public bool HandleCommand(Agent agent, string command, params string[] args)
        {
            var c = agent.GetCommand(command);
            if(c != null) {
                return CommandHandler.HandleCommand(c, args);
            }

            Console.WriteLine("Unknown message");
            return false;
        }


        public static bool HandleCommand(Command command, params string[] args)
        {
            foreach(var methodIfo in command.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public).Where(x => x.Name == "Call")) {
                var parameters = methodIfo.GetParameters();

                if(parameters.Length != args.Length)
                    continue;

                object[] objs = new object[args.Length];
                bool isValid = true;

                for(int i = 0; i < parameters.Length; i++) {
                    if(parameters[i].ParameterType == typeof(string)) {
                        objs[i] = args[i];
                        continue;
                    }

                    var tryParseMethod = parameters[i].ParameterType.GetMethod("TryParse", new Type[] { typeof(string), parameters[i].ParameterType.MakeByRefType() });
                    if(!(bool)tryParseMethod.Invoke(command, new object[] { args[i], objs[i] })) {
                        isValid = false;
                        break;
                    }

                    var parseMethod = parameters[i].ParameterType.GetMethod("Parse", new Type[] { typeof(string) });
                    objs[i] = parseMethod.Invoke(command, new object[] { args[i] });
                }

                if(isValid) {
                    methodIfo.Invoke(command, objs);
                    return true;
                }
            }

            Console.WriteLine("Invalid arguments");
            return false;
        }
    }
}
