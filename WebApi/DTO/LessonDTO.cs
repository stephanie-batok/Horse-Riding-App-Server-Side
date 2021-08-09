using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class LessonDTO
    {
        public int lesson_id;
        public string date;
        public string start_time;
        public string end_time;
        public string rider_id;
        public string rider_fullName;

        public int? horse_id;
        public string horse_name;

        public string instructor_id;
        public string instructor_fullName;

        public string field;
        public string lesson_type;
        public int? price;
        public string funding_source;
        public string charge_type;
        public string was_present;
        public string comments;
        public double? match_rank;
        public HorseDTO horse;
        public InstructorDTO worker;
        public RiderDTO rider;
    }
}