using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace test
{
    public static class AvatarManager
    {
        public static ConcurrentDictionary<string, Image> ImageCache = new ConcurrentDictionary<string, Image>();

        public async static Task<Image> GetImageAsync(string url)
        {
            Image image;
            ImageCache.TryGetValue(url, out image);

            if (image == null)
            {
                //http://thirdwx.qlogo.cn/mmopen/vi_32/PiajxSqBRaEJQ6hJ3s1c2cUicwicAHU6Efyqq67ibTBCVpEppIUoOrPiby2Dw10P2qwKUP4r4obkp9g0Ic3H7WpwickQ/132
                string uri = Regex.Replace(url, "[^A-Za-z0-9-_]+", "\\");
                string path = $"{Settings1.Default.RootFolder}\\avatar\\{uri}";
                if (File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        image = Image.FromStream(fs);
                    }

                    ImageCache[url] = image;
                }
            }

            return image;
        }

        public static async void SaveImageAsync(string url, Image image)
        {
            await Task.Run(() =>
            {
                ImageCache[url] = image;

                string uri = Regex.Replace(url, "[^A-Za-z0-9-_.]+", "\\");
                string path = $"{Settings1.Default.RootFolder}\\avatar\\{uri}";
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(new DirectoryInfo(path).Parent.FullName);
                    new Bitmap(image, image.Width, image.Height).Save(path, ImageFormat.Jpeg);
                }
            });
        }
    }
}
