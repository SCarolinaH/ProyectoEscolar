using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolAppV2.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<CourseGrade> CoursesGrades { get; set; }

    }
}
