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

        private static string sourceFolder = "../";
        private static string saveFolder = "../received/";

        public PackageControl()
        {
            files = new Dictionary<string, string[]>();
            counter = new Dictionary<string, int>();
        }

        public void Add(string filename, string data, int order, int count)
        {
            //Console.WriteLine("Adding {0}, package: {1}/{2}", filename, order, count);

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
            string path = saveFolder + filename;
            Directory.CreateDirectory(saveFolder);
            System.IO.File.WriteAllBytes(path, Convert.FromBase64String(text));

            return true;
        }

        public static void Zip(string sourcePath, string destPath)
        {
            using(ZipFile zip = new ZipFile()) {
                zip.AddDirectory(sourcePath);
                zip.Save(destPath);
            }
        }

        public static void Unzip(string zipPath, string destPath)
        {
            if(ZipFile.IsZipFile(zipPath)) {
                using(ZipFile zip1 = ZipFile.Read(zipPath)) {
                    zip1.ExtractAll(destPath);
                }
            }
        }
    }
}
