using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SchoolAppV2.Models
{

    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StudentCode { get; set; }


        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }


        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public String Address { get; set; }

        [Required]
        [StringLength(100)]
        public String Gender { get; set; }

      
        public bool Status { get; set; }
        


        [Required]
        public int GradeId { get; set; }

        public Grade? Grade { get; set; }

        [StringLength(300)]
        public String Comments { get; set; }

        public List<Archivo> Archivos { get; set; }
    }


}

