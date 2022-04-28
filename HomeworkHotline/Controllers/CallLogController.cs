using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace HomeworkHotline.Controllers
{
    [Authorize]
    public class CallLogController : Controller
    {
        private CallLogService callLogService;
        public CallLogController()
        {
            callLogService = new CallLogService(new HomeworkHotlineEntities());
        }


        private HomeworkHotlineEntities context = new HomeworkHotlineEntities();

        // GET: CallLog
        public ActionResult Index()
        {
            if (!isEdit)
            {
                ViewBag.StudentModel = TempData["studentModel"] as Repository.StudentModel;
                TempData.Keep("studentModel");

                CallLogModel clm = new CallLogModel();
                if (ViewBag.StudentModel != null)
                {
                    //   callLogModel.CallID = 0;
                    clm.StudentID = ViewBag.StudentModel.StudentID;
                    clm.LanguageSpokenID = ViewBag.StudentModel.HomeLanguageID != null ? ViewBag.StudentModel.HomeLanguageID : 0;
                    clm = CreateCallLog(clm);
                    clm.PrizeStudentName = ViewBag.StudentModel.StudentFirstName;
                }
                else
                    return RedirectToAction("Index", "Timesheet");//Page was navigated to without selecting/creating a student - send back to timesheets
                GetDropDownData();
                ViewBag.IsEdit = false;
                return View(clm);
            }

            //    GetDropDownData();

            return View();
        }

        public ActionResult AdminDashboard(DateTime? startDate = null)
        {
            // TODO: get dates from date/time pickers
            DateTime varForStartAndEndDate = startDate ?? DateTime.Today;
            DateTime endDate = varForStartAndEndDate;
            DateTime selectedStart = varForStartAndEndDate.AddDays(-30);

            GetAllStudents(selectedStart, endDate);
            GetBeatMathCalls(selectedStart, endDate);
            GetCallsInForeignLanguage(selectedStart, endDate);
            GetCountyCountCalls(selectedStart, endDate);
            GetFeaturesUsed(selectedStart, endDate);
            GetNumberOfCallsPerSubject(selectedStart, endDate);
            GetAdminWeeklySummary(selectedStart, endDate);
            AdminWeeklySummary summary = CallLogs.GetAdminWeeklySummary(selectedStart, endDate);
            var sessions = CallLogs.GetSessionsByGrade(selectedStart, endDate).ToList();
            if (sessions.Any())
            {
                var k = sessions.SingleOrDefault(s => s.Grade.ToLower() == "k");
                if (k != null)
                {
                    sessions.Remove(k);
                }
                sessions = sessions.OrderBy(s => int.Parse(s.Grade)).ToList();
                sessions.Insert(0, k);
            }
            return View(sessions);
        }

        private void GetAllStudents(DateTime selectedStart, DateTime endDate)
        {
     
            var AllStudentsdeName = CallLogs.GetAllStudents(selectedStart, endDate);
            ViewBag.AllStudentsdeName = AllStudentsdeName.NumOFStudents;
        }

        private void GetBeatMathCalls(DateTime selectedStart, DateTime endDate)
        {
            var BeatMath = CallLogs.GetBeatMath(selectedStart, endDate);
            ViewBag.BeatMath = BeatMath.NumBeatMathSessions;
        }

        private void GetCallsInForeignLanguage(DateTime selectedStart, DateTime endDate)
        {
            var CallsInForeignLanguage = CallLogs.GetCallsInForeignLanguage(selectedStart, endDate);
            ViewBag.ForeignLanguage = CallsInForeignLanguage.NumForeignLanguageSpoken;
        }


        private void GetCountyCountCalls(DateTime selectedStart, DateTime endDate)
        {

            var CountyCount = CallLogs.GetCountyCount(selectedStart, endDate);

            string CountyCall = "";

            foreach (var call in CountyCount)
            {

                {
                    var callCount = call.CallCount;
                    var countyName = call.CountyName;
                    if (callCount > 5)
                    {
                        CountyCall += ",['" + countyName.Replace("County", "") + "'," + callCount + "]";
                    }
                }

            }
            ViewBag.CountyCall = new HtmlString(CountyCall);
        }

        private void GetFeaturesUsed(DateTime selectedStart, DateTime endDate)
        {

  

            var FeaturesUsed = CallLogs.GetFeaturesUsed(selectedStart, endDate);

            string Features = "";

            foreach (var call in FeaturesUsed)
            {

                {
                    var features = call.Features;
                    var numResult = call.NumResult;
                    Features += ",['" + features + "'," + numResult + "]";
                }

            }
            ViewBag.FeaturesUsed = new HtmlString(Features);
        }

        private void GetNumberOfCallsPerSubject(DateTime selectedStart, DateTime endDate)
        {

     
            var CallsPerSubject = CallLogs.GetNumberOfCallsPerSubject(selectedStart, endDate);

            string Subject = "";

            foreach (var call in CallsPerSubject)
            {

                {
                    var Call = call.CallCount;
                    var Subjectname = call.SubjectTypeName;
                    Subject += ",['" + Subjectname + "'," + Call + "]";
                }

            }
            ViewBag.CallPerSubject = new HtmlString(Subject);
        }


        private void GetAdminWeeklySummary(DateTime selectedStart, DateTime endDate)
        {
            AdminWeeklySummary summary = CallLogs.GetAdminWeeklySummary(selectedStart, endDate);
            ViewBag.TotalMinutes = summary.TotalHours ?? 0;
            ViewBag.TotalCalls = summary.TotalCalls;

            IList<EvalEndCounts> endEvals = CallLogs.GetEvalEndCounts(selectedStart, endDate).ToList();

            int totalEndEvals = endEvals.Count();
            int completedAssignment = (from e2 in endEvals where e2.EvalEndID == 2 select e2.CallCount).FirstOrDefault();
            int masteredMaterial = (from e2 in endEvals where e2.EvalEndID == 3 select e2.CallCount).FirstOrDefault();

            if (totalEndEvals > 0)
            {
                ViewBag.CompletedAssignment = Math.Round(((Convert.ToDecimal(completedAssignment) / Convert.ToDecimal(summary.TotalCalls)) * 100), 1);
                ViewBag.MasteredMaterial = Math.Round(((Convert.ToDecimal(masteredMaterial) / Convert.ToDecimal(summary.TotalCalls)) * 100), 1);
                var test = (completedAssignment / summary.TotalCalls) * 100;
            }
            else
            {
                ViewBag.CompletedAssignment = 0;
                ViewBag.MasteredMaterial = 0;
            }

            string weekdayTotalCalls = "";
            string weekdayTotalTime = "";

            DateTime reference = selectedStart;

            int totalDailyCalls;
            decimal totalDailyTime;
            AdminWeeklySummary callLogSummary;
            for (int i = 0; i < 28; i++)
            {
                if (reference.DayOfWeek != DayOfWeek.Saturday && reference.DayOfWeek != DayOfWeek.Sunday)
                {
                    callLogSummary = CallLogs.GetAdminWeeklySummary(reference, reference.AddDays(1));
                    totalDailyCalls = callLogSummary.TotalCalls;
                    totalDailyTime = (callLogSummary.TotalHours == null) ? 0 : (decimal)callLogSummary.TotalHours;
                    weekdayTotalCalls += ",['" + reference.ToString("MM/dd") + "'," + totalDailyCalls + "]";
                    weekdayTotalTime += ",['" + reference.ToString("MM/dd") + "'," + totalDailyTime + "]";
                }
                reference = reference.AddDays(1);
            }
            ViewBag.DailyTotalCalls = new HtmlString(weekdayTotalCalls);
            ViewBag.DailyTotalHours = new HtmlString(weekdayTotalTime);
        }

        public ActionResult List()
        {
            GetDropDownData();
            ViewBag.TotalCount = callLogService.Read().Count();
            ViewBag.IsAdmin = User.IsInRole("Administrator");

            return View();
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CallLogModel callLog = await context.CallLogs.FindAsync(id);
            if (callLog == null)
            {
                return HttpNotFound();
            }
            isEdit = true;

            Student student = await context.Students.FindAsync(callLog.StudentID);
            if (student != null)
            {
                StudentController sc = new StudentController();
                ViewBag.StudentModel = sc.MapStudentToModel(student);
            }

            ViewBag.IsEdit = isEdit;

            GetDropDownData();
            return View("Index", callLog);
        }

        private bool isEdit = false;

        public ActionResult DeleteCall(Int32 callID)
        {
            CallLog call = context.CallLogs.Where(m => m.CallID == callID).FirstOrDefault();
            context.CallLogs.Remove(call);
            context.SaveChanges();

            return RedirectToAction("Index", "Student");
        }

        public void GetDropDownData()
        {
            ViewData["languageTypes"] = new HomeworkHotlineEntities()
            .LanguageTypes
            .Select(e => new LanguageTypeModel
            {
                LanguageID = e.LanguageID,
                LanguageName = e.LanguageName
            })
            .OrderBy(e => e.LanguageID);

            ViewData["subjects"] = new HomeworkHotlineEntities()
            .Subjects
            .Select(e => new SubjectModel
            {
                SubjectID = e.SubjectID,
                SubjectName =  e.SubjectType.SubjectTypeName + " - " + e.SubjectName
            })
            .OrderBy(e => e.SubjectName);

            ViewData["prizes"] = new HomeworkHotlineEntities()
            .Prizes
            .Select(e => new PrizeModel
            {
                PrizeID = e.PrizeID,
                PrizeName = e.PrizeName
            })
            .OrderBy(e => e.PrizeID);

            ViewData["evalStartItems"] = new HomeworkHotlineEntities()
            .EvalStarts
            .Select(e => new EvalStartModel
            {
                EvalStartID = e.EvalStartID,
                EvalStartDescription = e.EvalStartDescription
            })
            .OrderBy(e => e.EvalStartID);

            ViewData["evalEndItems"] = new HomeworkHotlineEntities()
            .EvalEnds
            .Select(e => new EvalEndModel
            {
                EvalEndID = e.EvalEndID,
                EvalEndDescription = e.EvalEndDescription
            })
            .OrderBy(e => e.EvalEndID);
        }

        private static string ReplaceFunc(string findStr)
        {
            if (_replacePatterns.ContainsKey(findStr))
            {
                return _replacePatterns[findStr];
            }
            return findStr;
        }

        private static Dictionary<string, string> _replacePatterns = new Dictionary<string, string>()
        { 
            { "DATE", "dateTest" },
            { "TEACHER_NAME", "clm.PrizeTeacherName" },
            { "SCHOOL_NAME", "schoolModel.SchoolName" },
            { "COUNTY", "schoolModel.CountyName" },
            { "ADDRESS", "schoolModel.Address1" + " " + "schoolModel.Address2" },
        };


        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/pdf", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }


        public ActionResult GeneratePrizeLetter()
        {
            using (var db = new HomeworkHotlineEntities())
            {
                List<CallLog> callLogs = db.CallLogs
                    .Where(m => m.PrizeGiven == null && m.PrizeID != null && m.PrizeStudentName != null)
                    .ToList();

                const int pageMargin = 50;
                var doc = new Document(PageSize.LETTER, pageMargin, pageMargin, pageMargin-30, pageMargin);
                var memoryStream = new MemoryStream();
                var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
                
                doc.Open();

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                Font font = new Font(baseFont, 10f);
                Font font_bold = new Font(baseFont, 10f, Font.BOLD);
                Font large_font_bold = new Font(baseFont, 12f, Font.BOLD);
                Font large_green_font_bold = new Font(baseFont, 12f, Font.BOLD, BaseColor.GREEN);


                int i = 0;
                foreach (CallLog call in callLogs)
                {
                    #region Letter Construction
                    School school = call.Student.School;

                    Paragraph header = new Paragraph();
                    header.Add(Chunk.NEWLINE);
                    header.Add(new Chunk(string.Format("{0}\n", DateTime.Today.ToLongDateString()), font));
                    header.Add(Chunk.NEWLINE);
                    header.Add(new Chunk(string.Format("{0}\n", call.PrizeTeacherName), font));
                    header.Add(new Chunk(string.Format("{0} -- {1}\n", school.SchoolName, school.County.CountyName), font));
                    header.Add(new Chunk(string.Format("{0}\n", school.Address1), font));
                    if (school.Address2 != null) header.Add(new Chunk(string.Format("{0}\n", school.Address2), font));
                    header.Add(new Chunk(string.Format("{0}, {1} {2}\n", school.City, school.State, school.Zip), font));

                    header.Add(Chunk.NEWLINE);
                    header.Add(new Chunk(string.Format("Dear {0} and Parent or Guardian,\n", call.PrizeStudentName), font));
                    header.Add(new Chunk("Thank you for calling Homework Hotline and working so hard!  We know homework can be difficult, but the best students are those who ask questions when they don't understand and who practice the skills and concepts they are learning.  You're doing both, and the teachers at Homework Hotline are proud of your efforts.  Keep up the great work and feel free to reach out to our certified teachers whenever you need help!  We hope you enjoy your prize!", font));
                    header.Add(Chunk.NEWLINE);

                    Paragraph letter = new Paragraph();
                    letter.Add(Chunk.NEWLINE);
                    letter.Add(new Chunk("Do your friends need help too?  Tell them to call ", font_bold));
                    letter.Add((new Chunk("615-298-6636", font_bold)).SetUnderline(1,-3));
                    letter.Add(new Chunk(" or chat with us through ", font_bold));
                    letter.Add((new Chunk("homeworkhotline.info", font_bold)).SetUnderline(1, -3));
                    letter.Add(new Chunk(" for help, and ", font_bold));
                    letter.Add((new Chunk("YOU", font_bold)).SetUnderline(1, -3));
                    letter.Add(new Chunk(" will receive an additional prize!", font_bold));
                    letter.Add(Chunk.NEWLINE);
                    letter.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                    Paragraph list = new Paragraph();
                    list.Add((new Chunk("Here's how:", font_bold)).SetUnderline(1, -3));
                    iTextSharp.text.List how_list = new iTextSharp.text.List(iTextSharp.text.List.ORDERED);
                    how_list.Add(new ListItem("Tell your friends to call or chat with Homework Hotline when they have homework questions or need tutoring.", font));
                    how_list.Add(new ListItem("Have them tell us your code name and that you told them to call us for help.",font));
                    how_list.Add(new ListItem("We will send you an extra prize the next time you call for tutoring.  Just tell us your friends called!",font));
                    list.Add(how_list);

                    list.Add(Chunk.NEWLINE);
                    list.Add(new Chunk("You should be proud of the work you are doing; we definitely are!", font));
                    list.Add(Chunk.NEWLINE);

                    Paragraph closing = new Paragraph();
                    closing.Add(Chunk.NEWLINE);
                    closing.Add(new Chunk(string.Format("{0}\n", "Sincerely yours,"), font));
                    Image signature = Image.GetInstance(Server.MapPath("~/Images/Signature.png"));
                    signature.ScalePercent(45);
                    closing.Add(signature);
                    closing.Add(new Chunk(string.Format("{0}\n", "Rebekah Vance"), font));
                    closing.Add(new Chunk(string.Format("{0}\n", "Executive Director"), font));
                    closing.Add(new Chunk(string.Format("{0}\n", "Homework Hotline"), font));
                    closing.Add(Chunk.NEWLINE);

                    Paragraph ps = new Paragraph();
                    ps.Add(new Chunk("P.S. If you enjoyed our services, we'd appreciate your review! Our teachers want to know how they're doing. To review us, Google: Homework Hotline. Then, on your phone you'll scroll and hit \"more about Homework Hotline.\"  You should see the stars to rate us.  Thank you!", font));
                    ps.Add(Chunk.NEWLINE);

                    Paragraph footer = new Paragraph();
                    footer.Add(Chunk.NEWLINE);
                    footer.Add(new Chunk("Homework Hotline is Tennessee’s source for FREE ", large_font_bold));
                    footer.Add(new Chunk("homework help", large_green_font_bold).SetUnderline(1, -3));
                    footer.Add(new Chunk(" and ", large_font_bold));
                    footer.Add(new Chunk("tutoring", large_green_font_bold).SetUnderline(1, -3));
                    footer.Add(new Chunk(" over the phone and online!", large_font_bold));
                    footer.Add(Chunk.NEWLINE);
                    footer.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                    PdfPTable codename_table = new PdfPTable(1);
                    codename_table.WidthPercentage = 100f;
                    codename_table.DefaultCell.Border = 0;
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    Paragraph contents = new Paragraph();
                    contents.Alignment = Element.ALIGN_RIGHT;
                    contents.Add(new Chunk(call.Student.CodeName, font));
                    cell.AddElement(contents);
                    codename_table.AddCell(cell);

                    Image header_logo = Image.GetInstance(Server.MapPath("~/Images/hhLogo.png"));
                    header_logo.ScaleToFit(260f, 280f);
                    header_logo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                    if (i > 0) doc.NewPage();
                    doc.Add(header_logo);
                    doc.Add(header);
                    doc.Add(letter);
                    doc.Add(list);
                    doc.Add(closing);
                    doc.Add(ps);
                    doc.Add(footer);
                    doc.Add(codename_table);

                    call.PrizeGiven = DateTime.Now;

                    i++;
                    #endregion
                }

                db.SaveChanges();

                pdfWriter.CloseStream = false;
                doc.Close();
                memoryStream.Position = 0;
                string handle = Guid.NewGuid().ToString();
                TempData[handle] = memoryStream.ToArray();
                string filename = "PrizeLetters" + (DateTime.Now.ToShortDateString()).Replace("/","") +".pdf";
                return new JsonResult()
                {
                    Data = new { FileGuid = handle, FileName = filename }
                };

                ////          Dictionary<string, string> _replacePatterns = new Dictionary<string, string>()
                ////            { 
                ////// TODO: format CallDate

                ////      { "DATE", date },
                ////      { "TEACHER_NAME", clm.PrizeTeacherName },
                ////      { "SCHOOL_NAME", schoolModel.SchoolName },
                ////      { "COUNTY", schoolModel.CountyName },
                ////      { "ADDRESS", schoolModel.Address1 + " " + schoolModel.Address2 },
                ////      { "CITY", schoolModel.City },
                ////      { "STATE", schoolModel.State },
                ////      { "ZIP", schoolModel.Zip },
                ////      { "STUDENT_NAME", clm.PrizeStudentName },
                ////      { "CODE_NAME", student.CodeName },
                ////            };

                //string fileName = "PrizeLetter_Template.docx";
                //string fileDownloadName = "PrizeLetter_Template.docx";
                //var filepath = System.IO.Path.Combine(Server.MapPath("/Documents/"), fileName);
                //var fileDownloadPath = System.IO.Path.Combine(Server.MapPath("/Documents/"), fileName);
                ////     string fileDownloadName = String.Format("PrizeLetter_{0}.docx", student.CodeName);
                ////     var fileDownloadPath = System.IO.Path.Combine(Server.MapPath("/Documents/"), fileDownloadName);

                //// replace text in docx with above values
                //using (DocX document = DocX.Load(filepath))
                //{
                //    // Check if all the replace patterns are used in the loaded document.
                //    if (document.FindUniqueByPattern(@"<[\w \=]{4,}>", RegexOptions.IgnoreCase).Count == _replacePatterns.Count)
                //    {
                //        // Do the replacement
                //        for (int i = 0; i < _replacePatterns.Count; ++i)
                //        {
                //            document.ReplaceText("<(.*?)>", ReplaceFunc, false, RegexOptions.IgnoreCase, null, new Formatting());
                //        }

                //        // TODO: not saving....
                //        // Save this document to disk.
                //        document.SaveAs(fileDownloadPath);
                //        //     Console.WriteLine("\tCreated: ReplacedText.docx\n");
                //    }
                //}
                //// end replace text

                ////return File(filepath, MimeMapping.GetMimeMapping(filepath), fileDownloadName);
                //return File(fileDownloadPath, MimeMapping.GetMimeMapping(fileDownloadPath), fileDownloadName);
            }
        }

        private CallLogModel CreateCallLog(CallLogModel clm)
        {
            CallLog callLog = new CallLog();
            callLog.StudentID = clm.StudentID;
            callLog.LanguageSpokenID = clm.LanguageSpokenID == 0 ? 1 : clm.LanguageSpokenID;
            callLog.CallStart = RoundToNearestMinute(DateTime.Now); 
            callLog.UserID = User.Identity.GetUserId();
            context.CallLogs.Add(callLog);

            context.SaveChanges();

            clm.CallID = callLog.CallID;
            clm.CallStart = callLog.CallStart;
            clm.UserID = callLog.UserID;
            return clm;
        }


        [HttpPost]
        public ActionResult Update(CallLogModel clm)
        {
            CallLog callLog = clm;
            if (!isEdit && clm.CallEnd == null)
            {
                DateTime endTime = RoundToNearestMinute(DateTime.Now);
                if (endTime == clm.CallStart) endTime = endTime.AddMinutes(1);
                callLog.CallEnd = endTime;
            }
            context.Entry(callLog).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("Index", "Timesheet");
        }

        private static DateTime RoundToNearestMinute(DateTime datetime)
        {
            int f = 0;
            double m = (double)(datetime.Ticks % TimeSpan.FromSeconds(60).Ticks) / TimeSpan.FromSeconds(60).Ticks;
            if (m >= 0.5)
                f = 1;
            return new DateTime(((datetime.Ticks / TimeSpan.FromSeconds(60).Ticks) + f) * TimeSpan.FromSeconds(60).Ticks);
        }


        public ActionResult CallLogs_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult = Json(callLogService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CallLogs_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<CallLogModel> callLogs)
        {
            if (callLogs != null && ModelState.IsValid)
            {
                foreach (var callLog in callLogs)
                {
                    callLogService.Update(callLog);
                }
            }

            return Json(callLogs.ToDataSourceResult(request, ModelState));
        }

        public ActionResult CallLogs_Delete([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")]IEnumerable<CallLogModel> callLogs)
        {
            if (callLogs != null)
            {
                var callLog = callLogs.First();
                callLogService.Destroy(callLog);

            }
            return Json(ModelState.ToDataSourceResult());
        }
    }
}