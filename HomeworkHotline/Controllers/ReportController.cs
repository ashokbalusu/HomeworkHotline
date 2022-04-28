using System.Linq;
using System.Web.Mvc;

using Microsoft.Reporting.WebForms;
using System.Data;
using System.Web.UI.WebControls;
using HomeworkHotline.Models;
using Repository;

namespace HomeworkHotline.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportController : Controller
    {
        private readonly HomeworkHotlineEntities _homeworkHotlineEntities;
        private readonly ReportService _reportService;

        public ReportController()
        {
            _homeworkHotlineEntities = new HomeworkHotlineEntities();
            _reportService = new ReportService(new HomeworkHotlineEntities());
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        private void GetCountiesDropdownData()
        {
            ViewData["Counties"] = _homeworkHotlineEntities
                .Counties
                .Select(c => new CountyModel
                {
                    CountyID = c.CountyID,
                    CountyName = c.CountyName,
                    StateCode = c.StateCode
                })
                .OrderBy(c => c.CountyName);
        }

        private void GetGradesDropdownData()
        {
            ViewData["Grades"] = _homeworkHotlineEntities
                .GradeLevels
                .Select(c => new GradeModel
                {
                    GradeID = c.Grade,
                    GradeText = c.GradeText,
                    GradeLevel = c.GradeLevel1
                })
                .OrderByDescending(c => c.GradeID);
        }


        private void GetSubjectsDropdownData()
        {
            ViewData["Subjects"] = _homeworkHotlineEntities
                .SubjectClassifications
                .Select(c => new SubjectClassificationModel
                {
                    SubjectID = c.SubjectGroup,
                    SubjectName = c.SubjectGroup
                }).Distinct()
                .OrderBy(c => c.SubjectName);
        }


        public ActionResult OverallReport()
        {
            GetCountiesDropdownData();

            GetGradesDropdownData();

            GetSubjectsDropdownData();

            var parameters = new OverallReportParametersViewModel();

            return View(parameters);
        }

        [HttpPost]
        public ActionResult OverallReport(OverallReportParametersViewModel parameters)
        {
            const string mimeType = "application/zip";
            const string fileName = "Reports.zip";

            if (parameters.StartDate != null && parameters.EndDate != null && parameters.StartDate > parameters.EndDate)
            {
                ModelState.AddModelError("StartDate", "Must be before to date");
                ModelState.AddModelError("EndDate", "Must be after from date");
            }

            if (ModelState.IsValid)
            {
                var reportData = _reportService.GetReportData(parameters.StartDate.Value, parameters.EndDate.Value, parameters.Counties, parameters.Grades, parameters.Subjects);

                string reportTemplatePath = ControllerContext.HttpContext.Server.MapPath("~/Documents/Report_Template.docx");
                var reportZipStream = _reportService.GetReportZip(reportData, reportTemplatePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(reportZipStream, mimeType, fileName);
            }
            else
            {
                GetCountiesDropdownData();
                GetGradesDropdownData();
                GetSubjectsDropdownData();
                return View(parameters);
            }
        }

        //    MyDataSet ds = new MyDataSet();
        public ActionResult ShowReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(900);
            reportViewer.Height = Unit.Percentage(900);

            //      var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


            //   SqlConnection conx = new SqlConnection(connectionString); SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", conx);

            //     adp.Fill(ds, ds.Employee_tbt.TableName);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Donors.rdl";
            //  reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MyDataSet", ds.Tables[0]));


            ViewBag.ReportViewer = reportViewer;

            return View();
        }
    }
}