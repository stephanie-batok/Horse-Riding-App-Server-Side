using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class ChatDTO
    {
        public int chat_num;
        public string last_message;
        public string user_id1;
        public string user_id2;
        public string user_name1;
        public string user_name2;
        public string user_profile1;
        public string user_profile2;
        public DateTime? dateTime;
        public string dateStr;
        public string timeStr;
        public string last_message_sent_by;
    }
}