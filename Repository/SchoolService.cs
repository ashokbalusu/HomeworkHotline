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

namespace Repository
{

    public class SchoolService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public SchoolService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<SchoolModel> GetAll()
        {
            var result = HttpContext.Current.Session["Schools"] as IList<SchoolModel>;

            if (result == null || UpdateDatabase)
            {
                result = (from school in entities.Schools
                        join userCreatedBy in entities.AspNetUsers on school.CreateBy equals userCreatedBy.Id into uc
                        from userCreatedBy in uc.DefaultIfEmpty()
                        join userModifiedBy in entities.AspNetUsers on school.ModifiedBy equals userModifiedBy.Id into um
                        from userModifiedBy in um.DefaultIfEmpty()
                        select new SchoolModel
                        {
                            SchoolID = school.SchoolID,
                            SchoolName = school.SchoolName,
                            SchoolTypeID = school.SchoolTypeID,
                            SchoolTypeName = school.SchoolType.SchoolTypeName,
                            Address1 = school.Address1,
                            Address2 = school.Address2,
                            City = school.City,
                            State = school.State,
                            Zip = school.Zip,
                            CountyID = school.CountyID,
                            CountyName = school.County.CountyName,
                            Phone = school.Phone,
                            PrincipalName = school.PrincipalName,
                            PrincipalEmail = school.PrincipalEmail,
                            PEDS = school.PEDS,
                            Census = school.Census,
                            PredictedThirdGradeStudents = school.PredictedThirdGradeStudents,
                            SchoolBoardDistrict = school.SchoolBoardDistrict,
                            CityCouncilDistrict = school.CityCouncilDistrict,
                            SonicPartner = school.SonicPartner,
                            TitleOneSchool = school.TitleOneSchool,
                            AppalachianRegion = school.AppalachianRegion,
                            IsDeleted = school.IsDeleted,
                            QuadrantID = school.QuadrantID,
                            CreateDate = school.CreateDate,
                            CreateBy = school.CreateBy,
                            ModifiedDate = school.ModifiedDate,
                            ModifiedBy = school.ModifiedBy,
                            FlashCardQty = school.FlashCardQty,
                            CreatedByFullName = userCreatedBy.FirstName + " " + userCreatedBy.LastName,
                            ModifiedByFullName = userModifiedBy.FirstName + " " + userModifiedBy.LastName,
                        }).Where(s => s.IsDeleted == false).ToList();

                HttpContext.Current.Session["Schools"] = result;
            }

            return result;
        }

        public IEnumerable<SchoolModel> Read()
        {
            return GetAll();
        }

        public IEnumerable<PrincipalModel> GetPrincipalList()
        {
            return GetAllPrincipals();
        }

        private IList<PrincipalModel> GetAllPrincipals()
        {
            var result = (from p in entities.vw_PrincipalList
                          join s in entities.Schools on p.SchoolID equals s.SchoolID
                          where p.PrincipalName != null && s.IsDeleted == false
                          select new PrincipalModel
                          {
                              PrincipalName = p.PrincipalName,
                              SchoolName = p.SchoolName,
                              CountyName = p.CountyName,
                              SchoolTypeName = p.SchoolTypeName,
                              PrincipalEmail = p.PrincipalEmail,
                              Census = p.Census,
                              Phone = p.Phone
                          }).ToList();

            return result;
        }

        public void Create(SchoolModel school, string userId)
        {
            if (!UpdateDatabase)
            {
                var first = GetAll().OrderByDescending(e => e.SchoolID).FirstOrDefault();
                var id = (first != null) ? first.SchoolID : 0;

                school.SchoolID = id + 1;

                GetAll().Insert(0, school);
            }
            else
            {
                var entity = new School();

                entity.SchoolName = school.SchoolName;
                entity.SchoolTypeID = school.SchoolTypeID;
                entity.Address1 = school.Address1;
                entity.Address2 = school.Address2;
                entity.City = school.City;
                entity.State = school.State;
                entity.Zip = school.Zip;
                entity.CountyID = school.CountyID;
                entity.Phone = school.Phone;
                entity.PrincipalName = school.PrincipalName;
                entity.PrincipalEmail = school.PrincipalEmail;
                entity.PEDS = school.PEDS;
                entity.Census = school.Census;
                entity.PredictedThirdGradeStudents = school.PredictedThirdGradeStudents;
                entity.SchoolBoardDistrict = school.SchoolBoardDistrict;
                entity.CityCouncilDistrict = school.CityCouncilDistrict;
                entity.SonicPartner = school.SonicPartner;
                entity.TitleOneSchool = school.TitleOneSchool;
                entity.AppalachianRegion = school.AppalachianRegion;
                entity.IsDeleted = school.IsDeleted;
                entity.QuadrantID = school.QuadrantID;
                entity.CreateDate = DateTime.Now;
                entity.CreateBy = userId;
                entities.Schools.Add(entity);
                entities.SaveChanges();

                school.SchoolID = entity.SchoolID;
            }
        }

        public void Update(SchoolModel school, string userId)
        {

            if (!UpdateDatabase)
            {
                var target = One(e => e.SchoolID == school.SchoolID);

                if (target != null)
                {
                    target.SchoolID = school.SchoolID;
                    target.SchoolName = school.SchoolName;
                    target.SchoolTypeID = school.SchoolTypeID;
                    target.Address1 = school.Address1;
                    target.Address2 = school.Address2;
                    target.City = school.City;
                    target.State = school.State;
                    target.Zip = school.Zip;
                    target.CountyID = school.CountyID;
                    target.Phone = school.Phone;
                    target.PrincipalName = school.PrincipalName;
                    target.PrincipalEmail = school.PrincipalEmail;
                    target.PEDS = school.PEDS;
                    target.Census = school.Census;
                    target.PredictedThirdGradeStudents = school.PredictedThirdGradeStudents;
                    target.SchoolBoardDistrict = school.SchoolBoardDistrict;
                    target.CityCouncilDistrict = school.CityCouncilDistrict;
                    target.SonicPartner = school.SonicPartner;
                    target.TitleOneSchool = school.TitleOneSchool;
                    target.AppalachianRegion = school.AppalachianRegion;
                    target.IsDeleted = school.IsDeleted;
                    target.QuadrantID = school.QuadrantID;
                    target.CreateDate = school.CreateDate;
                    target.CreateBy = school.CreateBy;
                    target.ModifiedDate = DateTime.Now;
                    target.ModifiedBy = userId;
                }
            }
            else
            {
                var entity = new School();

                entity.SchoolID = school.SchoolID;
                entity.SchoolName = school.SchoolName;
                entity.SchoolTypeID = school.SchoolTypeID;
                entity.Address1 = school.Address1;
                entity.Address2 = school.Address2;
                entity.City = school.City;
                entity.State = school.State;
                entity.Zip = school.Zip;
                entity.CountyID = school.CountyID;
                entity.Phone = school.Phone;
                entity.PrincipalName = school.PrincipalName;
                entity.PrincipalEmail = school.PrincipalEmail;
                entity.PEDS = school.PEDS;
                entity.Census = school.Census;
                entity.PredictedThirdGradeStudents = school.PredictedThirdGradeStudents;
                entity.SchoolBoardDistrict = school.SchoolBoardDistrict;
                entity.CityCouncilDistrict = school.CityCouncilDistrict;
                entity.SonicPartner = school.SonicPartner;
                entity.TitleOneSchool = school.TitleOneSchool;
                entity.AppalachianRegion = school.AppalachianRegion;
                entity.IsDeleted = school.IsDeleted;
                entity.QuadrantID = school.QuadrantID;
                entity.CreateDate = school.CreateDate;
                entity.CreateBy = school.CreateBy;
                entity.ModifiedDate = DateTime.Now;
                entity.ModifiedBy = userId;

                entities.Schools.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Destroy(SchoolModel school)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.SchoolID == school.SchoolID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.Schools
                              where s.SchoolID == school.SchoolID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;

                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public SchoolModel One(Func<SchoolModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
            
        public void Dispose()
        {
            entities.Dispose();
        }

    }
}
