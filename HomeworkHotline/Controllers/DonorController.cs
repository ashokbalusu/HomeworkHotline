using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace HomeworkHotline.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DonorController : Controller
    {
        private DonorService donorService;
        private DonorGivingService donorGivingService;
        public DonorController()
        {
            donorService = new DonorService(new HomeworkHotlineEntities());
            donorGivingService = new DonorGivingService(new HomeworkHotlineEntities());
        }

        public ActionResult Index()
        {
            ViewBag.TotalCount = donorService.Read().Count();

            GetDropdownData();
            return View();
        }

        public ActionResult List()
        {
            GetDropdownData();
            return View();
        }


        public void GetDropdownData()
        {
            ViewData["states"] = new HomeworkHotlineEntities()
             .States
             .Select(e => new StatesModel
             {
                 StateCode = e.StateCode,
                 StateName = e.StateCode
             })
             .OrderByDescending(e => e.StateCode);
        }

        public ActionResult Donors_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult = Json(donorService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Donors_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<DonorModel> donors)
        {
            var results = new List<DonorModel>();

            if (donors != null && ModelState.IsValid)
            {
                foreach (var donor in donors)
                {
                    donorService.Create(donor);

                    results.Add(donor);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Donors_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<DonorModel> donors)
        {
            if (donors != null && ModelState.IsValid)
            {
                foreach (var donor in donors)
                {

                    donorService.Update(donor);
                }
            }

            return Json(donors.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Donors_Delete([DataSourceRequest] DataSourceRequest request,
      [Bind(Prefix = "models")]IEnumerable<DonorModel> donors)
        {
            if (donors != null)
            {
                var donor = donors.First();

                donorService.Destroy(donor);
            }
            return Json(ModelState.ToDataSourceResult());
        }



        //Donor Giving 
        public ActionResult DonorGivings_Read([DataSourceRequest] DataSourceRequest request, int parentID)
        {
            return Json(donorGivingService.Read(parentID).ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DonorGivings_Create(int parentID, [DataSourceRequest] DataSourceRequest request, DonorGivingModel donorGiving)
        {

            if (donorGiving != null)
            {
                donorGiving.DonorID = parentID;
                donorGivingService.Create(donorGiving);
            }

            return Json(new[] { donorGiving }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DonorGivings_Update([DataSourceRequest] DataSourceRequest request, DonorGivingModel donorGiving)
        {
            if (donorGiving != null && ModelState.IsValid)
            {
                donorGivingService.Update(donorGiving);
            }

            return Json(new[] { donorGiving }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DonorGivings_Delete([DataSourceRequest] DataSourceRequest request, DonorGivingModel donorGiving)
        {
            if (donorGiving != null)
            {
                donorGivingService.Destroy(donorGiving);
            }

            return Json(new[] { donorGiving }.ToDataSourceResult(request, ModelState));
        }

    }
}
