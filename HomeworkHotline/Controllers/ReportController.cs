using System.Linq;
using System.Web.Mvc;

using Microsoft.Reporting.WebForms;
using System.Data;
using System.Web.UI.WebControls;
using HomeworkHotline.Models;
using Repository;
using System.Collections.Generic;

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

        public ActionResult OverallReport()
        {
            GetCountiesDropdownData();

            var parameters = new OverallReportParametersViewModel();

            return View(parameters);
        }

        [HttpPost]
        public ActionResult OverallReport(OverallReportParametersViewModel parameters)
        {
            if (ModelState.IsValid)
            {
                var reportData = _reportService.GetReportData(parameters.StartDate.Value, parameters.EndDate.Value, parameters.Counties, parameters.AggregationType == OverallReportAggregationType.Aggregate);

                string reportTemplatePath = ControllerContext.HttpContext.Server.MapPath("~/Documents/Report_Template.docx");
                var reportZipStream = _reportService.GetReportZip(reportData, reportTemplatePath);
                reportZipStream.Seek(0, System.IO.SeekOrigin.Begin);
                return File(reportZipStream, "application/zip", "Reports.zip");
            }
            else
            {
                GetCountiesDropdownData();
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