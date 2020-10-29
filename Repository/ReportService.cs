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

namespace Repository
{

    public class ReportService
    {
        private HomeworkHotlineEntities entities;

        public ReportService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public List<ReportModel> GetReportData(string startDate, string endDate, List<int> countyIds = null)
        {
            var reportData = new List<ReportModel>();
            entities.Database.Initialize(force: false);

            var cmd = entities.Database.Connection.CreateCommand();
            cmd.CommandText = "[dbo].[usp_ReportSummary]";

            var startDateParam = new SqlParameter("StartDate", SqlDbType.VarChar);
            startDateParam.Value = startDate;
            cmd.Parameters.Add(startDateParam);

            var endDateParam = new SqlParameter("EndDate", SqlDbType.VarChar);
            endDateParam.Value = endDate;
            cmd.Parameters.Add(endDateParam);

            if (countyIds != null && countyIds.Count > 0)
            {
                var countyIdTableSchema = new List<SqlMetaData>(1)
                    {
                            new SqlMetaData("CountyId", SqlDbType.Int),
                            new SqlMetaData("CountyName", SqlDbType.VarChar)
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
                    ParameterName = "CountyIds",
                    TypeName = "[dbo].[CountyList]",
                    Value = countyIdTable
                };

                endDateParam.Value = endDate;
                cmd.Parameters.Add(endDateParam);
            }

            try
            {
                entities.Database.Connection.Open();
                var reader = cmd.ExecuteReader();



                reader.NextResult();
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
                    var entityIds = reportData.Select(p => p.EntityId).Distinct().OrderBy(c => c);
                    foreach (var entityId in entityIds)
                    {

                    }
                }
            }
            catch (Exception)
            {
                outStream.Close();
            }

            return outStream;
        }

        public Stream GenerateReportDocX(ReportModel reportData)
        {
            var docxStream = new MemoryStream();
            return docxStream;
        }

        public void Dispose()
        {
            entities.Dispose();
        }

    }
}
