using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.DAL
{
    public class Member
    {
        public bool IsChecked { get; set; }
        public string ID { get; set; }
        public string avatarUrl { get; set; }
        public Image avatar { get; set; }
        public string weixin_uername { get; set; }
        public string username { get; set; }
        public string telephone { get; set; }
        public string weixin_number { get; set; }
        public string status { get; set; }
        public string registertime { get; set; }
        public string checkintime { get; set; }
        public string checkin_addressId { get; set; }
        public string message { get; set; }
    }
}
