using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ReportModels
{
    public class District
    {
        public int CountyId { get; set; }
        public string DistrictName { get; set; }
        public double TotalTutoringHours { get; set; }
        public double TotalPromotionalItems { get; set; }
        public long CurrentYearStudents { get; set; }
    }
}
