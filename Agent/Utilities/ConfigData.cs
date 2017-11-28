using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Agent.Utilities
{
    public class ConfigData
    {
        public string Tag { get; set; }
        public int Port { get; set; }
        public int PortFrom { get; set; }
        public int PortTo { get; set; }
        public AgentContact LoggerContacts { get; set; }
        public List<AgentContact> Contacts { get; set; }
        public string StartCommand { get; set; }

        static public void Serialize(ConfigData data, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
            using(TextWriter writer = new StreamWriter(path)) {
                serializer.Serialize(writer, data);
            }
        }

        static public ConfigData Deserialize(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ConfigData));
            TextReader reader = new StreamReader(path);
            var data = (ConfigData)deserializer.Deserialize(reader);
            reader.Close();
            return data;
        }
    }
}
