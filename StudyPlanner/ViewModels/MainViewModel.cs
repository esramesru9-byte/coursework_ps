using StudyPlanner.Models;
using StudyPlanner.Services;

namespace StudyPlanner.ViewModels
{ 
    public class MainViewModel
    {
        public TaskManagerWithEvent EventManager { get; } = new TaskManagerWithEvent();
        public TaskManagerWithDelegate DelegateManager { get; } = new TaskManagerWithDelegate();
        public string NewTitle { get; set; }
        public string NewSubject { get; set; }
        public DateTime NewDeadline { get; set; }
        public string ComparisonResult { get; set; }

        public MainViewModel()
        {
            NewDeadline = DateTime.Now.AddDays(1);
            DelegateManager.Validator = ValidateTaskData;
            EventManager.Validator = ValidateTaskData;
        }

        public bool ValidateTaskData(StudyTask task)
        {
            if (string.IsNullOrWhiteSpace(task.Title) || string.IsNullOrWhiteSpace(task.Subject))
            {
                ComparisonResult = "Грешка: Празни полета!";
                return false;
            }
            if (task.Deadline < DateTime.Today)
            {
                ComparisonResult = "Грешка: Невалидна дата!";
                return false;
            }
            ComparisonResult = "Валидацията успешна.";
            return true;
        }
    }
}