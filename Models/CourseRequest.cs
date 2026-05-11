using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach
{
    public class CourseRequest
    {
        public int request_id { get; set; }
        public int teacher_id { get; set; }
        public string title { get; set; }
        public int hours { get; set; }
        public string format { get; set; }
        public string schedule_file { get; set; }
        public string status { get; set; }
    }
}
