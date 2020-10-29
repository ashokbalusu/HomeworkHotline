using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReportModel
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public string TotalMinutes { get; set; }
        public string TotalDollars { get; set; }
        public int SeesionsPreviousYearComparison { get; set; }
        public string PreviousYearRange { get; set; }
        public string CurrentYearRange { get; set; }
        public string QuarterMidYearName { get; set; }
        public string ReportEndDate { get; set; }
        public int TotalSessions { get; set; }
        public int TotalIndividualStudents { get; set; }
        public int TotalIndividualParents { get; set; }
        public int TotalMinutesFreeTutoring { get; set; }
        public int TotalTeacherPositionsPerWeek { get; set; }
        public string DistrictTutoringHourCost { get; set; }
        public string DistrictTutoringHourlyRate { get; set; }
        public int DistirctPromotionalItemStudents { get; set; }
        public string DistirctPromotionalItemCost { get; set; }
        public double DistirctPhonesPercentOfUsage { get; set; }
        public double DistirctPhonesCost { get; set; }
        public List<ChartModel> StudentsAndSessions { get; set; }
        public List<ChartModel> SessionResults { get; set; }
        public List<ChartModel> SubjectBreakdown { get; set; }
        public List<ChartModel> SessionsPerGrade { get; set; }
        public List<SchoolSessionStudentGrid> SchoolSessionsStudentGrid { get; set; }
    }
}
