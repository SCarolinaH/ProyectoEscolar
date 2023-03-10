using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SchoolAppV2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SchoolAppV2.Controllers

{
    public class CourseController : Controller
    {
        private readonly SchoolContext context;

        public CourseController(SchoolContext context)
        {
            this.context = context;
        }


    
        public IActionResult Index()
        {

            return View();

        }



        //// Devuelve un listado de Courses
        [HttpGet("/Course/ListCourse")]
        public async Task<IActionResult> ListCourse()
        {
            var courses = await context.Courses.Select(item => new
            {
                item.Id,
                item.Name

            }).ToListAsync();

            return Json(courses);
        }




        // Crea el Course en la base de datos
        [HttpPost("/Course/Create")]
        public ActionResult Create(Course course)

        {

            string message = "";

            if (ModelState.IsValid)
            {

                  bool exist = context.Courses.Any(g => g.Name.ToUpper().Trim() == course.Name.ToUpper().Trim());

       
                  if (exist)
                      message = "The course already exist!";

                    else
                    {
                        context.Add(course);

                         int result = context.SaveChanges();

                        if (result > 0)
                        {
                            message = "SUCCESS, Course Ingresado con Exito !!";
                            return Json(new { courseId = course.Id, msg = message, state = true });
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
               return Json(new { courseId = 0, msg = message, state = false });


        }



        // Elimina el Course en la base de datos
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0) return Json(new { msg = "ID No Found" });

            var delete = await context.Courses.FindAsync(Id);

            if (delete == null) return Json(new { msg = "ID No Found" });

            try
            {
                context.Remove(delete);
                int result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return Json(new { msg = "The Course was deleted!" });
                }
            }

            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.InnerException;
                return Json(new { msg = "The Course was deleted!" });
            }

            return Json(new { msg = "No operation" });


        }





        [HttpPost]
        public async Task<IActionResult> GetCourse(int Id)
        {
     
            if (Id == 0) return Json(new { msg = "ID No Found" });

            Course course = await context.Courses.FindAsync(Id);
    
            if (course == null) return Json(new { msg = "ID No Found" });

            return Json(course);
        }




        //Edita el Course en la base de datos
       [HttpPost]
        public async Task<IActionResult> Edit(Course course)
        {
            if (course.Id == 0) return Json(new { state = false, msg = "ID No Found" });

            if (ModelState.IsValid)
            {
                              
                bool exist = await context.Courses.AnyAsync(g => g.Name.ToUpper().Trim() == course.Name.ToUpper().Trim() && g.Id != course.Id);


                if (exist)
                    return Json(new { state = false, msg = "The Course already exist!" });
                else
                {
                    context.Update(course);
                    int result = await context.SaveChangesAsync();

                    if (result > 0)
                    {
                        return Json(new { state = true, msg = "The Course was modified!" });
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







