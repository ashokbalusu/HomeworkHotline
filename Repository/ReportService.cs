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
using OpenXmlPowerTools;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Repository.ReportModels;

namespace Repository
{

    public class ReportService
    {
        private HomeworkHotlineEntities entities;

        public ReportService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public List<ReportModel> GetReportData(DateTime startDate, DateTime endDate, List<int> countyIds = null, bool isAggregate = false)
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

            var aggregateParam = new SqlParameter("Aggregate", SqlDbType.VarChar);
            aggregateParam.Value = isAggregate ? "All" : "Individual";
            cmd.Parameters.Add(aggregateParam);

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

                var sessionsPerGradeChartData = new List<ChartModel>();
                var sessionPerGradeChartData = new ChartModel();

                var totals = new List<Total>();
                var greenSection = new GreenSection();
                var districts = new List<District>();
                var schools = new List<ReportModels.School>();
                var ytds = new List<SessionsYTD>();
                
                // TO DO - REMOVE
                reader.NextResult();

                #region Result Set 1 - Totals

                while (reader.Read())
                {
                    int columnOrdinal = 0;

                    var total = new Total();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    total.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    total.CountyName = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("MinutesVar");
                    total.MinutesVar = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("Tutoring");
                    total.Tutoring = reader.GetDouble(columnOrdinal);

                    totals.Add(total);
                }

                reader.NextResult();

                #endregion

                #region Result Set 2 - SessionsFromYTD

                while (reader.Read())
                {
                    var columnOrdinal = 0;

                    var ytd = new SessionsYTD();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    ytd.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("PreviousYear");
                    ytd.PreviousYear = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CurrentYear");
                    ytd.CurrentYear = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CurrentSeason");
                    ytd.CurrentSeason = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("YTDSessions");
                    ytd.Sessions = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CurrentQuarter");
                    ytd.Quarter = reader.GetString(columnOrdinal);

                    ytds.Add(ytd);
                }

                reader.NextResult();
                #endregion

                #region Result Set 3 - Students and Sessions Chart (Students Count)
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

                #endregion

                #region Result Set 4 - Students and Sessions Chart (Sessions Count)
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

                #endregion

                #region Result Set 5 - Sessions Results Chart

                while (reader.Read())
                {
                    int columnOrdinal = 0;

                    sessionResultsChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    sessionResultsChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CompletedAssignment");
                    sessionResultsChartData.ChartElementName = "Completed Assignment";
                    sessionResultsChartData.ChartElementValue = reader.GetDouble(columnOrdinal) / 100.0;

                    sessionsResultsChartData.Add(sessionResultsChartData);

                    sessionResultsChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    sessionResultsChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("PostTestPassed");
                    sessionResultsChartData.ChartElementName = "Post-Test Passed";
                    sessionResultsChartData.ChartElementValue = reader.GetDouble(columnOrdinal) / 100.0;

                    sessionsResultsChartData.Add(sessionResultsChartData);
                }

                reader.NextResult();
                #endregion

                #region Result Set 6 - Subject Breakdown Chart
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
                #endregion

                #region Result Set 7 - Green Section

                while (reader.Read())
                {
                    var columnOrdinal = 0;

                    columnOrdinal = reader.GetOrdinal("GreenThroughDate");
                    greenSection.ThroughDate = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("GreenSessions");
                    greenSection.Sessions = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("GreenStudents");
                    greenSection.Students = reader.GetInt64(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("GreenParents");
                    greenSection.Parents = reader.GetInt64(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("GreenMinutes");
                    greenSection.Minutes = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("TeacherPositionsPerWeek");
                    greenSection.TeacherPositionsPerWeek = reader.GetInt32(columnOrdinal);
                }

                reader.NextResult();
                #endregion

                #region Result Set 8 - Total District Tutoring Hours

                while (reader.Read())
                {
                    var columnOrdinal = 0;

                    var district = new District();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    district.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("TotalDistrictTutoringHours");
                    district.TotalTutoringHours = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("TutoringHours");
                    district.TutoringHours = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("PhonesPercent");
                    district.PhonesPercent = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("PhoneUsageRate");
                    district.PhoneUsageRate = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("TotalDistrictPromotionalItems");
                    district.TotalPromotionalItems = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("PromotionalItemCostsCents");
                    district.PromotionalItemCostCents = reader.GetDouble(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CurrentYearStudents");
                    district.CurrentYearStudents = reader.GetInt64(columnOrdinal);

                    districts.Add(district);
                }

                reader.NextResult();
                #endregion

                #region Result Set 9 - Sessions per Grade Chart
                while (reader.Read())
                {
                    int columnOrdinal = 0;
                    sessionPerGradeChartData = new ChartModel();

                    columnOrdinal = reader.GetOrdinal("CountyId");
                    sessionPerGradeChartData.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountyName");
                    sessionPerGradeChartData.CountyName = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("CountOfStudentGrade");
                    sessionPerGradeChartData.ChartElementValue = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("Grade");
                    sessionPerGradeChartData.ChartElementName = reader.GetString(columnOrdinal);

                    sessionsPerGradeChartData.Add(sessionPerGradeChartData);
                }

                reader.NextResult();
                #endregion

                #region Result Set 10 - School Table

                while (reader.Read())
                {
                    var columnOrdinal = 0;
                    var school = new ReportModels.School();

                    columnOrdinal = reader.GetOrdinal("CountyID");
                    school.CountyId = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("SchoolName");
                    school.Name = reader.GetString(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("NoOfSessions");
                    school.NumberOfSessions = reader.GetInt32(columnOrdinal);

                    columnOrdinal = reader.GetOrdinal("NoOfStudents");
                    school.NumberOfStudents = reader.GetInt32(columnOrdinal);

                    schools.Add(school);
                }

                reader.NextResult();
                #endregion

                reportData = studentsChartData.Select(s => new ReportModel() { CountyId = s.CountyId, CountyName = s.CountyName }).ToList();
                foreach (var report in reportData)
                {
                    report.TotalMinutes = string.Format("{0:n0}", Math.Round(totals.SingleOrDefault(c => c.CountyId == report.CountyId).MinutesVar));
                    report.TutoringProvided = string.Format("{0:n0}", Math.Round(totals.SingleOrDefault(c => c.CountyId == report.CountyId).Tutoring));

                    report.ThroughDate = greenSection.ThroughDate;
                    report.TotalSessions = greenSection.Sessions;
                    report.TotalIndividualStudents = greenSection.Students;
                    report.TotalIndividualParents = greenSection.Parents;
                    report.TotalMinutesFreeTutoring = Convert.ToInt32(greenSection.Minutes);
                    report.TotalTeacherPositionsPerWeek = greenSection.TeacherPositionsPerWeek;
                    var district = districts.SingleOrDefault(d => d.CountyId == report.CountyId);

                    if (district != null)
                    {
                        report.DistrictPromotionalItemCost = district.TotalPromotionalItems;
                        report.DistrictTutoringHourCost = district.TotalTutoringHours;
                        report.DistrictTutoringHours = district.TutoringHours;
                        report.DistrictPromotionalItemStudents = district.CurrentYearStudents;
                        report.DistrictPhonesPercentOfUsage = district.PhonesPercent;
                        report.DistrictPhonesCost = district.PhoneUsageRate;
                        report.DistirctPromotionalItemRate = district.PromotionalItemCostCents;
                    }

                    report.StudentsAndSessions = studentsChartData.Union(sessionsChartData).Where(c => c.CountyId == report.CountyId).ToList();
                    report.SessionResults = sessionsResultsChartData.Where(c => c.CountyId == report.CountyId).ToList();
                    report.SubjectBreakdown = subjectBreakdownsChartData.Where(c => c.CountyId == report.CountyId).ToList();
                    report.SessionsPerGrade = sessionsPerGradeChartData.Where(c => c.CountyId == report.CountyId).OrderBy(c => TryGetInt(c.ChartElementName)).ToList();
                    report.Schools = schools.Where(s => s.CountyId == report.CountyId).ToList();

                    var ytd = ytds.First();

                    report.CurrentYearRange = ytd.CurrentYear;
                    report.CurrentSeason = ytd.CurrentSeason;
                    report.SelectedGrades = "K, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12";
                    report.SelectedSubjects = "Math, Science, Other, Non - Academic, Language Arts, History, Foreign Language, Elementary";
                    report.PreviousYearRange = ytd.PreviousYear;
                    report.SessionsPreviousYearComparison = ytd.Sessions;
                    report.QuarterMidYearName = ytd.Quarter;
                }
            }
            finally
            {
                entities.Database.Connection.Close();
            }

            return reportData;
        }

        public Stream GetReportZip(List<ReportModel> reportData, string filePath)
        {
            var outStream = new MemoryStream();
            var generatedFilePath = filePath.Replace("\\Documents\\Report_Template.docx", "") + "\\ReportGenerated.docx";

            try
            {
                using (ZipArchive archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    var countyIds = reportData.Select(p => p.CountyId).Distinct().OrderBy(c => c);
                    foreach (var countyId in countyIds)
                    {
                        var reportCountyData = reportData.Where(r => r.CountyId == countyId).Single();

                        byte[] byteArray = File.ReadAllBytes(filePath);
                        using (MemoryStream mem = new MemoryStream())
                        {
                            mem.Write(byteArray, 0, (int)byteArray.Length);
                            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(mem, true))
                            {

                                #region Header and Body
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#countyschools_ucase]", replace: reportCountyData.CountyName.ToUpper(), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#countyschools_lcase]", replace: reportCountyData.CountyName, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#scholl_total]", replace: reportCountyData.TotalMinutes, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#tutoring_provided]", replace: reportCountyData.TutoringProvided, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#total_sessions]", replace: reportCountyData.SessionsPreviousYearComparison.ToString(), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#ytd_yyyy_yy]", replace: reportCountyData.PreviousYearRange, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#hotline_year_yyyy_yy] ", replace: reportCountyData.PreviousYearRange, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#schoolYear_yyyy_yy]", replace: reportCountyData.CurrentYearRange, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#season_year]", replace: reportCountyData.CurrentSeason, matchCase: false);

                                if (reportCountyData.SelectedGrades != null && reportCountyData.SelectedGrades != "")
                                {
                                    TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#grades_chosen]", replace: "Grade(s): " + reportCountyData.SelectedGrades, matchCase: false);
                                }
                                else
                                {
                                    RemoveTextBoxContentLine(wordDoc, "[#grades_chosen]");
                                }

                                if (reportCountyData.SelectedSubjects != null && reportCountyData.SelectedSubjects != "")
                                {
                                    TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#subjects_chosen]", replace: "Subject(s): " + reportCountyData.SelectedSubjects, matchCase: false);
                                }
                                else
                                {
                                    RemoveTextBoxContentLine(wordDoc, "[#subjects_chosen]");
                                }

                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#firstSemester_yyyy_yy]", replace: reportCountyData.CurrentYearRange, matchCase: false);

                                var relativeModifier = reportCountyData.SessionsPreviousYearComparison < 0 ? "fewer" : "more";
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#hotline_usage]", replace: $"{ Math.Abs(reportCountyData.SessionsPreviousYearComparison)} {relativeModifier}", matchCase: false);

                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#quarter]", replace: reportCountyData.QuarterMidYearName, matchCase: false);

                                #endregion

                                #region Green Total Section
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_through_date]", replace: reportCountyData.ThroughDate, matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_sessions]", replace: string.Format("{0:n0}", reportCountyData.TotalSessions), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_total_sp]", replace: string.Format("{0:n0}", reportCountyData.TotalIndividualStudentsParents), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_stu]", replace: string.Format("{0:n0}", reportCountyData.TotalIndividualStudents), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_par]", replace: string.Format("{0:n0}", reportCountyData.TotalIndividualParents), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_minutes]", replace: string.Format("{0:n0}", reportCountyData.TotalMinutesFreeTutoring), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#green_teacher_posit]", replace: string.Format("{0:n0}", reportCountyData.TotalTeacherPositionsPerWeek), matchCase: false);
                                #endregion

                                #region District Total Section
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_cost_ht]", replace: string.Format("{0:n}", reportCountyData.DistrictTotalCost), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_tutoring_hours]", replace: string.Format("{0:n}", reportCountyData.DistrictTutoringHourCost), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_tutoring_rate]", replace: string.Format("{0:n}", reportCountyData.DistrictTutoringHours), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_st]", replace: string.Format("{0:n0}", reportCountyData.DistrictPromotionalItemStudents), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_st_rate]", replace: string.Format("{0:n}", reportCountyData.DistirctPromotionalItemRate), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_ph]", replace: string.Format("{0:n}", reportCountyData.DistrictPromotionalItemCost), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_ph_percent]", replace: string.Format("{0:n}", reportCountyData.DistrictPhonesPercentOfUsage), matchCase: false);
                                TextReplacer.SearchAndReplace(wordDoc: wordDoc, search: "[#dist_percent_usage]", replace: string.Format("{0:n}", reportCountyData.DistrictPhonesCost), matchCase: false);
                                #endregion

                                #region Charts

                                var dummySeries = new string[] { "dummy" };

                                var studentsSessionsChartData = new ChartData
                                {
                                    SeriesNames = dummySeries,
                                    CategoryDataType = ChartDataType.String,
                                    CategoryNames = reportCountyData.StudentsAndSessions.Select(s => s.ChartElementName).ToArray(),
                                    Values = new double[][] { reportCountyData.StudentsAndSessions.Select(s => s.ChartElementValue).ToArray() }
                                };

                                ChartUpdater.UpdateChart(wordDoc, "Chart1", studentsSessionsChartData);

                                var sessionResultsChartData = new ChartData
                                {
                                    SeriesNames = dummySeries,
                                    CategoryDataType = ChartDataType.String,
                                    CategoryNames = reportCountyData.SessionResults.Select(s => s.ChartElementName).ToArray(),
                                    Values = new double[][] { reportCountyData.SessionResults.Select(s => s.ChartElementValue).ToArray() }
                                };

                                ChartUpdater.UpdateChart(wordDoc, "Chart2", sessionResultsChartData);

                                var subjectBreakdownChartData = new ChartData
                                {
                                    SeriesNames = dummySeries,
                                    CategoryDataType = ChartDataType.String,
                                    CategoryNames = reportCountyData.SubjectBreakdown.Select(s => s.ChartElementName).ToArray(),
                                    Values = new double[][] { reportCountyData.SubjectBreakdown.Select(s => s.ChartElementValue).ToArray() }
                                };

                                ChartUpdater.UpdateChart(wordDoc, "Chart3", subjectBreakdownChartData);

                                var sessionsPerGradeChartData = new ChartData
                                {
                                    SeriesNames = dummySeries,
                                    CategoryDataType = ChartDataType.String,
                                    CategoryNames = reportCountyData.SessionsPerGrade.Select(s => "Grade " + s.ChartElementName).ToArray(),
                                    Values = new double[][] { reportCountyData.SessionsPerGrade.Select(s => s.ChartElementValue).ToArray() }
                                };

                                ChartUpdater.UpdateChart(wordDoc, "Chart4", sessionsPerGradeChartData);

                                #endregion

                                #region Table
                                Body bod = wordDoc.MainDocumentPart.Document.Body;

                                foreach (DocumentFormat.OpenXml.Wordprocessing.Table t in bod.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().Where(tbl => tbl.InnerText.Contains("# of sessions")))
                                {
                                    foreach (var school in reportCountyData.Schools)
                                    {
                                        var tableRow = new OpenXmlElement[] { new DocumentFormat.OpenXml.Wordprocessing.TableRow(new DocumentFormat.OpenXml.Wordprocessing.TableCell(new Paragraph(new Run(new Text(school.Name)))),
                                                                                new DocumentFormat.OpenXml.Wordprocessing.TableCell(new Paragraph(new Run(new Text(school.NumberOfSessions.ToString())))),
                                                                                new DocumentFormat.OpenXml.Wordprocessing.TableCell(new Paragraph(new Run(new Text(school.NumberOfStudents.ToString()))))) };

                                        t.Append(tableRow);
                                    }
                                }
                                #endregion

                                wordDoc.Save();
                                wordDoc.SaveAs(generatedFilePath).Close();
                                wordDoc.Close();
                            }

                        }

                        var zipEntry = archive.CreateEntry("HH Report " + reportCountyData.CountyName + ".docx");
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            var generatedWordDocumentBytes = File.ReadAllBytes(generatedFilePath);
                            var generatedMemoryStream = new MemoryStream();
                            generatedMemoryStream.Write(generatedWordDocumentBytes, 0, (int)generatedWordDocumentBytes.Length);
                            generatedMemoryStream.Seek(0, SeekOrigin.Begin);
                            generatedMemoryStream.CopyTo(zipEntryStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outStream.Close();
            }

            outStream.Position = 0;
            return outStream;
        }

        private void RemoveTextBoxContentLine(WordprocessingDocument wordDoc, string contentText)
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            var textBoxContents = body.Descendants<TextBoxContent>();

            foreach (var textBoxContent in textBoxContents)
            {
                if (textBoxContent.HasChildren)
                {
                    var childElement = textBoxContent.Where(t => t.InnerText == contentText).FirstOrDefault();
                    if (childElement != null)
                    {
                        textBoxContent.RemoveChild(childElement);
                    }
                }
            }
        }

        public int? TryGetInt(string item)
        {
            int i;
            bool success = int.TryParse(item, out i);
            return success ? (int?)i : (int?)null;
        }

        public void Dispose()
        {
            entities.Dispose();
        }

    }
}
