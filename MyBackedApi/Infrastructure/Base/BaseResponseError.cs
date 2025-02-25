using System;

namespace Infrastructure.Base
{
    public class BaseResponseError
    {
        public string Message { get; set; }
        public string Error { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
