using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach
{
    public class User
    {
        public int user_id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
    }
}
