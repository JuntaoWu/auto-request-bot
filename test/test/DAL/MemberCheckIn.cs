using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    public class MemberCheckIn
    {
        public string _id { get; set; }
        public string openId { get; set; }
        public string avatarUrl { get; set; }
        public string nickName { get; set; }
        public string contactName { get; set; }
        public string telephone { get; set; }
        public string wechatId { get; set; }
        public CheckInStatus status { get; set; }
        public string registerTime { get; set; }
        public string checkInTime { get; set; }
        public string locationId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string message { get; set; }
    }
}
