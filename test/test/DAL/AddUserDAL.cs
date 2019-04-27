using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    public class AddUserDAL
    {
        public List<CheckInAddress> getCheckInAddressList()
        {
            List<CheckInAddress> result = new List<CheckInAddress>();
            result.Add(new CheckInAddress("", ""));
            result.Add(new CheckInAddress("四川省成都市青羊区草市街街道泰丰国际广场", "四川省成都市青羊区草市街街道泰丰国际广场"));
            result.Add(new CheckInAddress("四川省成都市高新区老成仁6号", "四川省成都市高新区老成仁6号"));
            return result;
        }

        public bool AddUser(Location userlocation, string weixin_uername, string weixin_number, string contact_name, string contact_telephone, string imagedata)
        {
            return true;
        }

        public List<MemberCheckIn> getAllMemberList() {
            List<MemberCheckIn> result = new List<MemberCheckIn>();
            result.Add(new MemberCheckIn
            {
                ID = "000001",
                openId = "0000001",
                avatarurl ="https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "stefnjiang",
                username = "jiangshangfeng",
                telephone = "11111111",
                weixin_number = "111111",
                status = CheckInStatus.Actived,
                registertime = "2019-4-27"
            });
            result.Add(new MemberCheckIn
            {
                ID = "000002",
                openId = "0000002",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "lihan",
                username = "lihan",
                telephone = "22222222",
                weixin_number = "222222",
                status = CheckInStatus.Actived,
                registertime = "2019-4-27"
            });
            result.Add(new MemberCheckIn
            {
                ID = "000003",
                openId = "0000003",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "wujuntao",
                username = "wujuntao",
                telephone = "33333333",
                weixin_number = "3333333",
                status = CheckInStatus.Actived,
                registertime = "2019-4-27"
            });
            result.Add(new MemberCheckIn
            {
                ID = "000004",
                openId = "0000004",
                avatarurl = "https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png",
                weixin_uername = "zhangxiaolan",
                username = "zhangxiaolan",
                telephone = "44444444",
                weixin_number = "444444",
                status = CheckInStatus.Actived,
                registertime = "2019-4-27"
            });
            return result;
        }
    }

    public class CheckInAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public CheckInAddress(string name, string address)
        {
            this.Name = name;
            this.Address = address;
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
