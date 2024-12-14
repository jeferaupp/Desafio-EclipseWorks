namespace TaskManagement.API.Domain.Dtos
{
    public class UserPerformanceReport
    {
        public int UserId { get; set; }
        public int AverageTasksCompleted { get; set; }
    }
}
