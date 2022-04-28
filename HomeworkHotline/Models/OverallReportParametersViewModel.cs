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
        [Required(ErrorMessage = "Please select a date")]
        [Display(Name = "From Date")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        [Display(Name = "To Date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Counties")]
        [Required(ErrorMessage = "Please select one or more counties")]
        public List<int> Counties { get; set; }
        [Display(Name = "Grades")]
        public List<string> Grades { get; set; }
        [Display(Name = "Subjects")]
        public List<string> Subjects { get; set; }
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