using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using test.DAL;

namespace test
{

    public enum CheckInStatus
    {
        Waiting,
        Success,
        Error,
        Actived,
        UnActive
    }

    public enum NeeChecked
    {
        Initial,
        NoNeed,
        Need
    }

    public class CustomCheckInEventArge : EventArgs
    {
        public List<MemberCheckIn> currentdata { get; set; }
    }

    public class MemberCheckInSingletonService
    {
        private static MemberCheckInSingletonService instance;
        public List<MemberCheckIn> membercheckinlist;
        public event EventHandler OnReceiveCheckInResponse;

        public HashSet<string> checkedInIds;

        // Constructor
        protected MemberCheckInSingletonService()
        {
        }

        // Methods
        public static MemberCheckInSingletonService Instance
        {
            get
            {
                // Uses "Lazy initialization"
                if (instance == null)
                {
                    instance = new MemberCheckInSingletonService();
                    Instance.membercheckinlist = new List<MemberCheckIn>();
                    Instance.checkedInIds = new HashSet<string>();
                }

                return instance;
            }
        }

        public static async void getAllMemberCheckInOnToday(CheckInType type)
        {
            var url = $"{Constant.Host}/api/member/checkin/?type={Convert.ToInt32(type)}";

            var response = await HttpUtil.Request(url);

            var obj = JsonConvert.DeserializeObject<ResponseResult<List<MemberCheckIn>>>(response);

            Instance.membercheckinlist.Clear();

            if (obj.code == 0)
            {
                Instance.membercheckinlist = obj.data;
            }

            Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
        }

        public static async void updateMemberCheckInInformation(string openId, CheckInStatus status, string result, string message, string requestUrl)
        {
            var member = Instance.membercheckinlist.Single(m => m.openId == openId);
            string Id = member._id;
            var url = $"{Constant.Host}/api/member/checkin/{Id}";
            var response = await HttpUtil.Request(url, "PUT", new
            {
                status = Convert.ToInt32(status),
                message = message,
                result = result,
                url = requestUrl
            });

            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);

            if (obj.code == 0)
            {
                member.status = status;
                member.checkInTime = obj.data?.checkInTime;
                member.message = message;
                Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
            }
            else
            {
                // todo: Error handling
            }
        }

        public static async Task<bool> updateNeedCheckIn(List<string> needCheckInIds)
        {
            var url = $"{Constant.Host}/api/member/updateNeedCheckIn";
            var response = await HttpUtil.Request(url, "POST", new
            {
                needCheckInIds = needCheckInIds
            });

            var obj = JsonConvert.DeserializeObject<ResponseResult>(response);

            return obj.code == 0;
        }

        public static async Task updateMemberCheckInInformation(MessageBody data)
        {
            string Id = data.id;
            var url = $"{Constant.Host}/api/member/checkin/{Id}";
            var response = await HttpUtil.Request(url);
            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);

            var member = Instance.membercheckinlist.Single(m => m.openId == obj.data.openId);
            member.status = obj.data.status;
            member.checkInTime = obj.data.checkInTime;
            member.message = obj.data.message;
            Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
        }

        public static async void createNewMemberCheckInformation(MessageBody data)
        {
            string Id = data.id;
            var url = $"{Constant.Host}/api/member/checkin/{Id}";
            var response = await HttpUtil.Request(url);
            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);
            Instance.membercheckinlist.Add(obj.data);
            Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
        }
    }
}
