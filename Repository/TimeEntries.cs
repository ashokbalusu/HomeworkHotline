using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TimeEntries
    {
        private static HomeworkHotlineEntities db = new HomeworkHotlineEntities();

        public static IEnumerable<TimeEntryModel> GetAllTimeEntries(HomeworkHotlineEntities context)
        {

            try
            {
                var timeEntries = context.TimeEntries.Select(t => new TimeEntryModel
                {
                    TimeEntryID = t.TimeEntryID,
                    UserID = t.UserID,
                    EntryDate = t.EntryDate,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    IsLocked = t.IsLocked,
                    IsDeleted = t.IsDeleted,
                    PayrollDate = t.PayrollDate
                }).Where(t => t.IsDeleted == false).ToList();

                return timeEntries;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //   context.Configuration.ProxyCreationEnabled = true;
            }
        }

        public static TimeEntry GetOpenTimeEntriesByUser(string userId)
        {
            TimeEntry timeEntry = (from t in db.TimeEntries
                                   where t.UserID == userId
                                   && t.StartTime != null
                                   && t.EndTime == null
                                   select t).OrderByDescending(t => t.StartTime).FirstOrDefault();
            return timeEntry;

        }
        public static void ClockIn(string UserID)
        {
            try
            {
                    TimeEntry t = new TimeEntry();
                    t.UserID = UserID;
                    t.EntryDate = DateTime.Now.Date;
                    t.StartTime = RoundToNearestMinute(DateTime.Now);
                    db.TimeEntries.Add(t);
                    db.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }

        public static void ClockOut(string UserID)
        {
            TimeEntry timeEntry = (from t in db.TimeEntries
                                   where t.UserID == UserID
                                   && t.StartTime != null
                                   && t.EndTime == null
                                   select t).OrderByDescending(t => t.StartTime).FirstOrDefault();

            if (timeEntry != null)
            {
                DateTime endTime = RoundToNearestMinute(DateTime.Now);
                
                TimeEntryService timeEntryService = new TimeEntryService(db);
                timeEntry.EndTime = timeEntryService.GetEndTime(endTime, timeEntry.StartTime, timeEntry.UserID);
                timeEntry.ActualEndTime = endTime;
                db.SaveChanges();
            }
        }
        public static void ClockOutAtEndOfDay(string UserId, TimeEntry te)
        {
            if (te != null)
            {
                DateTime dte = te.EntryDate;
                te.EndTime = dte.Date.AddHours(23).AddMinutes(58).AddSeconds(00);
                te.ActualEndTime = dte.Date.AddHours(23).AddMinutes(58).AddSeconds(00);
                db.SaveChanges();
            }
        }

        private static DateTime RoundToNearestMinute(DateTime datetime)
        {
            int f = 0;
            double m = (double)(datetime.Ticks % TimeSpan.FromSeconds(60).Ticks) / TimeSpan.FromSeconds(60).Ticks;
            if (m >= 0.5)
                f = 1;
            return new DateTime(((datetime.Ticks / TimeSpan.FromSeconds(60).Ticks) + f) * TimeSpan.FromSeconds(60).Ticks);
        }
    }
}
