using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Repository;
using Microsoft.AspNet.Identity;

namespace HomeworkHotline.Controllers
{
    [Authorize]
    public class TimesheetController : Controller
    {
        private static HomeworkHotlineEntities context = new HomeworkHotlineEntities();
        TimeEntryService timeEntryService = new TimeEntryService(context);

        

        // GET: Timesheet
        public ActionResult Index()
        {
            ViewBag.TotalCount = timeEntryService.Read().Count();
            ViewBag.IsAdmin = !User.IsInRole("Administrator");
            return View();
        }

        public ActionResult EditingInline_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult;
            if (User.IsInRole("Administrator"))
                jsonResult = Json(timeEntryService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            else
                jsonResult = Json(timeEntryService.Read(User.Identity.GetUserId()).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, TimeEntryModel timeEntry)
        {
            if (timeEntry != null && ModelState.IsValid)
            {
                timeEntryService.Update(timeEntry);
            }

            return Json(new[] { timeEntry }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Destroy([DataSourceRequest] DataSourceRequest request, TimeEntryModel timeEntry)
        {
            if (timeEntry != null)
            {
                timeEntryService.Destroy(timeEntry);
            }

            return Json(new[] { timeEntry }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ConductPayroll([DataSourceRequest]DataSourceRequest request, bool isPayrollForMnps)
        {
            using (var db = new HomeworkHotlineEntities())
            {
                DataSourceResult result = db.TimeEntries.ToDataSourceResult(request);

                List<TimeEntry> timeEntriesList = (List<TimeEntry>)result.Data;
                List<TimeEntry> timeEntries = timeEntriesList;
                if (isPayrollForMnps) {
                   timeEntries = timeEntryService.GetMNPSTimeEntries(timeEntriesList);
                }

                foreach (TimeEntry item in timeEntries)
                {
                    if (item.EndTime != null)
                    {
                        item.IsLocked = true;
                        if (item.PayrollDate == null) item.PayrollDate = DateTime.Now;
                    }
                }

                db.SaveChanges();

                return View();
            }
        }
    }
}