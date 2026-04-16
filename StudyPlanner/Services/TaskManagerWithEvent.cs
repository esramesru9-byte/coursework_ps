using StudyPlanner.Models;
using System.Collections.ObjectModel;

namespace StudyPlanner.Services
{
    public class TaskManagerWithEvent
    {
        public ObservableCollection<StudyTask> Tasks { get; set; }
        public event TaskNotificationHandler OnNotification;

        public TaskManagerWithEvent()
        {
            Tasks = TaskStorageService.LoadTasks(true);
            SortTasksByDeadline();
        }

        public TaskValidator Validator;

        public void AddTask(StudyTask task)
        {
            if (task == null) return;

         
            if (Validator != null && !Validator.Invoke(task))
            {
                OnNotification?.Invoke("Валидацията в Event Mode пропадна! Задачата не е добавена.");
                return;
            }

            Tasks.Add(task);
            SortTasksByDeadline();
            TaskStorageService.SaveTasks(Tasks, true);
            OnNotification?.Invoke($"Задача: '{task.Title}' е добавена успешно!");
        }

        public void CompleteTask(StudyTask task)
        {
            if (task != null && !task.IsCompleted)
            {
                task.IsCompleted = true;
                TaskStorageService.SaveTasks(Tasks, true);
                OnNotification?.Invoke($"Задача:'{task.Title}' е отбелязана като завършена!");
            }
        }

        public void DeleteTask(StudyTask task)
        {
            if (task != null)
            {
                Tasks.Remove(task);
                TaskStorageService.SaveTasks(Tasks, true);
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
    }
 }
        
    

