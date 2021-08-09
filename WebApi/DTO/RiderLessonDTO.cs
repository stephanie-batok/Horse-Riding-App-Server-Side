using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class RiderLessonDTO
    {
        public int lesson_id;
        public string date;
        public string start_time;
        public string end_time;
        public string rider_id;
        public string horse_name;
        public string instructor_fullName;
        public string field;
        public string lesson_type;
    }
}