using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Repository
{
   public class StudentModel
    {
        public int StudentID { get; set; }
        [Required(ErrorMessage = "Student code is required")]
       [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string CodeName { get; set; }
        [Required(ErrorMessage = "Grade is required")]
        public string Grade { get; set; }
        [Required(ErrorMessage = "Language is required")]
        public int? HomeLanguageID { get; set; }
        public string HomeLanguageOther { get; set; }
        public bool? HasInternet { get; set; }
        public string HasInternetString { get; set; }
        [Required(ErrorMessage ="School is required")]
        public int? SchoolID { get; set; }
        [Required(ErrorMessage ="Referred by is required")]
        public int? ReferredTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public string LanguageName { get; set; }
        public string SchoolName { get; set; }
        public string CountyName { get; set; }
        public string ReferredTypeName { get; set; }
        public int? TotalCalls { get; set; }
        public string GuardianContactPhone { get; set; }
        public string GuardianContactEmail { get; set; }
        public string StudentContactPhone { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool StudentOptOut { get; set; }
        public bool GuardianOptOut { get; set; }
    }
}

