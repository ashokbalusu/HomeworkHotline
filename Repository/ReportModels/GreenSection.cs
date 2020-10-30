using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ReportModels
{
    public class GreenSection
    {
        public string ThroughDate { get; set; }
        public int Sessions { get; set; }
        public int StudentsParents { get; set; }
        public int Students { get; set; }
        public int Parents { get; set; }
        public double Minutes { get; set; }
        public int TeacherPositionsPerWeek { get; set; }
    }
}
