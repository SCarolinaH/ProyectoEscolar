using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolAppV2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SchoolAppV2.Controllers
{
    public class CourseGradeController : Controller
    {
        private readonly SchoolContext context;



        public CourseGradeController(SchoolContext context)
        {
            this.context = context;

     
        }



        public IActionResult Index()
        {

            var grades = context.Grades.ToList();
            grades.Insert(0, new Grade() { Id = 0, Name = "Select an option" });

            var courses = context.Courses.ToList();
            courses.Insert(0, new Course() { Id = 0, Name = "Select an option" });

            ViewData["cbGrades"] = new SelectList(grades, "Id", "Name");
            ViewData["cbCourses"] = new SelectList(courses, "Id", "Name");


            return View();
        }



        // Elimina el CourseGrade en la base de datos
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0) return Json(new { msg = "ID No Found" });

            var delete = await context.CoursesGrades.FindAsync(Id);

            if (delete == null) return Json(new { msg = "ID No Found" });

            try
            {
                context.Remove(delete);
                int result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return Json(new { msg = "The CourseGrade was deleted!" });
                }
            }

            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.InnerException;
                return Json(new { msg = "The CourseGrade was deleted!" });
            }

            return Json(new { msg = "No operation" });


        }





        // Devuelve un listado de CourseGrade
        [HttpGet("/CourseGrade/ListCourseGrade")]
        public async Task<IActionResult> ListCourseGrade()
        {
            var CourseGrades = await context.CoursesGrades.Select(item => new 
            {
                item.Id,
                Grade = item.Grade.Name,
                Course = item.Course.Name,
       

                
            }).ToListAsync();

            return Json(CourseGrades);
        }



        // Crea el Course en la base de datos
        [HttpPost("/CourseGrade/Create")]
        public async Task<IActionResult> Create(CourseGrade courseGrade)

        {

            string message = "";

            if (ModelState.IsValid)
            {
                bool exist = await context.CoursesGrades.AnyAsync(g => g.GradeId == courseGrade.GradeId && g.CourseId == courseGrade.CourseId);
        
                if (exist)
                    message = "The course already exist!";

                else
                {
                    context.Add(courseGrade);     
                    int result = await context.SaveChangesAsync();

                    if (result > 0)
                    {
                        message = "SUCCESS, CourseGrade Ingresado con Exito !!";
                        return Json(new { coursegradeId = courseGrade.Id, msg = message, state = true });
                    }
                }

            }

            message += "Error:\n";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    message += "\t" + error.ErrorMessage + "\n";
                }
            }
            return Json(new { coursegradeId = 0, msg = message, state = false });

        }



        [HttpPost]
        public async Task<IActionResult> GetCourseGrade(int Id)
        {

            if (Id == 0) return Json(new { msg = "ID No Found" });

            CourseGrade coursegrade = await context.CoursesGrades.FindAsync(Id);

            if (coursegrade == null) return Json(new { msg = "ID No Found" });

            return Json(coursegrade);
        }





        // Edita el Course en la base de datos
        [HttpPost]
        public async Task<IActionResult> Edit(CourseGrade courseGrade)
        {
            
            if (courseGrade.Id == 0) return Json(new { state = false, msg = "ID No Found" });

            if (ModelState.IsValid)
            {
                bool exist = await context.CoursesGrades.AnyAsync(g => g.GradeId == courseGrade.GradeId && g.CourseId == courseGrade.CourseId && g.Id != courseGrade.Id);
     
                if (exist)
                    return Json(new { state = false, msg = "The Course already exist!" });
            
                else
                {
                    context.Update(courseGrade);
                    int result = await context.SaveChangesAsync();

                    if (result > 0)
                    {
                        return Json(new { state = true, msg = "The CourseGrade was modified!" });
                    }
                }
            }

            String message = "";
            message += "Error:\n";
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    message += "\t" + error.ErrorMessage + "\n";
                }
            }

            return Json(new { state = false, msg = message });
        }


    }





}















