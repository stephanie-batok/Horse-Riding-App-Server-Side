using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class TrialLessonDTO
    {
        public int lesson_id;
        public string date;
        public string start_time;
        public string end_time;
        public string visitor_id;
        public string visitor_fullName;
        public VisitorDTO visitor;


        public int? horse_id;
        public string horse_name;

        public string instructor_id;
        public string instructor_fullName;

        public string field;
        public int? price;
        public string funding_source;
        public string charge_type;
        public string was_present;
        public string comments;
        public string lesson_type;
    }
}