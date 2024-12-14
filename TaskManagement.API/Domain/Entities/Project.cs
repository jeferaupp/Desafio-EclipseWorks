namespace TaskManagement.API.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();


        // Método para verificar se o projeto pode ser deletado
        public bool CanBeDeleted() => Tasks.All(t => t.Status == Enums.TaskItemStatus.Completed);
    }
}
