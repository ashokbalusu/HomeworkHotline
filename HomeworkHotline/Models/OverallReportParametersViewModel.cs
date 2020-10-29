using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeworkHotline.Models
{
    public class OverallReportParametersViewModel
    {
        [Required(ErrorMessage = "Please Select a Date")]
        [Display(Name = "From Date")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Please Select a Date")]
        [Display(Name = "To Date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Counties")]
        [Required(ErrorMessage = "Please Select One or More Counties")]
        public List<int> Counties { get; set; }
        [Display(Name = "Report Aggregation")]
        public OverallReportAggregationType AggregationType { get; set; }
    }

    public enum OverallReportAggregationType
    {
        [Display(Name = "One by One")]
        Individual,
        [Display(Name = "Flying Solo")]
        Aggregate
    }
}