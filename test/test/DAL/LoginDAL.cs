using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class LoginDAL
    {
        public async Task<ResponseResult> Login(string username, string password) {
            var data = new
            {
                username = username,
                password = password
            };
            var url = "http://localhost:4040/api/usermanage/login";
            HttpClient client = new HttpClient();
            HttpResponseMessage result;
            result = await client.PostAsJsonAsync(url, data);
            if (result == null)
            {
                return null;
            }

            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseResult>(response);
        }
    }
}
