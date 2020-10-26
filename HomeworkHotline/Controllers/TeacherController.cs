using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace HomeworkHotline.Controllers
{
    [Authorize(Roles="Administrator")]
    public class TeacherController : Controller
    {

        private TeacherService teacherService;
        private AccountController accountController;
        public TeacherController()
        {
            teacherService = new TeacherService(new HomeworkHotlineEntities());
            GetDropdownData();

            accountController = new AccountController();
        }
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }
        public void GetDropdownData()
        {
            ViewData["subjectTypes"] = new HomeworkHotlineEntities()
            .SubjectTypes
            .Select(e => new SubjectTypeModel
            {
                SubjectTypeID = e.SubjectTypeID,
                SubjectTypeName = e.SubjectTypeName
            })
            .OrderBy(e => e.SubjectTypeID);
        }
        public ActionResult Teachers()
        {
            IEnumerable<TeacherModel> teachers = Repository.Teachers.GetTeacherList();
            ViewBag.TotalCount = teachers.Count();
            return View(teachers);
        }


        public ActionResult Teachers_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult = Json(teacherService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Teachers_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<TeacherModel> teachers)
        {
            if (teachers != null && ModelState.IsValid)
            {
                foreach (var teacher in teachers)
                {
                    accountController.Update(teacher);
                }
            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
            //return Json(teachers.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Teachers_Delete([DataSourceRequest] DataSourceRequest request,
      [Bind(Prefix = "models")]IEnumerable<TeacherModel> teachers)
        {
            if (teachers != null)
            {
                foreach (var teacher in teachers)
                {
                    accountController.Delete(teacher);
                }
            }
            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }
    }
}