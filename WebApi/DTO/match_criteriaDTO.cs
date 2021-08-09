using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class match_criteriaDTO
    {
        public int criterion_id;
        public string criterion_name;
        public string criterion_description;
        public DateTime? last_update;
        public double? criterion_weight;
        public double? min_weight;
        public double? max_weight;
    }
}