namespace TaskManagement.API.Domain.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public TaskItem Task { get; set; }
    }
}
