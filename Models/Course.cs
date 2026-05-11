using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach
{
    public class Course
    {
        public int course_id { get; set; }
        public string title { get; set; }
        public int hours { get; set; }
        public decimal price { get; set; }
        public int teacher_id { get; set; }
        public string status { get; set; }
        public string description { get; set; }
    }
}
