using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CallLogModel
    {
        public int CallID { get; set; }
        [Required(ErrorMessage = "*Spoken language is required")]
        public int LanguageSpokenID { get; set; }
        public string LanguageSpokenOther { get; set; }
        [DataType(DataType.Time)]
        public Nullable<DateTime> CallStart { get; set; }
        [DataType(DataType.Time)]
        public Nullable<DateTime> CallEnd { get; set; }
        public bool ParentParticipation { get; set; }
        public bool CallDropped { get; set; }
        public bool CallTransferred { get; set; }
        public bool PostTestGiven { get; set; }
        [Required(ErrorMessage ="*Subject is required")]
        public Nullable<int> SubjectID { get; set; }
        [Required(ErrorMessage = "*Skill Assessed is required")]
        public string SkillAssessedNotes { get; set; }
        public string SessionEvalNotes { get; set; }
        [Required(ErrorMessage = "*Understanding at session start is required")]
        public Nullable<int> EvalStartID { get; set; }
        [Required(ErrorMessage = "*Understanding at session end is requried")]
        public Nullable<int> EvalEndID { get; set; }
        public int StudentID { get; set; }
        public string UserID { get; set; }
        public bool Textbook { get; set; }
        public bool Worksheet { get; set; }
        public bool Stoodle { get; set; }
        public bool ImageShare { get; set; }
        public bool Chat { get; set; }
        public bool GoogleDocs { get; set; }
        public DateTime? PrizeGiven { get; set; }
        public Nullable<int> PrizeID { get; set; }
        public bool IsLocked { get; set; }
        public bool BeatMath { get; set; }
        public string PrizeStudentName { get; set; }
        public string PrizeTeacherName { get; set; }
        public bool IsDeleted { get; set; }

        public string CodeName { get; set; }
        public string LanguageName { get; set; }
        public string SubjectName { get; set; }
        public string EvalStartDescription { get; set; }
        public string EvalEndDescription { get; set; }
        public string PrizeName { get; set; }
        public string TeacherName { get; set; }
        public double Duration
        {
            get
            {
                if (CallStart.HasValue && CallEnd.HasValue)
                {
                    return Math.Round((CallEnd.Value.Subtract(CallStart.Value)).TotalHours, 2);
                }
                return 0.00;
            }
            set { }
        }
        public virtual Student Student { get; set; }

        public string StudentSchool { get; set; }
        public string StudentSchoolCounty { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentGrade { get; set; }
        public string StudentHasInternet { get; set; }
        public int StudentTotalCalls { get; set; }
        public bool StudentNeedsPrize {
            get
            {
                bool needPrize = false;
                if (PrizeGiven == null && PrizeID != null && PrizeStudentName != null) needPrize = true;
                return needPrize;
            }
            set { }
        }

        public static implicit operator CallLogModel(CallLog callLog)
        {
            return new CallLogModel
            {
                CallID = callLog.CallID,
                StudentID = callLog.StudentID,
                UserID = callLog.UserID,
                LanguageSpokenID = callLog.LanguageSpokenID == 0 ? 1 : callLog.LanguageSpokenID,
                LanguageSpokenOther = callLog.LanguageSpokenOther,
                CallStart = callLog.CallStart,
                CallEnd = callLog.CallEnd,
                ParentParticipation = callLog.ParentParticipation,
                CallDropped = callLog.CallDropped,
                CallTransferred = callLog.CallTransferred,
                PostTestGiven = callLog.PostTestGiven,
                SubjectID = callLog.SubjectID,
                SkillAssessedNotes = callLog.SkillAssessedNotes,
                SessionEvalNotes = callLog.SessionEvalNotes,
                EvalStartID = callLog.EvalStartID,
                EvalEndID = callLog.EvalEndID,
                Textbook = callLog.Textbook,
                Worksheet = callLog.Worksheet,
                Stoodle = callLog.Stoodle,
                ImageShare = callLog.ImageShare,
                Chat = callLog.Chat,
                GoogleDocs = callLog.GoogleDocs,
                PrizeGiven = callLog.PrizeGiven,
                PrizeID = callLog.PrizeID,
                IsLocked = callLog.IsLocked,
                BeatMath = callLog.BeatMath,
                PrizeStudentName = callLog.PrizeStudentName,
                PrizeTeacherName = callLog.PrizeTeacherName,
                Student = callLog.Student
            };
        }

        public static implicit operator CallLog(CallLogModel callLogModel)
        {
            return new CallLog
            {
            CallID = callLogModel.CallID,
            StudentID = callLogModel.StudentID,
            UserID = callLogModel.UserID,
            LanguageSpokenID = callLogModel.LanguageSpokenID == 0 ? 1 : callLogModel.LanguageSpokenID,
            LanguageSpokenOther = callLogModel.LanguageSpokenOther,
            CallStart = callLogModel.CallStart,
            CallEnd = callLogModel.CallEnd,
            ParentParticipation = callLogModel.ParentParticipation,
            CallDropped = callLogModel.CallDropped,
            CallTransferred = callLogModel.CallTransferred,
            PostTestGiven = callLogModel.PostTestGiven,
            SubjectID = callLogModel.SubjectID,
            SkillAssessedNotes = callLogModel.SkillAssessedNotes,
            SessionEvalNotes = callLogModel.SessionEvalNotes,
            EvalStartID = callLogModel.EvalStartID,
            EvalEndID = callLogModel.EvalEndID,
            Textbook = callLogModel.Textbook,
            Worksheet = callLogModel.Worksheet,
            Stoodle = callLogModel.Stoodle,
            ImageShare = callLogModel.ImageShare,
            Chat = callLogModel.Chat,
            GoogleDocs = callLogModel.GoogleDocs,
            PrizeGiven = callLogModel.PrizeGiven,
            PrizeID = callLogModel.PrizeID,
            IsLocked = callLogModel.IsLocked,
                BeatMath = callLogModel.BeatMath,
                PrizeStudentName = callLogModel.PrizeStudentName,
                PrizeTeacherName = callLogModel.PrizeTeacherName,
                Student = callLogModel.Student
            };
        }

    }
}
