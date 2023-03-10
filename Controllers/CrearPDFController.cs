using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SchoolAppV2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;

namespace SchoolAppV2.Controllers
{
    public class CrearPDFController : Controller
    {

        private readonly SchoolContext context;

        public CrearPDFController(SchoolContext context)
        {
            this.context = context;
        }


        public ActionResult Index(int Id, string Gender, int GradeId)
        {
            List<ReportDTO> reports = new List<ReportDTO>();

            var list = context.Students.Include(s => s.Grade).ToList();

            if (Id != 0)
            {
                list = context.Students.Where(s => s.Id == Id).Include(s => s.Grade).ToList();
            }
            if (!(Gender??"").ToString().IsEmpty())
            {
                list = context.Students.Where(s => s.Gender == Gender).Include(s => s.Grade).ToList();
            }
            if (GradeId != 0)
            {
                list = context.Students.Where(s => s.GradeId == GradeId).Include(s => s.Grade).ToList();
            }

            foreach (var item in list)
            {
                string courses = "";


                var listCourses = context.CoursesGrades.Where(c => c.GradeId == item.GradeId).Include(i => i.Course).ToList();
                foreach (var course in listCourses)
                {
                    courses += course.Course.Name + ", ";
                }
                if (courses != "")
                    courses = courses.Substring(0, courses.Length - 2);


                reports.Add(new ReportDTO()
                {
                    Id = item.Id,
                    StudentCode = item.StudentCode,
                    Name = item.Name,
                    Grade = item.Grade.Name,
                    Courses = courses,
                    Gender = item.Gender,
                    BirthDate = item.BirthDate.ToShortDateString(),
                    Address = item.Address,
                    Phone = item.Phone,
                    Status = item.Status,


                });

            }

            return new ViewAsPdf(reports);
        }


    
    



    }
}
