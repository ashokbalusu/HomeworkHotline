using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CallLogStudentModel
    {
    //    public IEnumerable<StudentModel> Students {get;set;}
    //public IEnumerable<CallLogModel> CallLogs {get;set;}

            public int CallID { get; set; }
        public string CodeName { get; set; }
        public string Grade { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? CallStart { get; set; }
        public DateTime? CallEnd { get; set; }

    }
}
