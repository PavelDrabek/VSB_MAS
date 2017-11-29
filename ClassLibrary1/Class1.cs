using System;
using System.Reflection;

namespace ClassLibrary1
{
    public class Command
    {

    }

    public class Class1
    {
        void xx()
        {
            Command command = new Command();
            foreach(var methodIfo in command.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public).Where(x => x.Name == "Call")) {
                var parameters = methodIfo.GetParameters();
            }
        }
    }
}
