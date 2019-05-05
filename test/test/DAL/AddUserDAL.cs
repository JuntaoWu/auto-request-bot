using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    public class AddUserDAL
    {
        public async Task<List<CheckInAddress>> getCheckInAddressList()
        {
            string url = $"http://localhost:4040/api/location";

            var result = await Request(url);

            var obj = JsonConvert.DeserializeObject<ResponseResult<List<CheckInAddress>>>(result);

            if (obj.code == 0)
            {
                return obj.data;
            }
            else
            {
                return null;
            }
        }

        private static async Task<string> Request(string url, string method = "GET", object data = null)
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

        public async Task<ResponseResult<MemberCheckIn>> AddUser(string locationId, string nickName, string wechatId, string contactName, string telephone, string imagedata, string openId)
        {
            var url = "http://localhost:4040/api/member";
            var response = await Request(url, "POST", new
            {
                openId = openId,
                nickName = nickName,
                wechatId = wechatId,
                contactName = contactName,
                telephone = telephone,
                locationId = locationId,
                avatar = imagedata,
            });

            return JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);
        }

        public async Task<bool> UpdateUser(string ID, string locationId, string nickName, string wechatId, string contactName, string telephone, string imagedata, string openId)
        {
            var url = $"http://localhost:4040/api/member/{ID}";
            var response = await Request(url, "PUT", new
            {
                openId = openId,
                nickName = nickName,
                wechatId = wechatId,
                contactName = contactName,
                telephone = telephone,
                locationId = locationId,
                avatar = imagedata,
            });

            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);
            return obj.code == 0;
        }

        public async Task<bool> DeleteUser(string ID)
        {
            var url = $"http://localhost:4040/api/member/{ID}";
            var response = await Request(url, "DELETE");

            var obj = JsonConvert.DeserializeObject<ResponseResult>(response);
            return obj.code == 0;
        }

        public async Task<List<MemberCheckIn>> getAllMemberList()
        {
            var url = $"{Constant.Host}/api/member/";
            var response = await Request(url);

            var obj = JsonConvert.DeserializeObject<ResponseResult<List<MemberCheckIn>>>(response);
            if (obj.code == 0)
            {
                return obj.data;
            }
            else
            {
                return null;
            }
        }

        public async Task<MemberCheckIn> getOneMemberById(string id)
        {
            var url = $"{Constant.Host}/api/member/{id}";
            var response = await Request(url);

            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);
            if (obj.code == 0)
            {
                return obj.data;
            }
            else
            {
                return null;
            }
        }

    }

    public class CheckInAddress
    {
        public string text { get; set; }
        public string value { get; set; }
        public CheckInAddress(string text, string value)
        {
            this.text = text;
            this.value = value;
        }
    }

    public class AddressUnity
    {
        public int status { get; set; }
        public Result result { get; set; }
    }

    public class Location
    {
        public float lng { get; set; }
        public float lat { get; set; }
    }

    public class Result
    {
        public Location location { get; set; }
        public int precise { get; set; }
        public int confidence { get; set; }
        public int comprehension { get; set; }
        public string level { get; set; }
    }

}
