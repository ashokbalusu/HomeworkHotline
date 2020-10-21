using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;

namespace Repository
{
    public class CallLogService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public CallLogService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<CallLogModel> GetAll()
        {
            var result = HttpContext.Current.Session["CallLogs"] as IList<CallLogModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.CallLogs.Select(callLog => new CallLogModel
                {
                    CallID = callLog.CallID,
                    StudentID = callLog.StudentID,
                    CodeName = callLog.Student.CodeName,
                    StudentFirstName = callLog.Student.StudentFirstName,
                    StudentLastName = callLog.Student.StudentLastName,
                    UserID = callLog.UserID,
                    TeacherName = callLog.AspNetUser.LastName +", "+ callLog.AspNetUser.FirstName,
                    LanguageSpokenID = callLog.LanguageSpokenID,
                    LanguageName = callLog.LanguageType.LanguageName,
                    LanguageSpokenOther = callLog.LanguageSpokenOther,
                    CallStart = callLog.CallStart,
                    CallEnd = callLog.CallEnd,
                    ParentParticipation = callLog.ParentParticipation,
                    CallDropped = callLog.CallDropped,
                    CallTransferred = callLog.CallTransferred,
                    PostTestGiven = callLog.PostTestGiven,
                    SubjectID = callLog.SubjectID,
                    SubjectName = callLog.Subject.SubjectType.SubjectTypeName + "-" + callLog.Subject.SubjectName,
                    SkillAssessedNotes = callLog.SkillAssessedNotes,
                    SessionEvalNotes = callLog.SessionEvalNotes,
                    EvalStartID = callLog.EvalStartID,
                    EvalStartDescription = callLog.EvalStart.EvalStartDescription,
                    EvalEndID = callLog.EvalEndID,
                    EvalEndDescription = callLog.EvalEnd.EvalEndDescription,
                    Textbook = callLog.Textbook,
                    Worksheet = callLog.Worksheet,
                    Stoodle = callLog.Stoodle,
                    ImageShare = callLog.ImageShare,
                    Chat = callLog.Chat,
                    GoogleDocs = callLog.GoogleDocs,
                    PrizeGiven = callLog.PrizeGiven,
                    PrizeID = callLog.PrizeID,
                    PrizeName = callLog.Prize.PrizeName,
                    IsLocked = callLog.IsLocked,
                    BeatMath = callLog.BeatMath,
                    StudentSchool = callLog.Student.School.SchoolName,
                    StudentSchoolCounty = callLog.Student.School.County.CountyName,
                    StudentGrade = callLog.Student.Grade,
                    StudentHasInternet = callLog.Student.HasInternet == false ? "No" : "Yes",
                    PrizeStudentName = callLog.PrizeStudentName,
                    PrizeTeacherName = callLog.PrizeTeacherName,
                    IsDeleted = callLog.IsDeleted,
                    StudentTotalCalls = (from c in entities.CallLogs where c.StudentID == callLog.StudentID select c).Count()
                 }).Where(e => e.IsDeleted == false)
                .ToList();


                HttpContext.Current.Session["CallLogs"] = result;
            }

            return result;
        }

        public IEnumerable<CallLogModel> Read()
        {
            return GetAll();
        }

        public void Create(CallLogModel callLog)
        {
            if (!UpdateDatabase)
            {
                var first = GetAll().OrderByDescending(e => e.StudentID).FirstOrDefault();
                var id = (first != null) ? first.StudentID : 0;

                GetAll().Insert(0, callLog);
            }
            else
            {
                var entity = new CallLog();

                entity.CallID = callLog.CallID;
                entity.StudentID = callLog.StudentID;
                entity.UserID = callLog.UserID;
                entity.LanguageSpokenID = callLog.LanguageSpokenID == 0 ? 1 : callLog.LanguageSpokenID;
                entity.LanguageSpokenOther = callLog.LanguageSpokenOther;
                entity.CallStart = callLog.CallStart;
                entity.CallEnd = callLog.CallEnd;
                entity.ParentParticipation = callLog.ParentParticipation;
                entity.CallDropped = callLog.CallDropped;
                entity.CallTransferred = callLog.CallTransferred;
                entity.PostTestGiven = callLog.PostTestGiven;
                entity.SubjectID = callLog.SubjectID;
                entity.SkillAssessedNotes = callLog.SkillAssessedNotes;
                entity.SessionEvalNotes = callLog.SessionEvalNotes;
                entity.EvalStartID = callLog.EvalStartID;
                entity.EvalEndID = callLog.EvalEndID;
                entity.Textbook = callLog.Textbook;
                entity.Worksheet = callLog.Worksheet;
                entity.Stoodle = callLog.Stoodle;
                entity.ImageShare = callLog.ImageShare;
                entity.Chat = callLog.Chat;
                entity.GoogleDocs = callLog.GoogleDocs;
                entity.PrizeGiven = callLog.PrizeGiven;
                entity.PrizeID = callLog.PrizeID;
                entity.IsLocked = callLog.IsLocked;
                entity.BeatMath = callLog.BeatMath;
                entity.PrizeStudentName = callLog.PrizeStudentName;
                entity.PrizeTeacherName = callLog.PrizeTeacherName;

                entities.CallLogs.Add(entity);
                entities.SaveChanges();

                callLog.CallID = entity.CallID;
            }
        }

        public void Update(CallLogModel callLog)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.CallID == callLog.CallID);

                if (target != null)
                {
                    target.CallID = callLog.CallID;
                    target.StudentID = callLog.StudentID;
                    target.UserID = callLog.UserID;
                    target.LanguageSpokenID = callLog.LanguageSpokenID == 0 ? 1 : callLog.LanguageSpokenID;
                    target.LanguageSpokenOther = callLog.LanguageSpokenOther;
                    target.CallStart = callLog.CallStart;
                    target.CallEnd = GetEndTime(callLog.CallEnd, callLog.CallStart);
                    target.ParentParticipation = callLog.ParentParticipation;
                    target.CallDropped = callLog.CallDropped;
                    target.CallTransferred = callLog.CallTransferred;
                    target.PostTestGiven = callLog.PostTestGiven;
                    target.SubjectID = callLog.SubjectID;
                    target.SkillAssessedNotes = callLog.SkillAssessedNotes;
                    target.SessionEvalNotes = callLog.SessionEvalNotes;
                    target.EvalStartID = callLog.EvalStartID;
                    target.EvalEndID = callLog.EvalEndID;
                    target.Textbook = callLog.Textbook;
                    target.Worksheet = callLog.Worksheet;
                    target.Stoodle = callLog.Stoodle;
                    target.ImageShare = callLog.ImageShare;
                    target.Chat = callLog.Chat;
                    target.GoogleDocs = callLog.GoogleDocs;
                    target.PrizeGiven = callLog.PrizeGiven;
                    target.PrizeID = callLog.PrizeID;
                    target.IsLocked = callLog.IsLocked;
                    target.BeatMath = callLog.BeatMath;
                    target.PrizeStudentName = callLog.PrizeStudentName;
                    target.PrizeTeacherName = callLog.PrizeTeacherName;
                }
            }
            else
            {
                var entity = new CallLog();

                entity.CallID = callLog.CallID;
                entity.StudentID = callLog.StudentID;
                entity.UserID = callLog.UserID;
                entity.LanguageSpokenID = callLog.LanguageSpokenID == 0 ? 1 : callLog.LanguageSpokenID;
                entity.LanguageSpokenOther = callLog.LanguageSpokenOther;
                entity.CallStart = callLog.CallStart;
                entity.CallEnd = GetEndTime(callLog.CallEnd,callLog.CallStart);
                entity.ParentParticipation = callLog.ParentParticipation;
                entity.CallDropped = callLog.CallDropped;
                entity.CallTransferred = callLog.CallTransferred;
                entity.PostTestGiven = callLog.PostTestGiven;
                entity.SubjectID = callLog.SubjectID;
                entity.SkillAssessedNotes = callLog.SkillAssessedNotes;
                entity.SessionEvalNotes = callLog.SessionEvalNotes;
                entity.EvalStartID = callLog.EvalStartID;
                entity.EvalEndID = callLog.EvalEndID;
                entity.Textbook = callLog.Textbook;
                entity.Worksheet = callLog.Worksheet;
                entity.Stoodle = callLog.Stoodle;
                entity.ImageShare = callLog.ImageShare;
                entity.Chat = callLog.Chat;
                entity.GoogleDocs = callLog.GoogleDocs;
                entity.PrizeGiven = callLog.PrizeGiven;
                entity.PrizeID = callLog.PrizeID;
                entity.IsLocked = callLog.IsLocked;
                entity.BeatMath = callLog.BeatMath;
                entity.PrizeStudentName = callLog.PrizeStudentName;
                entity.PrizeTeacherName = callLog.PrizeTeacherName;


                entities.CallLogs.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Destroy(CallLogModel callLog)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.CallID == callLog.CallID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.CallLogs
                              where s.CallID == callLog.CallID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;
                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public CallLogModel One(Func<CallLogModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }

        public DateTime? GetEndTime(DateTime? endTime, DateTime? startTime)
        {
            if (startTime != null && endTime != null) endTime = (startTime).Value.Date.Add(endTime.Value.TimeOfDay);

            return endTime;
        }
    }
}
