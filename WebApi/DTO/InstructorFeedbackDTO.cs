using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class InstructorFeedbackDTO
    {
        public int feedback_id;
        public int? lesson_id;
        public int? q1;
        public int? q2;
        public int? q3;
        public int? q4;
    }
}