using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class HorseDTO
    {
        public int? id;
        public string name;
        public string gender;
        public string size;
        public string temper;
        public bool? is_active;
        public int? required_rank;
        public int? max_weight;
        public int? min_weight;
        public double? max_height;
        public double? min_height;
        public bool? therapeutic_riding;
        public bool? can_jump;
        public bool? is_qualified;
    }
}