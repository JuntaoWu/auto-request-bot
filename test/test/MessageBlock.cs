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
        CHECK_IN_SKIP,
        MEMBER_NEED_CHECK_IN_UPDATED,
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
        public string name { get; set; }
    }
}
