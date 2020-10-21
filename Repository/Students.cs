using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Repository
{
    public class Students
    {
        private static HomeworkHotlineEntities context = new HomeworkHotlineEntities();

        public static Student GetStudentByStudentID(int StudentID)

        {
            Student student = (from s in context.Students
                                         where s.StudentID == StudentID
                                         select s).FirstOrDefault();

            return student; 
        }
    }
}
