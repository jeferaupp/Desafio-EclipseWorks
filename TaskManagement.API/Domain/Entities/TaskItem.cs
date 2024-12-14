namespace TaskManagement.API.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Enums.TaskItemStatus? Status { get; set; }
        public Enums.PriorityLevel Priority { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public List<TaskItemHistory> History { get; set; } = new List<TaskItemHistory>();
        public List<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
