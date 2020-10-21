using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.Reporting.WebForms;

namespace HomeworkHotline.WebForms
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ReportFolder"]))
                {
                    string reportpath = HttpUtility.HtmlDecode(Request.QueryString["ReportFolder"]);
                    int aantalKeys = Request.Params.AllKeys.Length;

                    List<ReportParameter> parameters = new List<ReportParameter>();
                    for (int i = 1; i < aantalKeys; i++)
                    {

                        string value = Request.Params[i];
                        string key = Request.Params.Keys[i];
                        if (key.Contains("_RAP"))
                        {
                            int index = key.IndexOf('_');
                            key = key.Substring(0, index);
                            ReportParameter parameter = new ReportParameter(key, HttpUtility.HtmlDecode(value));
                            parameters.Add(parameter);
                        }
                    }
                    this.RenderReport(reportpath, parameters);
                }

            }
        }

        private void RenderReport(string reportpath, List<ReportParameter> parameters = null)
        {
            string User = "hhUser"; // [ReportserverUser];
            string Pass = "qazWSXedc_101"; //[ReportserverPass];
            string ReportServerUrl = "http://localhost/ReportServer"; // [ResportserverUrl]];
         //   IReportServerCredentials irsc = new CustomReportCredentials(User, Pass, "");
            Uri uri = new Uri(ReportServerUrl);
            int lastSegment = uri.Segments.Length - 1;
            string page = uri.Segments[lastSegment];

            // EVENTS
            //reportViewer.Load += reportViewer_Load;
            //reportViewer.Unload += reportViewer_Unload;
           
            rptViewer.Visible = true;
            rptViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;  // Remote
                                                                                           //   reportViewer.ServerReport.ReportServerCredentials = irsc;
            rptViewer.ServerReport.ReportServerUrl = new Uri(uri.AbsoluteUri.Replace(page, ""));
            rptViewer.ServerReport.ReportPath = reportpath;
            if (parameters != null && parameters.Count != 0)
            {
                rptViewer.ServerReport.SetParameters(parameters);

            }
            rptViewer.ServerReport.Refresh();
        }

        private Dictionary<string, object> GetCurrentParameters()
        {

            var parameterCollection = rptViewer.ServerReport.GetParameters();


            var param = new Dictionary<string, object>();
            foreach (var p in parameterCollection)
            {
                var name = p.Name;
                if (p.DataType == ParameterDataType.DateTime)
                {
                    var d = Convert.ToDateTime(p.Values[0]);
                    param[name] = d.ToString("dd-MM-yyyy");
                }
                else
                {
                    var values = p.Values.ToList();
                    param[name] = values;
                }

            }
            return param;
        }


    }
}