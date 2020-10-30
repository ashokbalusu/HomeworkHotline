using Repository.ReportModels;
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
        public string TutoringProvided { get; set; }
        public int SeesionsPreviousYearComparison { get; set; }
        public string PreviousYearRange { get; set; }
        public string CurrentYearRange { get; set; }
        public string QuarterMidYearName { get; set; }
        public string ThroughDate { get; set; }
        public int TotalSessions { get; set; }
        public long TotalIndividualStudents { get; set; }
        public long TotalIndividualParents { get; set; }
        public long TotalIndividualStudentsParents { get { return TotalIndividualParents + TotalIndividualStudents; } }
        public int TotalMinutesFreeTutoring { get; set; }
        public int TotalTeacherPositionsPerWeek { get; set; }
        public double DistrictTutoringHourCost { get; set; }
        public double DistrictTutoringHours { get; set; }
        public long DistrictPromotionalItemStudents { get; set; }
        public double DistrictPromotionalItemStudentRate { get; set; }
        public double DistrictPromotionalItemCost { get; set; }
        public double DistirctPromotionalItemRate { get; set; }
        public double DistrictPhonesPercentOfUsage { get; set; }
        public double DistrictPhonesCost { get; set; }
        public double DistrictTotalCost
        {
            get
            {
                return DistrictTutoringHourCost + DistrictPromotionalItemCost + DistrictPhonesCost;
            }
        }
        public List<ChartModel> StudentsAndSessions { get; set; }
        public List<ChartModel> SessionResults { get; set; }
        public List<ChartModel> SubjectBreakdown { get; set; }
        public List<ChartModel> SessionsPerGrade { get; set; }
        public List<ReportModels.School> Schools { get; set; }
    }
}
