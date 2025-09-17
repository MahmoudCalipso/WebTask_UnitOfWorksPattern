using WebTask.Entities;

namespace WebTask.DTO
{
    public class TasksDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public EnumPriority Priority { get; set; }
        public EnumStatus Status { get; set; }
        public bool IsOverdue => DateTime.Now > DueDate && Status != EnumStatus.Completed;
        public int DaysUntilDue => (DueDate.Date - DateTime.Now.Date).Days;
    }
}
