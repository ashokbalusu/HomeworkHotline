using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;


namespace Repository
{
    public class CallLogs
    {
        private static HomeworkHotlineEntities context = new HomeworkHotlineEntities();

        public static TotalCallsHoursUser GetCallsHoursByUserID(string UserID, DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@UserID",  Value =UserID , Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

           return context.Database.SqlQuery<TotalCallsHoursUser>("spc_TotalCallsHoursByUserID @UserID, @StartDate, @EndDate",sqlParams).SingleOrDefault();

       //     return (TotalCallsHoursUser)o;
        }

        public static CallLogModel GetCallLogByCallID(int CallID)
        {
            CallLogModel clm = (from c in context.CallLogs
                                where c.CallID == CallID
                                select c).FirstOrDefault();

            return clm;
        }

        public static AdminWeeklySummary GetAdminWeeklySummary(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<AdminWeeklySummary>("spc_AdminWeeklySummary @StartDate, @EndDate", sqlParams).SingleOrDefault();
        }

        public static IEnumerable<SessionsByGrade> GetSessionsByGrade(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<SessionsByGrade>("spc_SessionsByGrade @StartDate, @EndDate", sqlParams);
        }

        public static IEnumerable<EvalEndCounts> GetEvalEndCounts(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<EvalEndCounts>("spc_EvalEndCounts @StartDate, @EndDate", sqlParams);
        }

        public static IEnumerable<FeaturesUsed> GetFeaturesUsed(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<FeaturesUsed>("spc_FeaturesUsed @StartDate, @EndDate", sqlParams);
        }

        public static IEnumerable<CountyCount> GetCountyCount(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<CountyCount>("spc_CountyCount @StartDate, @EndDate", sqlParams);
        }

        public static IEnumerable<NumberOfCallsPerSubject> GetNumberOfCallsPerSubject(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<NumberOfCallsPerSubject>("spc_NumberOfCallsPerSubject @StartDate, @EndDate", sqlParams);
        }

        public static CallsInForeignLanguage GetCallsInForeignLanguage(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<CallsInForeignLanguage>("spc_CallsInForeignLanguage @StartDate, @EndDate", sqlParams).SingleOrDefault();
        }
        public static BeatMath GetBeatMath(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<BeatMath>("spc_BeatMath @StartDate, @EndDate", sqlParams).SingleOrDefault();
        }
        public static AllStudents GetAllStudents(DateTime StartDate, DateTime EndDate)
        {
            SqlParameter[] sqlParams;
            sqlParams = new SqlParameter[]
           {
                new SqlParameter { ParameterName = "@StartDate",  Value =StartDate, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@EndDate",  Value =EndDate, Direction = System.Data.ParameterDirection.Input }
           };

            return context.Database.SqlQuery<AllStudents>("spc_AllStudents @StartDate, @EndDate", sqlParams).SingleOrDefault();
        }


        //public static IEnumerable<CallLogStudentModel> GetCallLogsByUserID(string UserID)
        //{
        //    SqlParameter[] sqlParams;
        //    sqlParams = new SqlParameter[]
        //   {
        //        new SqlParameter { ParameterName = "@UserID",  Value =UserID, Direction = System.Data.ParameterDirection.Input }
        //   };

        //    return context.Database.SqlQuery<CallLogStudentModel>("spc_CallLogsByUserID @UserID", sqlParams);
        //}

        //public static IEnumerable<CallLogStudentModel> GetAllCallLogs()
        //{
        //    try
        //    {
        //        var callLogs = context.CallLogs.Select(s => new CallLogStudentModel
        //        {
        //            CallID = s.CallID,
        //            CodeName = s.Student.CodeName,
        //            Grade = s.Student.Grade,
        //            CallStart = s.CallStart,
        //            IsLocked = s.IsLocked

        //        }).Where(s => s.IsLocked == false).ToList();

        //        return callLogs;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        //   context.Configuration.ProxyCreationEnabled = true;
        //    }
        //}


    } // end class CallLogs





    public class TotalCallsHoursUser
    {
        public string UserID { get; set; }
        public string TotalHours { get; set; }
        public int TotalCalls { get; set; }
    }

    public class AdminWeeklySummary
    {
        public Nullable<decimal> TotalHours { get; set; }
        public int TotalCalls { get; set; }
    }

    public class SessionsByGrade
    {
        public string Grade { get; set; }
        public int Sessions { get; set; }
    }

    public class EvalEndCounts
    {
        public int CallCount { get; set; }
        public int EvalEndID { get; set; }
    }

    public class CountyCount
    {
        public int CallCount { get; set; }
        public string CountyName { get; set; }
    }

    public class FeaturesUsed
    {
        public string Features { get; set; }
        public int NumResult { get; set; }
    }

    public class CallsInForeignLanguage
    {
        public int NumForeignLanguageSpoken { get; set; }
    }

    public class NumberOfCallsPerSubject
    {
        public int CallCount { get; set; }
        public string SubjectTypeName { get; set; }
    }
    public class BeatMath
    {
        public int NumBeatMathSessions { get; set; }
    }
    public class AllStudents
    {
        public int NumOFStudents { get; set; }
    }
}
