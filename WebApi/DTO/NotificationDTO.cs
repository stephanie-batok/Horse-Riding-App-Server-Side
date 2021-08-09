using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class NotificationDTO
    {
        public int notification_id;
        public string user_id;
        public string title;
        public string text;
        public string dateStr;
        public string timeStr;
        public DateTime? dateTime;
        public int? lesson_id;
    }
}