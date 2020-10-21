using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Repository
{
    public class Schools
    {
        private static HomeworkHotlineEntities db = new HomeworkHotlineEntities();
        public static IEnumerable<SchoolModel> GetAllSchools(HomeworkHotlineEntities context)
        {

            try
            {
                var schools = context.Schools.Select(s => new SchoolModel
                {
                    SchoolID = s.SchoolID,
                    SchoolName = s.SchoolName,
                    SchoolTypeID = s.SchoolTypeID,
                    SchoolTypeName = s.SchoolType.SchoolTypeName,
                    Address1 = s.Address1,
                    Address2 = s.Address2,
                    City = s.City,
                    State = s.State,
                    Zip = s.Zip,
                    CountyID = s.CountyID,
                    CountyName = s.County.CountyName,
                    Phone = s.Phone,
                    PrincipalName = s.PrincipalName,
                    PrincipalEmail = s.PrincipalEmail,
                    PEDS = s.PEDS,
                    Census = s.Census,
                    PredictedThirdGradeStudents = s.PredictedThirdGradeStudents,
                    SchoolBoardDistrict = s.SchoolBoardDistrict,
                    CityCouncilDistrict = s.CityCouncilDistrict,
                    SonicPartner = s.SonicPartner,
                    TitleOneSchool = s.TitleOneSchool,
                    AppalachianRegion = s.AppalachianRegion,
                    IsDeleted = s.IsDeleted,
                    QuadrantID = s.QuadrantID,
                    CreateDate = s.CreateDate,
                    CreateBy = s.CreateBy,
                    ModifiedDate = s.ModifiedDate,
                    ModifiedBy = s.ModifiedBy
                                               }).Where(s => s.IsDeleted == false).ToList();

                return schools;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
             //   context.Configuration.ProxyCreationEnabled = true;
            }
        }

        public static SchoolModel GetSchoolBySchoolID(int SchoolID)
        {
            School school = (from s in db.Schools
                                  where s.SchoolID == SchoolID
                                  select s).FirstOrDefault();
            SchoolModel sm = school;
            County countyModel = (from c in db.Counties
                                  where c.CountyID == sm.CountyID
                                  select c).FirstOrDefault();

            sm.CountyName = countyModel.CountyName;

            return sm;
        }

        public static IEnumerable<SchoolType> GetAllSchoolTypes()
        {
            try
            {
                using (var db = new HomeworkHotlineEntities())
                {
                    List<SchoolType> schoolTypes = (from s in db.SchoolTypes
                                                    where s.IsDeleted == false
                                                    select s).ToList();
                    return schoolTypes;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
