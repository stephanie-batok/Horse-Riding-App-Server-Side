using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class InstructorLessonDTO
    {
        public int lesson_id;
        public string date;
        public string start_time;
        public string end_time;
        public string rider_id;
        public string rider_fullName;

        public int? horse_id;
        public string horse_name;

        public string field;
        public string lesson_type;
        public string was_present;
        public string comments;
        public double? match_rank;
    }
}