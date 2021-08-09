using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class VisitorDTO
    {
        public string id;
        public string first_name;
        public string last_name;
        public string gender;
        public string date_of_birth;
        public string phone_number;
        public string city;
        public string address;
        public int? weight;
        public double? height;
        public string comments;

    }
}