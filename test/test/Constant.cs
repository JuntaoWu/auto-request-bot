using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Constant
    {
        public const string Host = "http://arb.hzsdgames.com";
        //public const string Host = "http://localhost:8103";
        public const string WebSocketEndpoint = "ws://ws.arb.hzsdgames.com";
        //public const string WebSocketEndpoint = "ws://localhost:8105";
    }

    public enum CheckInType
    {
        CheckIn,
        CheckOut,
    }
}
