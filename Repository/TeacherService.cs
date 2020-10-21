using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace Repository
{
    public class TeacherService
    {
        private static bool UpdateDatabase = true;
        private HomeworkHotlineEntities entities;

        public TeacherService(HomeworkHotlineEntities entities)
        {
            this.entities = entities;
        }

        public IList<TeacherModel> GetAll()
        {
            var result = HttpContext.Current.Session["Teachers"] as IList<TeacherModel>;

            if (result == null || UpdateDatabase)
            {
                result = entities.AspNetUsers.Select(teacher => new TeacherModel
                {
                    UserID = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    HireDate = teacher.HireDate,
                    PhoneNumber = teacher.PhoneNumber,
                    Email = teacher.Email,
                    MNPSEmployeeNo = teacher.MNPSEmployeeNo,
                    IsDeleted = teacher.IsDeleted,
                    EndDate = teacher.EndDate,
                    Volunteer = teacher.Volunteer,
                    MainSubject = teacher.MainSubject,
                    SubjectOther = teacher.SubjectOther,
                    GradeLevel = teacher.GradeLevel

                }).Where( e => e.IsDeleted == false ).ToList();

                HttpContext.Current.Session["Teachers"] = result;
            }

            return result;
        }
        public TeacherModel GetById(string userId)
        {
            var result = HttpContext.Current.Session["Teachers"] as IList<TeacherModel>;
            if (result == null || UpdateDatabase)
            {
                result = entities.AspNetUsers.Select(t => new TeacherModel
                {
                    UserID = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    HireDate = t.HireDate,
                    PhoneNumber = t.PhoneNumber,
                    Email = t.Email,
                    MNPSEmployeeNo = t.MNPSEmployeeNo,
                    IsDeleted = t.IsDeleted,
                    EndDate = t.EndDate,
                    Volunteer = t.Volunteer,
                    MainSubject = t.MainSubject,
                    SubjectOther = t.SubjectOther,
                    GradeLevel = t.GradeLevel
                }).Where(e => e.UserID == userId).Where(i => i.IsDeleted == false).ToList();

                HttpContext.Current.Session["Teachers"] = result;
            }

            return result.FirstOrDefault();
        }
        public IEnumerable<TeacherModel> Read()
        {
            return GetAll();
        }

        //public void Update(TeacherModel teacher)
        //{

        //    if (!UpdateDatabase)
        //    {
        //        var target = One(e => e.UserID == teacher.UserID);

        //        if (target != null)
        //        {
        //            target.UserID = teacher.UserID;
        //            target.FirstName = teacher.FirstName;
        //            target.LastName = teacher.LastName;
        //            target.HireDate = teacher.HireDate;
        //            target.PhoneNumber = teacher.PhoneNumber;
        //            target.Email = teacher.Email;
        //            target.IsDeleted = teacher.IsDeleted;
        //        }
        //    }
        //    else
        //    {
        //        var entity = new AspNetUser();

        //        entity.Id = teacher.UserID;
        //        entity.FirstName = teacher.FirstName;
        //        entity.LastName = teacher.LastName;
        //        entity.HireDate = teacher.HireDate;
        //        entity.PhoneNumber = teacher.PhoneNumber;
        //        entity.Email = teacher.Email;
        //        entity.IsDeleted = teacher.IsDeleted;

        //        entities.AspNetUsers.Attach(entity);
        //        entities.Entry(entity).State = EntityState.Modified;
        //        entities.SaveChanges();
        //    }
        //}

        //public void Destroy(TeacherModel teacher)
        //{
        //    if (!UpdateDatabase)
        //    {
        //        var target = GetAll().FirstOrDefault(p => p.UserID == teacher.UserID);
        //        if (target != null)
        //        {
        //            GetAll().Remove(target);
        //        }
        //    }
        //    else
        //    {
        //        var target = (from s in entities.AspNetUsers
        //                      where s.Id == teacher.UserID
        //                      select s).FirstOrDefault();

        //        if (target != null)
        //        {
        //            target.IsDeleted = true;
        //            entities.Entry(target).State = System.Data.Entity.EntityState.Modified;
        //            entities.SaveChanges();
        //        }
        //    }
        //}

        public TeacherModel One(Func<TeacherModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}
