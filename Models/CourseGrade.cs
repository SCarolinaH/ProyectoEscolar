using System.ComponentModel.DataAnnotations;

namespace SchoolAppV2.Models
{
    public class CourseGrade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GradeId { get; set; }
        public Grade? Grade { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
