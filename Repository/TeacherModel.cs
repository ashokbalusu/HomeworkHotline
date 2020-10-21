using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TeacherModel
    {
        public string UserID { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name ="MNPS Employee #")]
        public string MNPSEmployeeNo { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool Volunteer { get; set; }
        public string MainSubject { get; set; }
        public string SubjectOther { get; set; }
        public string GradeLevel { get; set; }
    }
}
