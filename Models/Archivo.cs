using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolAppV2.Models
{
    public class Archivo
    {


        [Key]
        public int Id { get; set; }

        [Required]
        public string Fullpath { get; set; }


        [Required]
        public string Name { get; set; }


        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
