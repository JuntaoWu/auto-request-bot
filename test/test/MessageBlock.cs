using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public enum SocketOp
    {
        ACK,
        PLAIN,
        CHECK_IN_CREATED,
        CHECK_IN_STARTED,
        CHECK_IN_UPDATED,
    }

    public class MessageBlock
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SocketOp op { get; set; }
        public MessageBody data { get; set; }
    }

    public class MessageBody
    {
        public string id { get; set; }
        public string message { get; set; }
        public string result { get; set; }
        public string allmessage { get; set; }
        public string checkInTime { get; set; }
        public string openId { get; set; }
        public string status { get; set; }
        public string avatarUrl { get; set; }
        public string nickName { get; set; }
    }
}
