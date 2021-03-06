using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class WorkerDTO
    {
        public string id;
        public string first_name;
        public string last_name;
        public string gender;
        public string email;
        public string date_of_birth;
        public string starting_date;
        public string phone_number;
        public string city;
        public string address;
        public string password;
        public bool isAllowed;
        public string user_type;
    }
}