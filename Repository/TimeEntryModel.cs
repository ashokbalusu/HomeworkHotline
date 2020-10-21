using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TimeEntryModel
    {
        public int TimeEntryID { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<System.DateTime> ActualEndTime { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> PayrollDate { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MNPSEmployeeNo { get; set; }
        public bool Volunteer { get; set; }
        public double Duration
        {
            get
            {
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    return Math.Round((EndTime.Value.Subtract(StartTime.Value)).TotalHours,2);
                }
                return 0.00;
            }
            set { }
        }
        public string FullName { get; set; }

    }

    
}
