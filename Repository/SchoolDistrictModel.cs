using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SchoolDistrictModel
    {
        public int SchoolDistrictID { get; set; }
        public string DistrictName { get; set; }
        public string StateCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
