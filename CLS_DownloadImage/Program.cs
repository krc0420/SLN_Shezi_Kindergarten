using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CLS_DownloadImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, List<string>> albumDictionary = ReadAlbumDictionaryFile();

            foreach (var item in albumDictionary)
            {
                string albumName = item.Key;
                List<string> imageUrlList = item.Value;
                string albumFolderPath = "./Shezi_Kindergarten_Photo/" + albumName;

                if (Directory.Exists(albumFolderPath) == false)
                    Directory.CreateDirectory(albumFolderPath);

                foreach (string url in imageUrlList)
                {
                    int idx = url.LastIndexOf("/") + 1;
                    string fileName = url.Substring(idx);
                    DownloadImage(url, albumFolderPath, fileName);
                }
            }
        }

        private static void DownloadImage(string url, string albumFolderPath, string fileName)
        {
            using (WebClient client = new WebClient())
            {
                string fullPath = Path.Combine(albumFolderPath, fileName);
                Console.WriteLine(fullPath);
                client.DownloadFile(new Uri(url), fullPath);
                //OR 
                //client.DownloadFileAsync(new Uri(url), @"c:\temp\image35.png");
            }
        }

        private static Dictionary<string, List<string>> ReadAlbumDictionaryFile()
        {
            JObject obj = JObject.Parse(File.ReadAllText(@"AlbumDictionary.json"));
            Dictionary<string, List<string>> albumDictionary = obj.ToObject<Dictionary<string, List<string>>>();
            return albumDictionary;
        }
    }
}
