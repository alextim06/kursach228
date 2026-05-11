using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach
{
    public class Application
    {
        public int application_id { get; set; }
        public int student_id { get; set; }
        public int course_id { get; set; }
        public string status { get; set; }
        public DateTime date_created { get; set; }
    }
}
