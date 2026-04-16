namespace StudyPlanner.Models
{
    public class StudyTask
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }
    }
}
