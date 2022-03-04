using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ReportModels
{
    public class SessionsYTD
    {
        public string PreviousYear { get; set; }
        public string CurrentYear { get; set; }
        public int CountyId { get; set; }
        public int Sessions { get; set; }
        public string Quarter { get; set; }
    }
}
