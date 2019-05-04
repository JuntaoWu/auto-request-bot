using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public static class HttpUtil
    {
        public static async Task<string> Request(string url, string method = "GET", object data = null)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage result;
            switch (method)
            {
                case "GET":
                    result = await client.GetAsync(url);
                    break;
                case "POST":
                    result = await client.PostAsJsonAsync(url, data);
                    break;
                case "PUT":
                    result = await client.PutAsJsonAsync(url, data);
                    break;
                case "DELETE":
                    result = await client.DeleteAsync(url);
                    break;
                default:
                    result = null;
                    break;
            }

            if (result == null)
            {
                return null;
            }

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStringAsync();
        }
    }
}
