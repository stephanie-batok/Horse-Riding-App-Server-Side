using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class RegularLessonDTO
    {
        public int lesson_id;
        public string day;
        public string start_time;
        public string end_time;
        public string lesson_type;
        public int? price;
        public string funding_source;
    }
}