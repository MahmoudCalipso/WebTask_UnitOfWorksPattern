using System.ComponentModel.DataAnnotations;

namespace WebTask.Entities
{
   
    public class Tasks
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }     
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public EnumPriority Priority { get; set; }
        public EnumStatus Status { get; set; } 
        public bool IsDeleted { get; set; }
    }

    public enum EnumPriority
    {
        Low,
        Medium,
        High
    }

    public enum EnumStatus
    {  
        New,
        InProgress,
        Completed,
        Archived
    }
}
