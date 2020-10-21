using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using System.Web;
using System.Data;
using System.Net.Http;
using System.Data.Entity;

namespace Repository
{

    public class TimeEntryService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public TimeEntryService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }


        public List<TimeEntry> GetMNPSTimeEntries(List<TimeEntry> timeEntries)
        {
            var q = (from t in timeEntries
                     join u in entities.AspNetUsers
                     on t.UserID equals u.Id
                     where u.MNPSEmployeeNo != null && t.PayrollDate == null && t.IsLocked == false
                     select t).ToList();
            return q;
        }

        public IList<TimeEntryModel> GetAll()
        {
            var timeEntries = HttpContext.Current.Session["TimeEntries"] as IList<TimeEntryModel>;

            if (timeEntries == null || UpdateDatabase)
            {

                timeEntries = (from t in entities.TimeEntries
                               join u in entities.AspNetUsers on t.UserID equals u.Id
                               where t.IsDeleted == false
                               select new TimeEntryModel
                               {
                                   TimeEntryID = t.TimeEntryID,
                                   UserID = t.UserID,
                                   EntryDate = t.EntryDate,
                                   StartTime = t.StartTime,
                                   EndTime = t.EndTime,
                                   IsLocked = t.IsLocked,
                                   IsDeleted = t.IsDeleted,
                                   PayrollDate = t.PayrollDate,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   MNPSEmployeeNo = u.MNPSEmployeeNo,
                                   Volunteer = u.Volunteer,
                                   FullName = u.LastName + ", " + u.FirstName
                               }).ToList();
                HttpContext.Current.Session["TimeEntries"] = timeEntries;
            }
            return timeEntries;

        }

        public IList<TimeEntryModel> GetUserEntries(string userID)
        {
            var timeEntries = HttpContext.Current.Session["TimeEntries"] as IList<TimeEntryModel>;
            if (timeEntries == null || UpdateDatabase)
            {
                timeEntries = entities.TimeEntries.Select(t => new TimeEntryModel
                {
                    TimeEntryID = t.TimeEntryID,
                    UserID = t.UserID,
                    EntryDate = t.EntryDate,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    IsLocked = t.IsLocked,
                    IsDeleted = t.IsDeleted,
                    PayrollDate = t.PayrollDate
                }).Where(s => s.UserID == userID && s.IsDeleted == false).ToList();

                HttpContext.Current.Session["TimeEntries"] = timeEntries;
            }

            return timeEntries;
        }

        public IEnumerable<TimeEntryModel> Read()
        {
            return GetAll();
        }

        public IEnumerable<TimeEntryModel> Read(string userID)
        {
            return GetUserEntries(userID);
        }

        public void Update(TimeEntryModel timeEntry)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.TimeEntryID == timeEntry.TimeEntryID);

                if (target != null)
                {
                    target.TimeEntryID = timeEntry.TimeEntryID;
                    target.UserID = timeEntry.UserID;
                    target.EntryDate = timeEntry.EntryDate;
                    target.StartTime = timeEntry.StartTime;
                    if (timeEntry.StartTime != null && timeEntry.EndTime != null) { 
                        target.EndTime = (timeEntry.StartTime).Value.Date.Add(timeEntry.EndTime.Value.TimeOfDay);
                    } else {
                        target.EndTime = timeEntry.EndTime;
                    }

                    target.IsLocked = timeEntry.IsLocked;
                    target.IsDeleted = timeEntry.IsDeleted;
                    target.PayrollDate = timeEntry.PayrollDate;
                }
            }
            else
            {
                var entity = entities.TimeEntries.Find(timeEntry.TimeEntryID);

                entity.TimeEntryID = timeEntry.TimeEntryID;
                entity.UserID = timeEntry.UserID;
                entity.EntryDate = timeEntry.EntryDate;
                entity.StartTime = timeEntry.StartTime;
                entity.EndTime = GetEndTime(timeEntry.EndTime, timeEntry.StartTime, timeEntry.UserID);
                entity.IsLocked = timeEntry.IsLocked;
                entity.IsDeleted = timeEntry.IsDeleted;
                entity.PayrollDate = timeEntry.PayrollDate;

                entities.SaveChanges();
            }
        }
        public DateTime? GetEndTime(DateTime? endTime, DateTime? startTime, string userId)
        {
            var teacherService = new TeacherService(new HomeworkHotlineEntities());

            var teacher = teacherService.GetById(userId);
            DateTime? _endTime = endTime;

            if (startTime != null && endTime != null) _endTime = (startTime).Value.Date.Add(endTime.Value.TimeOfDay);

            if (teacher.MNPSEmployeeNo != null)
               _endTime = startTime + GetDuration(_endTime, startTime,teacher.MNPSEmployeeNo != null);

            return _endTime;
        }

        public TimeSpan GetDuration(DateTime? endTime, DateTime? startTime, bool isMNPSEmployee)
        {
            TimeSpan duration = (endTime - startTime).Value;
            if (isMNPSEmployee)
            {
                if (duration.TotalMinutes < 15) duration = new TimeSpan(0, 0, 15, 0, 0);
                return duration.RoundToNearestMinutes(15);
            }
            return duration;
        }

        public void Destroy(TimeEntryModel timeEntry)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.TimeEntryID == timeEntry.TimeEntryID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.TimeEntries
                              where s.TimeEntryID == timeEntry.TimeEntryID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;

                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }

            }
        }

        public TimeEntryModel One(Func<TimeEntryModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
            
        public void Dispose()
        {
            entities.Dispose();
        }

    }
    public static class TimeSpanExtensions
    {
        public static TimeSpan RoundToNearestMinutes(this TimeSpan input, int minutes)
        {
            var totalMinutes = (int)(input + new TimeSpan(0, minutes / 2, 0)).TotalMinutes;

            return new TimeSpan(0, totalMinutes - totalMinutes % minutes, 0);
        }
    }
}
