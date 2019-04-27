using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    public class MemberCheckIn
    {
        public string ID { get; set; }
        public string openId { get; set; }
        public string avatarurl { get; set; }
        public string weixin_uername { get; set; }
        public string username { get; set; }
        public string telephone { get; set; }
        public string weixin_number { get; set; }
        public CheckInStatus status { get; set; }
        public string registertime { get; set; }
        public string checkintime { get; set; }
    }
}
