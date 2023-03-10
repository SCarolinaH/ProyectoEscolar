using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolAppV2.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<CourseGrade> CoursesGrades { get; set; }


        public List<Student> Students { get; set; }
    }
}
