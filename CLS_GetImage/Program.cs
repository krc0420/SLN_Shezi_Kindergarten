using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CLS_GetImage
{
    class Program
    {
        private static string _imageBaseURL = @"http://web.stps.tp.edu.tw/eweb/module/activephoto/";
        static void Main(string[] args)
        {
            int seq = 1;
            List<string> albumUrlList = ReadAlbumUrlListFile();
            Dictionary<string, List<string>> albumDict = new Dictionary<string, List<string>>();

            foreach (string url in albumUrlList)
            {
                Console.WriteLine(url);
                List<string> imageUrlList = new List<string>();
                string content = GetImagesContent(url);
                List<string> jpgPaths = content.Split(new char[] { ',' }).ToList<string>();
                string topic = jpgPaths[0];
                string date = jpgPaths[1];
                string albumName = (topic + date).Replace(";","");
                Console.WriteLine(albumName);
                jpgPaths.RemoveAt(0);
                jpgPaths.RemoveAt(1);

                foreach (string path in jpgPaths)
                {
                    // activephoto/ 開頭表示為真正圖檔路徑
                    if (path.IndexOf("activephoto/") == 0)
                    {
                        string realPath = _imageBaseURL + path;
                        imageUrlList.Add(realPath);
                        Console.WriteLine(realPath);
                    }
                }
                if (albumDict.ContainsKey(albumName) == true)
                    albumName = albumName+ "_" + seq.ToString();
                
                albumDict.Add(albumName, imageUrlList);
            }
            WriteAlbumDictionaryToJsonFile(albumDict);
        }

        private static void WriteAlbumDictionaryToJsonFile(Dictionary<string, List<string>> albumDict)
        {
            string json = JsonConvert.SerializeObject(albumDict, Formatting.Indented);
            File.WriteAllText(@"AlbumDictionary.json", json);
        }

        private static string GetImagesContent(string url)
        {
            string content = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //GET Method  
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;
                        content = responseContent.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpClient Error");
                throw;
            }
            return content;
        }

        private static List<string> ReadAlbumUrlListFile()
        {
            JArray array = JArray.Parse(File.ReadAllText(@"AlbumUrlList.json"));
            List<string> albumUrlList = array.ToObject<List<string>>();
            return albumUrlList;
        }
    }
}
