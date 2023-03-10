using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolAppV2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Grpc.Core;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Text.Json;
using NLog.LayoutRenderers;

namespace SchoolAppV2.Controllers
{

    public class Gender
    {
        public string Name { get; set; }
    }

    public class StudentController : Controller

    {
       
        private readonly SchoolContext context;

        private readonly IHostingEnvironment _hostingEnvironment;

        private List<Gender> genders = new List<Gender>();


        public StudentController(SchoolContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;

            _hostingEnvironment = hostingEnvironment;

            genders.Add(new Gender() { Name = "Select an option" });
            genders.Add(new Gender() { Name = "Masculino" });
            genders.Add(new Gender() { Name = "Femenino" });

        }

       
        #region Student:Ajax_Image_Upload

        public IActionResult student_start()
        {
            return View("Student/UploadFilesAjax");
        }

        [HttpPost]
        public IActionResult ajax_Image_Upload_post(IFormCollection formFields)

        {
            return View("Student/UploadFilesAjax");

        }

        [HttpPost]
        public async Task<IActionResult> obtenerImagesDB(FileUpdate fileUpdate)
        {
            List<Archivo> lista = new List<Archivo>();
            try
            {
                lista = await context.Archivos.Where(a => a.StudentId == fileUpdate.StudentId).ToListAsync();
                
                return Json(new { images = lista });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }

            return Json(new { images = lista });
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarArchivosDB(FileUpdate fileUpdate)
        {
            
            try
            {
                
                foreach(var item in fileUpdate.FilesServers)
                {
                    Archivo archivo = new Archivo() { 
                        Fullpath = item.Fullpath,
                        Name = item.Name,
                        StudentId = fileUpdate.StudentId,
                    };

                    context.Add(archivo);
                }

                int result = context.SaveChanges();

                if (result > 0)
                {
                    return Json(new { message = "Updated" });
                }
            }
            catch(Exception ex)
            {
                return Json(new { message = ex.Message });
            }
            return Json(new { message = "Finish" });
        }


        [HttpPost]
        public IActionResult DeleteFile(FileServer archivo)
        {
            try
            {
                System.IO.File.Delete(archivo.Fullpath);
            }
            catch(Exception ex)
            {
                return Json(new { message = ex.Message });
            }

            return Json(new { message = "Deleted" });
        }

      //  [HttpGet("Descarga de archivo")]





        [HttpPost]
        public async Task<IActionResult> UploadFilesAjax()

        {

            List<FileServer> files = new List<FileServer>();
       
            string path_for_Uploaded_Files = _hostingEnvironment.WebRootPath + "\\User_Files\\_Upload_Folder\\";

            var uploaded_files = Request.Form.Files;

            int iCounter = 0;
          
            foreach (var uploaded_file in uploaded_files)
            {
               
                String newName = Guid.NewGuid().ToString() + Path.GetExtension(uploaded_file.FileName);

                string uploaded_Filename = newName;

                string new_Filename_on_Server = path_for_Uploaded_Files + uploaded_Filename;

                using (FileStream stream = new FileStream(new_Filename_on_Server, FileMode.Create))
                {
                    await uploaded_file.CopyToAsync(stream);
                    files.Add(new FileServer
                    {
                        Fullpath = new_Filename_on_Server.Replace(_hostingEnvironment.WebRootPath+"\\", ""),
                        Name = uploaded_Filename,
                        OriginalName = uploaded_file.FileName
                    });
                }

            }
          
           return Json(files);

        }
        #endregion
        
         public IActionResult Index()
         {
            var grades = context.Grades.ToList();
            grades.Insert(0, new Grade() { Id = 0, Name = "Select an option" });

            ViewData["cbGrades"] = new SelectList(grades, "Id", "Name");
            ViewData["cbGenders"] = new SelectList(genders, "Name", "Name");

            return View();
        }


        // Devuelve un listado de Estudiantes
        [HttpGet("/Student/ListStudents")]
        public async Task<IActionResult> ListStudents()
        {
            var students = await context.Students.Select(item => new { 
                item.Id,
                item.StudentCode,
                item.Name,
                item.Phone,
                item.Gender,
                Grade = item.Grade.Name,
                item.Comments,
                BirthDate = item.BirthDate.ToShortDateString(),
                item.Address,
                item.Status

            }).ToListAsync();

            
            return Json(students);
        }
 



        // Crea el estudiante en la base de datos
        [HttpPost("/Student/Create")]
        public async Task<IActionResult> Create(Student student)
        {
            string message = "";

            if (ModelState.IsValid)
            {
             
                bool exist = await context.Students.AnyAsync(g => g.StudentCode.ToUpper().Trim() == student.StudentCode.ToUpper().Trim());
        
                if (exist)
                    message = "The student already exist!";
               
                else
                {  
                    context.Add(student);
                   
                    int result =context.SaveChanges();

                    if (result > 0)
                    {
                        message = "SUCCESS, Estudiante Ingresado con Exito !!";
                        return Json(new { studentId= student.Id, msg = message, state=true });
                    }
                } 
            }
            

            message += "Error:\n";
            foreach (var modelState in ModelState.Values)
            {
                foreach(var error in modelState.Errors)
                {
                    message += "\t" + error.ErrorMessage + "\n";
                }
            }
            return Json(new { studentId = 0, msg = message, state=false });
        }


        // Elimina el Estudiante en la base de datos
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if (Id == 0) return Json(new { msg = "ID No Found" });

            var delete = await context.Students.FindAsync(Id);

            if (delete == null) return Json(new { msg = "ID No Found" });

            try
            {
                context.Remove(delete);
                int result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    return Json(new {msg = "The student was deleted!" });
                }

                          
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.InnerException;
                return Json(new { msg = "The student cannot be deleted as it contains references.!" });
            }

            return Json(new { msg = "No operation" });
        }




        [HttpPost]
        public async Task<IActionResult> GetStudent(int Id)
        {
            if (Id == 0) return Json(new { msg = "ID No Found" });

            Student student = await context.Students.FindAsync(Id);

            if (student == null) return Json(new { msg = "ID No Found" });

            return Json(student);
        }




        // Edita el estudiante en la base de datos
        [HttpPost]
        public async Task<IActionResult> Edit(Student student)
        {
            if (student.Id == 0) return Json(new {state=false, msg = "ID No Found" });

            if (ModelState.IsValid)
            {
                bool exist = await context.Students.AnyAsync(g => g.StudentCode.ToUpper().Trim() == student.StudentCode.ToUpper().Trim() && g.Id != student.Id);

                if (exist)
                    return Json(new {state=false, msg = "The student already exist!" });
                else
                {
                    context.Update(student);
                    int result = await context.SaveChangesAsync();

                    if (result > 0)
                    {
                        return Json(new {state=true, msg = "The student was modified!" });
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

            return Json(new {state=false, msg = message });
        }


    }
}





