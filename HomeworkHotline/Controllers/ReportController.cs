﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Repository;

namespace HomeworkHotline.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportController : Controller
    {
        private HomeworkHotlineEntities context = new HomeworkHotlineEntities();
        private ReportService reportService;

        public ReportController()
        {
            reportService = new ReportService(context);
        }

        // GET: Report
        public ActionResult Index()
        {
            var countyIdList = new List<int>();
            countyIdList.Add(10);
            countyIdList.Add(96);
            var reportData = reportService.GetReportData("01/01/2020", "12/31/2020", countyIdList);
            var docxStream = reportService.GetReportZip(reportData);
            return View();
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