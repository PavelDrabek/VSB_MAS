using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Agent
{
    public class PackageControl
    {
        private Dictionary<string, string[]> files;
        private Dictionary<string, int> counter;

        public PackageControl()
        {
            files = new Dictionary<string, string[]>();
            counter = new Dictionary<string, int>();
        }

        public void Add(string filename, string data, int order, int count)
        {
            Console.WriteLine("Adding {0}, package: {1}/{2}", filename, order, count);

            if(!files.ContainsKey(filename)) {
                files.Add(filename, new string[count]);
                counter.Add(filename, 0);
            }

            if(string.IsNullOrEmpty(files[filename][order])) {
                files[filename][order] = data;
                counter[filename]++;

                if(counter[filename] == count) {
                    MakeFile(filename);
                }
            }
        }

        public bool MakeFile(string filename)
        {
            Console.WriteLine("Saving file {0}", filename);
            string text = string.Join("", files[filename]);
            System.IO.File.WriteAllBytes(filename, Convert.FromBase64String(text));

            return true;
        }

        public static void Zip(string filename, string zipname)
        {
            using(ZipFile zip = new ZipFile()) {
                zip.AddDirectory(filename);
                zip.Save(zipname);
            }
        }

        public static void Unzip(string zipname, string unzipname)
        {
            if(ZipFile.IsZipFile(zipname)) {
                using(ZipFile zip1 = ZipFile.Read(zipname)) {
                    zip1.ExtractAll(unzipname);
                }
            }
        }
    }
}
