using Agent.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentController
{
    public class Entry
    {
        public string name;
        public int size;
        public List<string> imports;

        public Entry()
        {
            size = 0;
            imports = new List<string>();
        }
    }

    public class DataCreator
    {
        public static List<Entry> Generate()
        {
            List<Entry> data = new List<Entry>();
            List<List<Entry>> entries = new List<List<Entry>>();

            var rand = new Random();

            int framesCount = 100;
            // person-frame ratio
            int pfRatioMin = 2;
            int pfRatioMax = 10;
            float chanceForPersonReturn = 0.4f;

            int frame = 0;
            int rep = 0;
            while(frame < framesCount) {
                int personId = entries.Count;
                if(rand.NextDouble() < chanceForPersonReturn) {
                    personId = rand.Next(0, entries.Count);
                }

                if(personId >= entries.Count) {
                    entries.Add(new List<Entry>());
                }
                int frames = rand.Next(pfRatioMin, pfRatioMax);
                for(int j = 0; j < frames; j++) {
                    var e = new Entry() { name = string.Format("rep{0}.frame_{1:0000}__person_{2:00}", rep, frame, personId) };
                    data.Add(e);
                    entries[personId].Add(e);
                    frame++;
                }
                rep++;
            }

            for(int i = 0; i < entries.Count; i++) {
                for(int j = 0; j < entries[i].Count; j++) {
                    for(int k = j + 1; k < entries[i].Count; k++) {
                        entries[i][j].imports.Add(entries[i][k].name);
                        entries[i][k].imports.Add(entries[i][j].name);
                    }
                }
            }

            return data;
        }

        public static void Generate(string path)
        {
            Save("data.json", Generate());
        }

        public static void Save(string path, List<Entry> data)
        {
            string s = CommandHandler.SerializeObject(data);
            System.IO.File.WriteAllText(path, s);
        }
    }
}
