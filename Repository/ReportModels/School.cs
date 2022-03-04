using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ReportModels
{
    public class School
    {
        public string Name { get; set; }
        public int NumberOfSessions { get; set; }
        public int NumberOfStudents { get; set; }
        public int CountyId { get; set; }
    }
}
