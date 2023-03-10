using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolAppV2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages;
using System.Xml.Linq;

namespace SchoolAppV2.Controllers
{
    public class  FilterStudent
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string StudentCode { get; set; }


    }


    public class FilterGender
    {
        [Required]
        [StringLength(100)]
        public String Name { get; set; }

    }

    public class FilterGrade
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

    }



    public class ReportController : Controller
    {
        private readonly SchoolContext context;

        private List<FilterStudent> filters = new List<FilterStudent>();

        private List<FilterGender> fGender = new List<FilterGender>();

        private List<FilterGrade> fGrade = new List<FilterGrade>();




        // Devuelve un listado de Estudiantes
        [HttpGet("/Report/ListStudentsReport")]
        public async Task<IActionResult> ListStudentsReport(FiltroReportDTO filtro)
        {
            List<ReportDTO> reports = new List<ReportDTO>();

            var list = context.Students.Include(s => s.Grade).ToList();

            if(filtro.Id != 0)
            {
                list = context.Students.Where(s => s.Id==filtro.Id).Include(s => s.Grade).ToList();
            }
            if (!filtro.Gender.IsEmpty())
            {
                list = context.Students.Where(s => s.Gender == filtro.Gender).Include(s => s.Grade).ToList();
            }
            if (filtro.GradeId != 0)
            {
                list = context.Students.Where(s => s.GradeId == filtro.GradeId).Include(s => s.Grade).ToList();
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

            return Json(reports);
        }







        public ReportController(SchoolContext context)
        {
            this.context = context;
            filters.Add(new FilterStudent() { Id = 0, Name = "Select an option", StudentCode = "Select an option" });

            fGender.Add(new FilterGender() { Name = "Select an option" });
            fGender.Add(new FilterGender() { Name = "Masculino" });
            fGender.Add(new FilterGender() { Name = "Femenino" });

            fGrade.Add(new FilterGrade() { Id = 0, Name = "Select an option" });           

 
            var students = context.Students.ToList();
            foreach (var item in students)
            {
                filters.Add(new FilterStudent() { Id = item.Id, Name = item.Name, StudentCode = item.StudentCode });


            }


            var grades = context.Grades.ToList();
            foreach (var item in grades)
            {
                
                fGrade.Add(new FilterGrade() { Id = item.Id, Name = item.Name });

            }

        }




        public IActionResult Index()
        {
            
            List<Gender> genders = new List<Gender>();


            var list = context.Students.Include(s => s.Grade).ToList();


            ViewData["cbStudents"] = new SelectList(filters, "Id", "Name");
            TempData["ID"] = 0;

            ViewData["cbStudents1"] = new SelectList(filters, "Id",  "StudentCode");
            TempData["StudentCode"] = 0;




            ViewData["cbGenders"] = new SelectList(fGender, "Name", "Name");

            ViewData["cbGrades"] = new SelectList(fGrade, "Id", "Name");

       

            return View();
        }
    }
}






