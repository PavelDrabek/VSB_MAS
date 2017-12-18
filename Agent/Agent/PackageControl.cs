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

        private static string sourceFolder = "";
        private static string saveFolder = "";

        public PackageControl()
        {
            files = new Dictionary<string, string[]>();
            counter = new Dictionary<string, int>();
        }

        public bool Add(string filename, string data, int order, int count, string sender)
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
                    return MakeFile(filename, "received/" + sender + "/");
                }
            }
            return false;
        }

        public bool MakeFile(string filename, string subfolder)
        {
            Console.WriteLine("Saving file {0}", filename);
            string text = string.Join("", files[filename]);
            string path = saveFolder + subfolder + filename;
            Directory.CreateDirectory(saveFolder + subfolder);
            System.IO.File.WriteAllBytes(path, Convert.FromBase64String(text));

            Unzip(path, saveFolder + subfolder);

            return true;
        }

        public static Stream Zip(string sourcePath, string destPath)
        {
            MemoryStream ms = new MemoryStream();
            using(ZipFile zip = new ZipFile()) {
                zip.AddFile("Agent.exe");
                zip.AddFile("DotNetZip.dll");
                zip.AddFile("Newtonsoft.Json.dll");
                zip.AddFile("tmp/config.xml", "");
                //zip.AddDirectory(sourcePath);
                zip.Save(ms);
            }
            return ms;
        }

        public static void Unzip(string zipPath, string destPath)
        {
            if(ZipFile.IsZipFile(zipPath)) {
                using(ZipFile zip1 = ZipFile.Read(zipPath)) {
                    zip1.ExtractAll(destPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public static void CreatePath(string filePath)
        {
            int i = filePath.LastIndexOf('/');
            if(i >= 0) {
                Directory.CreateDirectory(filePath.Substring(0, i));
            }
        }
    }
}
