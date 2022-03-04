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
        public long Students { get; set; }
        public long Parents { get; set; }
        public double Minutes { get; set; }
        public int TeacherPositionsPerWeek { get; set; }
    }
}
