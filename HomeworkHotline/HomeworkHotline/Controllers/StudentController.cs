using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;

namespace HomeworkHotline.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {

        private StudentService studentService;
        public StudentController()
        {
            studentService = new StudentService(new HomeworkHotlineEntities());
        }
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            GetDropdownData();
            ViewBag.TotalCount = studentService.Read().Count();
            ViewBag.IsAdmin = User.IsInRole("Administrator");
            return View();
        }

        public ActionResult StudentManage(bool addStudentOnly = false)
        {
            TempData["AdminAddStudent"] = addStudentOnly;
            TempData.Keep("AdminAddStudent");

            StudentModel studentModel = TempData["studentModel"] as Repository.StudentModel;
            GetDropdownData();
            return View(studentModel);
        }

        public ActionResult StudentConfirmation()
        {
            StudentModel studentModel = TempData["studentModel"] as Repository.StudentModel;
            TempData.Keep("language");
            TempData.Keep("studentModel");
            return View(studentModel);
        }

        [HttpPost]
        public ActionResult ConfirmStudent(StudentModel studentModel)
        {
            Student studentExists = new Student();
            if (studentModel.CodeName != null)
            {
                studentExists = GetStudent(studentModel);

                if (studentExists != null) //if student exists, confirm student
                {
                    if (studentExists.IsDeleted)
                    {
                        ModelState.AddModelError("", "That codename is not available. Please enter a different code name.");
                        return View("Index");
                    } else { 
                        var student = MapStudentToModel(studentExists);
                        TempData["language"] = new HomeworkHotlineEntities()
                        .LanguageTypes
                        .Select(e => new LanguageTypeModel
                        {
                            LanguageID = e.LanguageID,
                            LanguageName = e.LanguageName
                        }).Where(e => e.LanguageID == student.HomeLanguageID)
                        .OrderBy(e => e.LanguageID).FirstOrDefault().LanguageName;
                        TempData["studentModel"] = student;
                        return RedirectToAction("StudentConfirmation");
                    }
                }
           
                StudentModel newStudentModel = new StudentModel();
                newStudentModel.CodeName = studentModel.CodeName.Trim();
                TempData["studentModel"] = newStudentModel;
                return RedirectToAction("StudentManage");
            } else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ContinueWithCall(StudentModel studentModel)
        {
            var student = GetStudent(studentModel);
            TempData["studentModel"] = MapStudentToModel(student);
            return RedirectToAction("Index", "CallLog");
        }
        private Student GetStudentById(int studentId)
        {
            Student student = null;
            using (var db = new HomeworkHotlineEntities())
            {
                student = (from s in db.Students
                           where s.StudentID == studentId
                           select s).FirstOrDefault();
            }
            return student;
        }

        private Student GetStudent(StudentModel studentModel)
        {
            Student student = null;
            using (var db = new HomeworkHotlineEntities())
            {
                student = (from s in db.Students
                               where s.CodeName == studentModel.CodeName.Trim()
                               select s).FirstOrDefault();
            }
            return student;
        }
        public void GetDropdownData()
        {
            var languages = new HomeworkHotlineEntities()
            .LanguageTypes
            .Select(e => new LanguageTypeModel
            {
                LanguageID = e.LanguageID,
                LanguageName = e.LanguageName
            })
            .OrderBy(e => e.LanguageID);

            ViewData["languageTypes"] = languages;


            var schools = new HomeworkHotlineEntities()
            .Schools
            .Select(e => new SchoolModel
            {
                SchoolID = e.SchoolID,
                SchoolName = e.SchoolName,
                IsDeleted = e.IsDeleted
            })
            .Where (e => e.IsDeleted == false)
            .OrderBy(e => e.SchoolName);

            ViewData["schools"] = schools;


            var referredTypes = new HomeworkHotlineEntities()
            .ReferredTypes
            .Select(e => new ReferredTypeModel
            {
                ReferredTypeID = e.ReferredTypeID,
                Name = e.Name
            })
            .OrderBy(e => e.ReferredTypeID);

            ViewData["referredTypes"] = referredTypes;

        }



        [HttpPost]
        public ActionResult Update(StudentModel studentModel)
        {
            Student studentExists = new Student();
            using (var db = new HomeworkHotlineEntities())
            {
                studentExists = GetStudent(studentModel);

                Student newStudent = MapModelToStudent(studentModel);
                if (ModelState.IsValid && studentExists == null)

                {
                    db.Students.Add(newStudent);
                    db.SaveChanges();

                    TempData["studentModel"] = MapStudentToModel(newStudent);
                    if ((bool)TempData["AdminAddStudent"])
                        return RedirectToAction("List");
                    return RedirectToAction("Index", "CallLog");
                }

                ModelState.AddModelError("", "That codename is not available. Please enter a different code name.");
                GetDropdownData();
                return View("StudentManage");
            }

        }

        public Student MapModelToStudent(StudentModel studentModel)
        {
            var userId = User.Identity.GetUserId();

            Student student = new Student();
            student.CodeName = studentModel.CodeName;
            student.Grade = studentModel.Grade;
            student.HomeLanguageID = studentModel.HomeLanguageID;
            student.HomeLanguageOther = studentModel.HomeLanguageOther;
            student.HasInternet = studentModel.HasInternet;
            student.SchoolID = studentModel.SchoolID;
            student.ReferredTypeID = studentModel.ReferredTypeID;
            student.GuardianContactNumber = studentModel.GuardianContactPhone;
            student.GuardianContactEmail = studentModel.GuardianContactEmail;
            student.StudentContactNumber = studentModel.StudentContactPhone;
            student.StudentFirstName = studentModel.StudentFirstName;
            student.StudentLastName = studentModel.StudentLastName;
            student.CreateDate = DateTime.Now;
            student.CreateBy = userId;
            student.GuardianOptOut = studentModel.GuardianOptOut;
            student.StudentOptOut = studentModel.StudentOptOut;
            return student;
        }

        public StudentModel MapStudentToModel(Student student)
        {
            using (var db = new HomeworkHotlineEntities())
            {
                StudentModel studentModel = new StudentModel();
                studentModel.StudentID = student.StudentID;
                studentModel.CodeName = student.CodeName;
                studentModel.StudentFirstName = student.StudentFirstName;
                studentModel.StudentLastName = student.StudentLastName;
                studentModel.Grade = student.Grade;
                studentModel.HomeLanguageID = student.HomeLanguageID;
                studentModel.HomeLanguageOther = student.HomeLanguageOther;
                studentModel.HasInternet = student.HasInternet;
                studentModel.HasInternetString = student.HasInternet != null && (bool)student.HasInternet ? "Yes" : "No";
                studentModel.SchoolID = student.SchoolID; 
                studentModel.ReferredTypeID = student.ReferredTypeID;
                studentModel.GuardianContactPhone = student.GuardianContactNumber;
                studentModel.GuardianContactEmail = student.GuardianContactEmail;
                studentModel.StudentContactPhone = student.StudentContactNumber;
                studentModel.StudentFirstName = studentModel.StudentFirstName;
                studentModel.StudentLastName = studentModel.StudentLastName;
                studentModel.GuardianOptOut = student.GuardianOptOut;
                studentModel.StudentOptOut = student.StudentOptOut;
                var school = (from s in db.Schools where student.SchoolID == s.SchoolID select s).FirstOrDefault();
                if (school != null)
                {
                    studentModel.SchoolName = school.SchoolName;
                    studentModel.CountyName = (from c in db.Counties where c.CountyID == school.CountyID select c.CountyName).FirstOrDefault();
                }

                studentModel.TotalCalls = (from c in db.CallLogs where c.StudentID == student.StudentID select c).Count();

                return studentModel;
            }
        }



        public ActionResult Students_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult = Json(studentService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Students_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<StudentModel> students)
        {
                if (students != null && ModelState.IsValid)
            {
                StudentService studentService = new StudentService(new HomeworkHotlineEntities());
                var userId = User.Identity.GetUserId();

                foreach (var updatedStudent in students)
                {
                    var beginningCodeName = GetStudentById(updatedStudent.StudentID).CodeName;
                    if (beginningCodeName != updatedStudent.CodeName)
                    {
                        var studentExists = GetStudent(updatedStudent);
                        if (studentExists != null)
                        {
                            ViewBag.Message = "A student with that code name already exists";
                            return View();
                        }
                    }
                    studentService.Update(updatedStudent, userId);
                }
            }

            return Json(students.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Students_Delete([DataSourceRequest] DataSourceRequest request,
      [Bind(Prefix = "models")]IEnumerable<StudentModel> students)
        {
            if (students != null)
            {
                var student = students.First();

                StudentService studentService = new StudentService(new HomeworkHotlineEntities());
                studentService.Destroy(student);
            }
            return Json(ModelState.ToDataSourceResult());
        }
    }


}