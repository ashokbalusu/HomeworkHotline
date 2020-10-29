using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ChartModel
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public string ChartElementName { get; set; }
        public double ChartElementValue { get; set; }
    }
}
