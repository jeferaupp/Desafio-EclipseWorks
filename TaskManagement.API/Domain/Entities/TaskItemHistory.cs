namespace TaskManagement.API.Domain.Entities
{
    public class TaskItemHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Changes { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}
