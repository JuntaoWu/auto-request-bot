using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using test.DAL;

namespace test
{
    public class FaceSyncService
    {
        public event EventHandler<CustomEventArgs> OnSynced;

        protected FaceSyncService()
        {

        }

        private static FaceSyncService instance;

        // Methods
        public static FaceSyncService Instance
        {
            get
            {
                // Uses "Lazy initialization"
                if (instance == null)
                {
                    instance = new FaceSyncService();
                }

                return instance;
            }
        }

        public async Task Run()
        {
            var url = $"{Constant.Host}/api/member";
            var response = await HttpUtil.Request(url, "GET");

            var obj = JsonConvert.DeserializeObject<ResponseResult<List<MemberCheckIn>>>(response);

            var allFaceList = obj.data.SelectMany(member =>
            {
                return member.faceList != null ? member.faceList : new List<string>();
            }).ToList();

            allFaceList.Sort();

            IEnumerable<Task> tasks = allFaceList.Distinct().Select(async (faceUri) =>
            {
                string path = $"{Settings1.Default.RootFolder}{faceUri}";
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(new DirectoryInfo(path).Parent.FullName);
                    Stream x = await DownloadAsync(faceUri);
                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    await x.CopyToAsync(fileStream);
                    fileStream.Dispose();
                }
            });

            await Task.WhenAll(tasks);

            OnSynced(this, new CustomEventArgs());
        }

        private async Task<Stream> DownloadAsync(string faceUri)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage result;

            Stream stream = await client.GetStreamAsync($"{Constant.Host}{faceUri}");

            return stream;
        }
    }
}
