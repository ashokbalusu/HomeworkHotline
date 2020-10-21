using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
   public  class PrincipalModel
    {
        public string CountyName { get; set; }
        public string SchoolTypeName { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public Nullable<int> Census { get; set; }
        public string Phone { get; set; }
        public string SchoolName { get; set; }
        public int SchoolID { get; set; }
    }
}
