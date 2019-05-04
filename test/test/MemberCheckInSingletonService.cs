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

    public class CustomCheckInEventArge : EventArgs
    {
        public List<MemberCheckIn> currentdata { get; set; }
    }

    public class MemberCheckInSingletonService
    {
        private static MemberCheckInSingletonService instance;
        private List<MemberCheckIn> membercheckinlist;
        public event EventHandler OnReceiveCheckInResponse;

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
                }

                return instance;
            }
        }

        public static async void getAllMemberCheckInOnToday()
        {
            var type = CheckInType.CheckIn;
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

        public static async void updateMemberCheckInInformation(string openId, CheckInStatus status, string message)
        {
            var member = Instance.membercheckinlist.Single(m => m.openId == openId);
            string Id = member._id;
            var url = $"{Constant.Host}/api/member/checkin/{Id}";
            var response = await HttpUtil.Request(url, "PUT", new
            {
                status = Convert.ToInt32(status),
                message = message,
            });

            var obj = JsonConvert.DeserializeObject<ResponseResult<MemberCheckIn>>(response);

            if (obj.code == 0)
            {
                member.status = status;
                member.checkInTime = obj.data?.checkInTime;
                Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
            }
            else
            {
                // todo: Error handling
            }
        }
    }
}
