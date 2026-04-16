using StudyPlanner.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using System;
using System.Linq.Expressions;

namespace StudyPlanner.Services
{
    public static class TaskStorageService
    {
        private const string EventFilePath = "tasks_event.json";
        private const string DelegateFilePath = "tasks_delegate.json";
        public static void SaveTasks(ObservableCollection<StudyTask> tasks, bool isEventMode)
        {
            if(tasks == null) return;
            try
            {
                string filePath = isEventMode ? EventFilePath : DelegateFilePath;

                var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Грешка при запис {ex.Message}");
            }
        }

        public static ObservableCollection<StudyTask> LoadTasks(bool isEventMode)
        {
            string filePath = isEventMode ? EventFilePath : DelegateFilePath;

            if (!File.Exists(filePath))
                return new ObservableCollection<StudyTask>();

            try
            {
                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new ObservableCollection<StudyTask>();
                }
                var tasks = JsonSerializer.Deserialize<ObservableCollection<StudyTask>>(json);
                return tasks ?? new ObservableCollection<StudyTask>();
            }
            catch (Exception)
            {
                return new ObservableCollection<StudyTask>();
            }
        }
    }
}
