using Agent.Utilities;
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
        public static Command GetCommand(Command[] commands, string json)
        {
            JObject dataObj = JsonConvert.DeserializeObject<JObject>(json);
            string commandType = dataObj["type"].ToString();

            Type type = null;
            for(int i = 0; i < commands.Length; i++) {
                if(commandType.ToLower().Equals(commands[i].GetType().Name.ToLower())) {
                    type = commands[i].GetType();
                }
            }

            if(type == null) {
                Debug.Log(String.Format("Unknown command {0}", json), Logger.Level.Error);
                return null;
            }

            Command command = null;
            try {
                command = (Command)JsonConvert.DeserializeObject(json, type);
            } catch(Exception e) {
                Debug.Log(String.Format("Cannot deserialize command {0} \n {1}", json, e.Message), Logger.Level.Error);
            }

            return command;
        }

        public static string CommandToString(Command c)
        {
            return JsonConvert.SerializeObject(c);
        }

        public static Command GetCommand(string json)
        {
            JObject dataObj = JsonConvert.DeserializeObject<JObject>(json);
            //string commandType = dataObj["type"].ToString();

            string commandType = typeof(Command).Namespace + "." + dataObj["type"].ToString();

            Type type = Type.GetType(commandType, true, true);
            Command command = (Command)JsonConvert.DeserializeObject(json, type);

            return command;
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
