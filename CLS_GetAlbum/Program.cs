using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CLS_GetAlbum
{
    class Program
    {
        private static string _albumBaseURI = @"http://web.stps.tp.edu.tw/eweb/module/activephoto/getBookData.php?home=k004&sn=4606&sn_ids=3532";
        private static string _photoBaseURI = @"http://web.stps.tp.edu.tw/eweb/module/activephoto/getPhotoData.php?home=k004&sn=4606&sn_ids=3532";
        static void Main(string[] args)
        {
            try
            {
                string content = GetAlbumContent(_albumBaseURI);
                List<string> albumList = content.Split(new char[] { ';' }).ToList<string>();
                List<AlbumsInfo> listAlbumInfo = GetAlbumsList(albumList);
                List<string> listAlbumUrl = GetAlbumUrlList(albumList, _photoBaseURI);
                WriteAlbumInfoToJsonFile(listAlbumInfo);
                WriteAlbumUrlListToJsonFile(listAlbumUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void WriteAlbumUrlListToJsonFile(List<string> listAlbumUrl)
        {
            string json = JsonConvert.SerializeObject(listAlbumUrl, Formatting.Indented);
            File.WriteAllText(@"AlbumUrlList.json", json);
        }

        private static void WriteAlbumInfoToJsonFile(List<AlbumsInfo> listAlbumInfo)
        {
            string json = JsonConvert.SerializeObject(listAlbumInfo, Formatting.Indented);
            File.WriteAllText(@"AlbumsInfo.json", json);
        }

        private static List<string> GetAlbumUrlList(List<string> albumList, string url)
        {
            List<string> albumUrlList = new List<string>();
            foreach (string album in albumList)
            {
                List<string> parameters = album.Split(new char[] { ',' }).ToList<string>();

                if (parameters.Count == 1)
                    continue;

                string bookId = parameters[8];
                string albumUrl = string.Format("{0}&book_id={1}&countup=yes", url, bookId);
                albumUrlList.Add(albumUrl);
            }
            return albumUrlList;
        }

        private static List<AlbumsInfo> GetAlbumsList(List<string> albumList)
        {
            List<AlbumsInfo> listAlbumInfo = new List<AlbumsInfo>();

            foreach (string album in albumList)
            {
                List<string> parameters = album.Split(new char[] { ',' }).ToList<string>();

                if (parameters.Count == 1)
                    continue;

                AlbumsInfo info = new AlbumsInfo()
                {
                    sequence = parameters[0],
                    albumName = parameters[1],
                    date = parameters[2],
                    jpgUrlPath = parameters[4],
                    imageCount = parameters[5],
                    bookId = parameters[8]
                };
                listAlbumInfo.Add(info);
            }
            return listAlbumInfo;
        }

        private static string GetAlbumContent(string url)
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
    }
}
