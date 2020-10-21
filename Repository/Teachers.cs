using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Repository
{
    public class Teachers
    {
        private static HomeworkHotlineEntities context = new HomeworkHotlineEntities();

        public static IEnumerable<TeacherModel> GetTeacherList()
        {
            //return context.Database.SqlQuery<TeacherModel>("spc_TeacherList");

            try
            {
                var teachers = context.AspNetUsers.Select(s => new TeacherModel
                {
                    UserID = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    PhoneNumber = s.PhoneNumber,
                    Email = s.Email,
                    HireDate = s.HireDate,
                    MNPSEmployeeNo = s.MNPSEmployeeNo,
                    IsDeleted = s.IsDeleted,
                    EndDate = s.EndDate,
                    Volunteer = s.Volunteer,
                    MainSubject = s.MainSubject,
                    SubjectOther = s.SubjectOther,
                    GradeLevel = s.GradeLevel
                }).Where( e => e.IsDeleted == false ).ToList();

                return teachers;
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
    }
}
