using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Text;
using System.Threading.Tasks;


namespace Repository
{
    public class StudentService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public StudentService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<StudentModel> GetAll()
        {
            var result = HttpContext.Current.Session["Students"] as IList<StudentModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.Students.Select(student => new StudentModel
                {
                    StudentID = student.StudentID,
                    CodeName = student.CodeName.ToUpper(),
                    Grade = student.Grade,
                    HomeLanguageID = student.HomeLanguageID,
                    LanguageName = student.LanguageType.LanguageName,
                    HomeLanguageOther = student.HomeLanguageOther,
                    HasInternet = student.HasInternet,
                    SchoolID = student.SchoolID,
                    SchoolName = student.School.SchoolName,
                    ReferredTypeID = student.ReferredTypeID,
                    ReferredTypeName = student.ReferredType.Name,
                    IsDeleted = student.IsDeleted,
                    GuardianContactPhone = student.GuardianContactNumber,
                    GuardianContactEmail = student.GuardianContactEmail,
                    StudentContactPhone = student.StudentContactNumber,
                    StudentFirstName = student.StudentFirstName,
                    StudentLastName = student.StudentLastName,
                    CountyName = student.School.County.CountyName,
                    CreateDate = student.CreateDate,
                    CreateBy = student.CreateBy,
                    ModifiedDate = student.ModifiedDate,
                    ModifiedBy = student.ModifiedBy,
                    StudentOptOut = student.StudentOptOut,
                    GuardianOptOut = student.GuardianOptOut
                }).Where(p => p.IsDeleted == false).ToList();

                HttpContext.Current.Session["Students"] = result;
            }

            return result;
        }

        public IEnumerable<StudentModel> Read()
        {
            return GetAll();
        }

        public void Create(StudentModel student, string userId)
        {
            if (!UpdateDatabase)
            {
                var first = GetAll().OrderByDescending(e => e.StudentID).FirstOrDefault();
                var id = (first != null) ? first.StudentID : 0;

                GetAll().Insert(0, student);
            }
            else
            {
                var entity = new Student();

                entity.StudentID = student.StudentID;
                entity.CodeName = student.CodeName.ToUpper();
                entity.Grade = student.Grade;
                entity.HomeLanguageID = student.HomeLanguageID;
                entity.HomeLanguageOther = student.HomeLanguageOther;
                entity.HasInternet = student.HasInternet;
                entity.SchoolID = student.SchoolID;
                entity.ReferredTypeID = student.ReferredTypeID;
                entity.IsDeleted = student.IsDeleted;
                entity.GuardianContactNumber = student.GuardianContactPhone;
                entity.GuardianContactEmail = student.GuardianContactEmail;
                entity.StudentContactNumber = student.StudentContactPhone;
                entity.StudentFirstName = student.StudentFirstName;
                entity.StudentLastName = student.StudentLastName;
                entity.CreateDate = DateTime.Now;
                entity.CreateBy = userId;
                entity.StudentOptOut = student.StudentOptOut;
                entity.GuardianOptOut = student.GuardianOptOut;
                entities.Students.Add(entity);
                entities.SaveChanges();
                
                student.StudentID = entity.StudentID;
            }
        }

        public void Update(StudentModel student, string userId)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.StudentID == student.StudentID);

                if (target != null)
                {
                    target.StudentID = student.StudentID;
                    target.CodeName = student.CodeName.ToUpper();
                    target.Grade = student.Grade;
                    target.HomeLanguageID = student.HomeLanguageID;
                    target.HomeLanguageOther = student.HomeLanguageOther;
                    target.HasInternet = student.HasInternet;
                    target.SchoolID = student.SchoolID;
                    target.ReferredTypeID = student.ReferredTypeID;
                    target.IsDeleted = student.IsDeleted;
                    target.GuardianContactPhone = student.GuardianContactPhone;
                    target.GuardianContactEmail = student.GuardianContactEmail;
                    target.StudentContactPhone = student.StudentContactPhone;
                    target.StudentFirstName = student.StudentFirstName;
                    target.StudentLastName = student.StudentLastName;
                    target.CreateDate = student.CreateDate;
                    target.CreateBy = student.CreateBy;
                    target.ModifiedDate = DateTime.Now;
                    target.ModifiedBy = userId;
                    target.StudentOptOut = student.StudentOptOut;
                    target.GuardianOptOut = student.GuardianOptOut;

                }
            }
            else
            {
                var entity = new Student();

                entity.StudentID = student.StudentID;
                entity.CodeName = student.CodeName;
                entity.Grade = student.Grade;
                entity.HomeLanguageID = student.HomeLanguageID;
                entity.HomeLanguageOther = student.HomeLanguageOther;
                entity.HasInternet = student.HasInternet;
                entity.SchoolID = student.SchoolID;
                entity.ReferredTypeID = student.ReferredTypeID;
                entity.IsDeleted = student.IsDeleted;
                entity.GuardianContactNumber = student.GuardianContactPhone;
                entity.GuardianContactEmail = student.GuardianContactEmail;
                entity.StudentContactNumber = student.StudentContactPhone;
                entity.StudentFirstName = student.StudentFirstName;
                entity.StudentLastName = student.StudentLastName;
                entity.CreateDate = student.CreateDate;
                entity.CreateBy = student.CreateBy;
                entity.ModifiedDate = DateTime.Now;
                entity.ModifiedBy = userId;
                entity.StudentOptOut = student.StudentOptOut;
                entity.GuardianOptOut = student.GuardianOptOut;
                entities.Students.Attach(entity);
                entities.Entry(entity).State = EntityState.Modified;
                entities.SaveChanges();
            }
        }

        public void Destroy(StudentModel student)
        {
            if (!UpdateDatabase)
            {
                var target = GetAll().FirstOrDefault(p => p.StudentID == student.StudentID);
                if (target != null)
                {
                    GetAll().Remove(target);
                }
            }
            else
            {
                var target = (from s in entities.Students
                              where s.StudentID == student.StudentID
                              select s).FirstOrDefault();

                if (target != null)
                {
                    target.IsDeleted = true;

                    entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public StudentModel One(Func<StudentModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}
