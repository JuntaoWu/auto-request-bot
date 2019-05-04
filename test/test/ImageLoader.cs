using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            Thread.Sleep(1000);
            System.Net.WebResponse response = await request.GetResponseAsync();
            System.IO.Stream responseStream = response.GetResponseStream();
            Bitmap bmp = new Bitmap(responseStream);
            System.IO.MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            //var SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
            responseStream.Dispose();

            return new Bitmap(bmp, 20, 20);
        }
    }
}
