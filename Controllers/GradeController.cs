using Microsoft.AspNetCore.Mvc;
using SchoolAppV2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SchoolAppV2.Controllers
{
    public class GradeController : Controller
    {
        private readonly SchoolContext context;

        public GradeController(SchoolContext context)
        {
            this.context = context;
        }

     


        public IActionResult Index()
        {
            return View();
        }


        // Elimina el Grade en la base de datos
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0) return Json(new { msg = "ID No Found" });

            var delete = await context.Grades.FindAsync(Id);

            if (delete == null) return Json(new { msg = "ID No Found" });

            try
            {
                context.Remove(delete);
                int result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return Json(new { msg = "The Grade was deleted!" });
                }
            }

            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.InnerException;
                return Json(new { msg = "The Grade was deleted!" });
            }

            return Json(new { msg = "No operation" });


        }







        //// Devuelve un listado de Grades
        [HttpGet("/Grade/ListGrade")]
        public async Task<IActionResult> ListGrade()
        {
            var grades = await context.Grades.Select(item => new
            {
                item.Id,
                item.Name

            }).ToListAsync();

            return Json(grades);
        }




        // Crea el Grade en la base de datos
        [HttpPost("/Grade/Create")]
        public ActionResult Create(Grade grade)

        {

            string message = "";

            if (ModelState.IsValid)
            {

                bool exist = context.Grades.Any(g => g.Name.ToUpper().Trim() == grade.Name.ToUpper().Trim());


                if (exist)
                    message = "The grade already exist!";

                else
                {
                    context.Add(grade);

                    int result = context.SaveChanges();

                    if (result > 0)
                    {
                        message = "SUCCESS, Grade Ingresado con Exito !!";
                        return Json(new { gradeId = grade.Id, msg = message, state = true });
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
            return Json(new { gradeId = 0, msg = message, state = false });


        }


        [HttpPost]
        public async Task<IActionResult> GetGrade(int Id)
        {

            if (Id == 0) return Json(new { msg = "ID No Found" });

            Grade grade = await context.Grades.FindAsync(Id);

            if (grade == null) return Json(new { msg = "ID No Found" });

            return Json(grade);
        }




        //Edita el Grade en la base de datos
        [HttpPost]
        public async Task<IActionResult> Edit(Grade grade)
        {
            if (grade.Id == 0) return Json(new { state = false, msg = "ID No Found" });

            if (ModelState.IsValid)
            {

                bool exist = await context.Grades.AnyAsync(g => g.Name.ToUpper().Trim() == grade.Name.ToUpper().Trim() && g.Id != grade.Id);


                if (exist)
                    return Json(new { state = false, msg = "The Grade already exist!" });
                else
                {
                    context.Update(grade);
                    int result = await context.SaveChangesAsync();

                    if (result > 0)
                    {
                        return Json(new { state = true, msg = "The Grade was modified!" });
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
