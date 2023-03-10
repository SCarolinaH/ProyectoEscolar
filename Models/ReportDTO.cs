using System.ComponentModel.DataAnnotations;
using System;

namespace SchoolAppV2.Models
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string Courses { get; set; }

        [Required]
        [StringLength(100)]
        public String Gender { get; set; }

        [Required]
        public String BirthDate { get; set; }


        [Required]
        public String Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        public bool Status { get; set; }
    }
}
