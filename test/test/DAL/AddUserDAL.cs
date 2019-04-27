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
