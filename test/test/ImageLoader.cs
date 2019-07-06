using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test
{
    public static class ImageLoader
    {
        /// <summary>
        /// 根据URL生成Image对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Image> LoadImage(string url)
        {
            try
            {
                url = url.StartsWith("http") ? url : Constant.Host + url;
                HttpClient client = new HttpClient();
                var responseMessage = await client.GetAsync(url);
                responseMessage.EnsureSuccessStatusCode();

                var responseStream = await responseMessage.Content.ReadAsStreamAsync();

                Bitmap bmp = new Bitmap(responseStream);
                System.IO.MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                //var SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
                responseStream.Dispose();

                return new Bitmap(bmp, 30, 30);
            }
            catch (Exception)
            {
                return new Bitmap(@"Resources\default.jpg");
            }
        }
    }
}
