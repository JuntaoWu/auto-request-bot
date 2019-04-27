using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void getAllMemberCheckInOnToday() {
            Instance.membercheckinlist.Add(new MemberCheckIn
            {
                ID = "000001",
                openId = "0000001",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "stefnjiang",
                username = "jiangshangfeng",
                telephone = "11111111",
                weixin_number = "111111",
                status = CheckInStatus.Waiting,
                registertime = "2019-4-27"
            });
            Instance.membercheckinlist.Add(new MemberCheckIn
            {
                ID = "000002",
                openId = "0000002",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "lihan",
                username = "lihan",
                telephone = "22222222",
                weixin_number = "222222",
                status = CheckInStatus.Waiting,
                registertime = "2019-4-27"
            });
            Instance.membercheckinlist.Add(new MemberCheckIn
            {
                ID = "000003",
                openId = "0000003",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "wujuntao",
                username = "wujuntao",
                telephone = "33333333",
                weixin_number = "3333333",
                status = CheckInStatus.Success,
                registertime = "2019-4-27"
            });
            Instance.membercheckinlist.Add(new MemberCheckIn
            {
                ID = "000004",
                openId = "0000004",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "zhangxiaolan",
                username = "zhangxiaolan",
                telephone = "44444444",
                weixin_number = "444444",
                status = CheckInStatus.Error,
                registertime = "2019-4-27"
            });

            Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
        }

        public static void updateMemberCheckInInformation(string openId,CheckInStatus status) {
            Instance.OnReceiveCheckInResponse(Instance, new CustomCheckInEventArge { currentdata = Instance.membercheckinlist });
        }
    }
}
