using System.ComponentModel.DataAnnotations;
using WebTask.Entities;

namespace WebTask.DTO
{
    public class CreateTasksDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public EnumPriority Priority { get; set; } = EnumPriority.Medium;
        public EnumStatus Status { get; set; }
    }
}
