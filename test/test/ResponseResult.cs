using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{

    public class ResponseResult
    {
        public int code { get; set; }

        public string message { get; set; }
    }

    public class ResponseResult<T> : ResponseResult
    {
        public T data { get; set; }
    }
}
