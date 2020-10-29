using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using System.Web;
using System.Data;
using System.Net.Http;
using System.Data.Entity;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Microsoft.SqlServer.Server;
using System.IO;
using System.IO.Compression;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Repository
{

    public class ReportService
    {
        private HomeworkHotlineEntities entities;

        public ReportService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public List<ReportModel> GetReportData(DateTime startDate, DateTime endDate, List<int> countyIds = null)
        {
            var reportData = new List<ReportModel>();
            entities.Database.Initialize(force: false);

            var cmd = entities.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[usp_ReportSummary]";
            cmd.CommandTimeout = 10000;

            var startDateParam = new SqlParameter("StartDate", SqlDbType.DateTime);
            startDateParam.Value = startDate;
            cmd.Parameters.Add(startDateParam);

            var endDateParam = new SqlParameter("EndDate", SqlDbType.DateTime);
            endDateParam.Value = endDate;
            cmd.Parameters.Add(endDateParam);

            if (countyIds != null && countyIds.Count > 0)
            {
                var countyIdTableSchema = new List<SqlMetaData>(1)
                    {
                            new SqlMetaData("CountyId", SqlDbType.Int),
                            new SqlMetaData("CountyName", SqlDbType.VarChar, 1000)
                     }.ToArray();

                var countyIdTable = new List<SqlDataRecord>();

                foreach (var countId in countyIds)
                {
                    var tableRow = new SqlDataRecord(countyIdTableSchema);
                    tableRow.SetValue(0, countId);
                    tableRow.SetValue(1, "");
                    countyIdTable.Add(tableRow);
                }

                var countyIdParam = new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CountyName",
                    TypeName = "[dbo].[CountyList]",
                    Value = countyIdTable
                };

                cmd.Parameters.Add(countyIdParam);
            }

            try
            {
                entities.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                var studentsChartData = new List<ChartModel>();
                var studentChartData = new ChartModel();

                var sessionsResultsChartData = new List<ChartModel>();
                var sessionResultsChartData = new ChartModel();

                var sessionsChartData = new List<ChartModel>();
                var sessionChartData = new ChartModel();

                var subjectBreakdownsChartData = new List<ChartModel>();
                var subjectBreakdownChartData = new ChartModel();

                var schoolSessionsStudentGridData = new List<SchoolSessionStudentGrid>();
                var schoolSessionStudentGridData = new SchoolSessionStudentGrid();

                reader.NextResult();

                while (reader.Read())
                {
                    int columnOrdinal = 0;
                    studentChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    studentChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    studentChartData.CountyName = reader.GetString(columnOrdinal);

                    studentChartData.ChartElementName = "Students";
                    
                    columnOrdinal = reader.GetOrdinal("StudentsChart1");
                    studentChartData.ChartElementValue = reader.GetInt32(columnOrdinal);

                    studentsChartData.Add(studentChartData);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    int columnOrdinal = 0;
                    sessionChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    sessionChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    sessionChartData.CountyName = reader.GetString(columnOrdinal);

                    sessionChartData.ChartElementName = "Sessions";

                    columnOrdinal = reader.GetOrdinal("SessionsChart1");
                    sessionChartData.ChartElementValue = reader.GetInt32(columnOrdinal);

                    sessionsChartData.Add(sessionChartData);
                }

                reader.NextResult();

                while (reader.Read())
                {
                    int columnOrdinal = 0;
                    subjectBreakdownChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    subjectBreakdownChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    subjectBreakdownChartData.CountyName = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("SubjectGroup");
                    subjectBreakdownChartData.ChartElementName = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CallBySubjectGroup");
                    subjectBreakdownChartData.ChartElementValue = reader.GetInt32(columnOrdinal);

                    subjectBreakdownsChartData.Add(subjectBreakdownChartData);
                }

                reader.NextResult();
                reader.NextResult();
                reader.NextResult();
                reader.NextResult();
                reader.NextResult();
                reader.NextResult();
                reader.NextResult();

                while (reader.Read())
                {
                    int columnOrdinal = 0;
                    schoolSessionStudentGridData = new SchoolSessionStudentGrid();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    schoolSessionStudentGridData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    schoolSessionStudentGridData.CountyName = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("Count of Student Grade");
                    schoolSessionStudentGridData.GradeCount = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("Grade");
                    schoolSessionStudentGridData.Grade = reader.GetString(columnOrdinal);

                    schoolSessionsStudentGridData.Add(schoolSessionStudentGridData);
                }

                reportData = studentsChartData.Select(s => new ReportModel() { CountyId = s.CountyId, CountyName = s.CountyName }).ToList();
                foreach (var report in reportData)
                {
                    report.StudentsAndSessions = studentsChartData.Union(sessionsChartData).Where(c => c.CountyId == report.CountyId).ToList();
                    report.SubjectBreakdown = subjectBreakdownsChartData.Where(c => c.CountyId == report.CountyId).ToList();
                    report.SchoolSessionsStudentGrid = schoolSessionsStudentGridData.Where(c => c.CountyId == report.CountyId).ToList();
                }
            }
            finally
            {
                entities.Database.Connection.Close();
            }

            return reportData;
        }

        public Stream GetReportZip(List<ReportModel> reportData) {
            var outStream = new MemoryStream();

            try
            {
                using (ZipArchive archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    var countyIds = reportData.Select(p => p.CountyId).Distinct().OrderBy(c => c);
                    foreach (var countyId in countyIds)
                    {
                        var reportCountyData = reportData.Where(r => r.CountyId == countyId).Single();
                        var docxStream = GenerateReportDocX(reportCountyData);
                        var zipEntry = archive.CreateEntry("HH Report" + reportCountyData.CountyName + ".docx");
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            docxStream.CopyTo(zipEntryStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outStream.Close();
            }

            return outStream;
        }

        public Stream GenerateReportDocX(ReportModel reportData)
        {
            var docxStream = new MemoryStream();

            string fileName = "Report_Template.docx";
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filepath = Path.Combine(assemblyFolder + "/Documents/", fileName);

            using (WordprocessingDocument wordDoc =
                    WordprocessingDocument.Open(filepath, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText = new Regex("[#countyschools_ucase]");
                docText = regexText.Replace(docText, reportData.CountyName);
            }

            return docxStream;
        }

        public void Dispose()
        {
            entities.Dispose();
        }

    }
}
