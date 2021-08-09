using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class RiderDTO
    {
        public string id;
        public int? weight;
        public double? height;
        public int? riding_rank;
        public string riding_type;
        public string starting_date;
        public bool? isActive;
        public string first_name;
        public string last_name;
        public string gender;
        public string email;
        public string date_of_birth;
        public string phone_number;
        public string city;
        public string address;
        public string password;
        public List<RegularLessonDTO> regular_lessons;
        public string instructor_full_name;
        public string instructor_id;
        public int? horse_id;
        public List<ParentDTO> parents;
    }
}