using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNet.Identity;

namespace HomeworkHotline.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SchoolController : Controller
    {
        private HomeworkHotlineEntities context = new HomeworkHotlineEntities();
        private SchoolService schoolService;

        public SchoolController() {
            schoolService = new SchoolService(context);
        }
        // GET: School
        public ActionResult Index()
        {
            ViewBag.TotalCount = schoolService.Read().Count();
            GetDropdownData();
            return View();
        }

        public ActionResult PrincipalList()
        {
            ViewBag.TotalPrincipalCount = schoolService.GetPrincipalList().Count();
            return View();
        }

        public ActionResult BulkEditSchoolList()
        {
            SchoolService schoolService = new SchoolService(context);

            ViewBag.TotalCount = schoolService.Read().Count();
            return View();
        }

        public void GetDropdownData()
        {
            // TODO: move to repo class and add where clause for !isDeleted

            ViewData["schoolTypes"] = new HomeworkHotlineEntities()
            .SchoolTypes
            .Select(e => new SchoolTypeModel
            {
                SchoolTypeID = e.SchoolTypeID,
                SchoolTypeName = e.SchoolTypeName
            })
            .OrderBy(e => e.SchoolTypeID);

            ViewData["counties"] = new HomeworkHotlineEntities()
            .Counties
            .Select(e => new CountyModel
            {
                CountyID = e.CountyID,
                CountyName = e.CountyName
                //String.Format("{0}{1}", e.CountyName, !String.IsNullOrWhiteSpace(e.StateCode) ? ", " + e.StateCode : "")
            })
            .OrderBy(e => e.CountyName);

            ViewData["states"] = new HomeworkHotlineEntities()
             .States
             .Select(e => new StatesModel
             {
                 StateCode = e.StateCode,
                 StateName = e.StateCode
                    })
             .OrderByDescending(e => e.StateCode);

            ViewData["schoolDistricts"] = new HomeworkHotlineEntities()
             .SchoolDistricts
             .Select(e => new SchoolDistrictModel
             {
                 SchoolDistrictID = e.SchoolDistrictID,
                 DistrictName = e.DistrictName,
                 StateCode = e.StateCode,
                 IsDeleted = e.IsDeleted
             })
             .Where (e => e.IsDeleted == false)
             .OrderBy(e => e.DistrictName);

            ViewData["quadrants"] = new HomeworkHotlineEntities()
         .MnpsQuadrants
         .Select(e => new MnpsQuadrantModel
         {
             QuadrantID = e.QuadrantID,
             QuadrantName = e.QuadrantName
         })
         .OrderBy(e => e.QuadrantName);

        }

        public ActionResult PrintLabels(string id, string filterArray)
        {
            const int pageMargin = 5;
            const int pageRows = 5;
            const int pageCols = 2;
            const int topMargin = 40;
            var doc = new Document(PageSize.LETTER, pageMargin, pageMargin, topMargin, 10);
            var memoryStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();
            var schools = JsonConvert.DeserializeObject<List<SchoolModel>>(id);
            string[] filters = JsonConvert.DeserializeObject<string[]>(filterArray);
            var displayPrincipal = filters.Contains("Principal");
            var displayCountyStudent = filters.Contains("CountyStudent");
            var displayCountyFlashcard = filters.Contains("CountyFlashcard");
            // Create the Label table

            PdfPTable table = new PdfPTable(pageCols);
            table.WidthPercentage = 100f;
            table.DefaultCell.Border = 0;

            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            foreach (var school in schools)
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (topMargin * 2)) / pageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_CENTER;
                if (displayPrincipal)
                    contents.Add(new Chunk(string.Format("Admin: {0}\n", school.PrincipalName), new Font(baseFont, 10f)));
                contents.Add(new Chunk(string.Format("{0}\n", school.SchoolName), new Font(baseFont, 10f)));
                contents.Add(new Chunk(string.Format("{0}\n", school.Address1), new Font(baseFont, 10f)));
                contents.Add(new Chunk(string.Format("{0}, {1} {2}\n", school.City, school.State, school.Zip), new Font(baseFont, 10f)));
                if (displayCountyStudent)
                    contents.Add(new Chunk(string.Format("{0} {1}\n", school.CountyName, school.Census.ToString()), new Font(baseFont, 10f)));
                if (displayCountyFlashcard)
                    contents.Add(new Chunk(string.Format("{0} {1}\n", school.CountyName, school.PredictedThirdGradeStudents.ToString()), new Font(baseFont, 10f)));

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            table.CompleteRow();
            doc.Add(table);

            pdfWriter.CloseStream = false;
            doc.Close();
            memoryStream.Position = 0;
            string handle = Guid.NewGuid().ToString();
            TempData[handle] = memoryStream.ToArray();
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "SchoolLabels.pdf" }
            };
        }
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


        public ActionResult GetAllPrincipals([DataSourceRequest]DataSourceRequest request)
        {
            return Json(schoolService.GetPrincipalList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Schools_Read([DataSourceRequest] DataSourceRequest request)
        {
            JsonResult jsonResult = Json(schoolService.Read().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Create([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<SchoolModel> schools)
        {
            var results = new List<SchoolModel>();
            var userId = User.Identity.GetUserId();

            if (schools != null && ModelState.IsValid)
            {
                SchoolService schoolService = new SchoolService(context);
                foreach (var school in schools)
                {
                    schoolService.Create(school, userId);

                    results.Add(school);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Update([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<SchoolModel> schools)
        {
            if (schools != null && ModelState.IsValid)
            {
                SchoolService schoolService = new SchoolService(context);
                var userId = User.Identity.GetUserId();

                foreach (var school in schools)
                {

                    schoolService.Update(school, userId);
                }
            }

            return Json(schools.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Schools_Delete([DataSourceRequest] DataSourceRequest request,
      [Bind(Prefix = "models")]IEnumerable<SchoolModel> schools)
        {
            if (schools != null)
            {
                var school = schools.First();

                SchoolService schoolService = new SchoolService(context);
                schoolService.Destroy(school);
            }
              return Json(ModelState.ToDataSourceResult());
            }
    }
}