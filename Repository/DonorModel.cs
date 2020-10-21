using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DonorModel
    {
        public int DonorID { get; set; }
        public string DonorName { get; set;}
        public Nullable<decimal> Amount { get; set; }
        public string Phone { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string DonorState { get; set; }
        public string Zip { get; set; }
        public string BusinessName { get; set; }
        public bool IsDeleted { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }

        public string DonorName2
        {
            get
            {
                if (DonorName == null)
                    return Title + " " + FirstName + " " + LastName;
                else
                    return DonorName;
            }
            set { }
        }

        public decimal? TotalDonations { get; set; }
    }

    public class DonorGivingModel
    {
        public int DonorGivingID { get; set; }
        [Required]
        public int DonorID { get; set; }
        [Required]
        public decimal AmountGiven { get; set; }
        public DateTime DateGiven { get; set; }
        public bool InKind { get; set; }
        public bool IsDeleted { get; set; }
    }
}
