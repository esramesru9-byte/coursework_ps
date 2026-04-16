using StudyPlanner.Models;
using System.Collections.ObjectModel;

namespace StudyPlanner.Services
{
    public delegate bool TaskValidator(StudyTask task);
    public delegate string TaskFormatter(StudyTask task);
    public class TaskManagerWithDelegate
    {
        public ObservableCollection<StudyTask> Tasks { get; set; }
        public TaskNotificationHandler OnNotification;
        public TaskValidator Validator;
        public TaskManagerWithDelegate()
        {
            Tasks = TaskStorageService.LoadTasks(false);
            SortTasksByDeadline();
        }
        public void AddTask(StudyTask task)
        {
            if (task == null) return;

            if (Validator != null && !Validator.Invoke(task))
            {
                OnNotification?.Invoke($"Валидацията пропадна! Задачата не е добавена успешно!");
                return;
            }
                Tasks.Add(task);
                SortTasksByDeadline();
                TaskStorageService.SaveTasks(Tasks, false);
                OnNotification?.Invoke($"Добавена е задача:'{task.Title}' добавена успешно!");
        }
        public void CompleteTask(StudyTask task)
        {
            if (task != null && !task.IsCompleted)
            {
                task.IsCompleted = true;
                TaskStorageService.SaveTasks(Tasks, false);
                OnNotification?.Invoke($"Задача:'{task.Title}' е отбелязана като завършена!");
            }
        }
        public void DeleteTask(StudyTask task)
        {
            if (task != null)
            {
                Tasks.Remove(task);
                TaskStorageService.SaveTasks(Tasks, false);
                OnNotification?.Invoke($"Задача:'{task.Title}' е изтрита успешно!");
            }
        }
        public void SortTasksByDeadline()
        {
            if (Tasks != null)
            {
                Tasks = new ObservableCollection<StudyTask>(Tasks.OrderBy(t => t.Deadline));
            }
        }
        public void ExportTask(StudyTask task, TaskFormatter formatter)
        {
            if (formatter == null || task == null) return;
            string formattedLine = formatter.Invoke(task);
           
        }
    }
    }
