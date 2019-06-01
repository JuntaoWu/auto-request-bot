using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    class RegisterDAL
    {
        public async Task<ResponseResult> Register(string username, string telephone, string password)
        {
            var data = new
            {
                username = username,
                telephone = telephone,
                password = password
            };
            var url = $"{Constant.Host}/api/usermanage/register";
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
