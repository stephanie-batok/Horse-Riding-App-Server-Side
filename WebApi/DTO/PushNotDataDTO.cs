using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class PushNotDataDTO
    {
        public string to;
        public string title;
        public string body;
        public int badge;
        public DataDTO data;
    }
}