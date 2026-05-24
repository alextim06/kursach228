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
        public int program_id { get; set; }  // Изменено с course_id
        public string status { get; set; }
        public DateTime date_created { get; set; }
        public DateTime application_date { get; set; }
        public string course_title { get; set; } // Для JOIN запросов
        public string student_name { get; set; }  // Для JOIN запросов
    }
}
