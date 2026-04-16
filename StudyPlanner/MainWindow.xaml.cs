using StudyPlanner.Models;
using StudyPlanner.Services;
using System.Windows;
using System.Windows.Controls;
using StudyPlanner.ViewModels;

namespace StudyPlanner
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private TaskManagerWithEvent eventManager;
        private TaskManagerWithDelegate delegateManager;

        private bool useEventMode = true;

        public MainWindow()
        {
            viewModel = new MainViewModel();
            eventManager = viewModel.EventManager;
            delegateManager = viewModel.DelegateManager;

            InitializeComponent();

            if (eventManager != null)
            {
                eventManager.OnNotification += ShowNotification;
                eventManager.Validator = viewModel.ValidateTaskData;
            }

            if (delegateManager != null)
            {
                delegateManager.OnNotification = ShowNotification;
                delegateManager.Validator = viewModel.ValidateTaskData;
            }

            useEventMode = true;
            RefreshGridBinding();
        }

        private void ShowNotification(string message)
        {
            if (NotificationsListBox == null)
                return;
            NotificationsListBox.Items.Add($"{DateTime.Now:T} - {message}");
        }

        /* private ObservableCollection<StudyTask> CurrentTasks
        {
           get
            {
                return useEventMode ? eventManager.Tasks : delegateManager.Tasks;
            }
        } */

        private void RefreshGridBinding()
        {
            
            if (eventManager == null || delegateManager == null) return;

            var tasks = useEventMode ? eventManager.Tasks : delegateManager.Tasks;
            if (tasks == null) return;

            TasksGrid.ItemsSource = tasks.Where(t => !t.IsCompleted).ToList();
            CompletedTasksGrid.ItemsSource = tasks.Where(t => t.IsCompleted).ToList();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(SubjectTextBox.Text) ||
                DeadlinePicker.SelectedDate == null)
            {
                MessageBox.Show("Моля попълнете всички полета.");
                return;
            }

            StudyTask task = new StudyTask
            {
                Title = TitleTextBox.Text,
                Subject = SubjectTextBox.Text,
                Deadline = DeadlinePicker.SelectedDate.Value,
                IsCompleted = false
            };

            if (useEventMode)
                eventManager.AddTask(task);
            else
                delegateManager.AddTask(task);

            TitleTextBox.Clear();
            SubjectTextBox.Clear();
            DeadlinePicker.SelectedDate = null;

            RefreshGridBinding();
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            StudyTask selectedTask = TasksGrid.SelectedItem as StudyTask;

            if (selectedTask == null)
            {
                MessageBox.Show("Избери активна задача.");
                return;
            }

            if (useEventMode)
                eventManager.CompleteTask(selectedTask);
            else
                delegateManager.CompleteTask(selectedTask);

            RefreshGridBinding();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            StudyTask selectedTask = TasksGrid.SelectedItem as StudyTask;

            if (selectedTask == null)
            {
                selectedTask = CompletedTasksGrid.SelectedItem as StudyTask;
            }
            if (selectedTask == null)
            {
                MessageBox.Show("Избери задача за изтриване.");
                return;
            }

            if (useEventMode)
                eventManager.DeleteTask(selectedTask);
            else
                delegateManager.DeleteTask(selectedTask);

            RefreshGridBinding();
        }


        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                useEventMode = selectedItem.Content.ToString() == "Event Mode";
                ShowNotification("Сменен режим: " + selectedItem.Content.ToString());
                RefreshGridBinding();
            }
        }

        private void BreakDelegate_Click(object sender, RoutedEventArgs e)
        {
            if (useEventMode)
            {
                MessageBox.Show("В Event Mode не можеш да занулиш event-а отвън.");
                return;
            }

            delegateManager.OnNotification = null;
            ShowNotification("Delegate беше занулен отвън.");
        }
        private void ExportTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksGrid.SelectedItem as StudyTask;

            if (selectedTask == null)
            {
                MessageBox.Show("Моля, изберете задача от списъка с активни задачи!");
                return;
            }

            
            if (ModeComboBox.SelectedIndex == 1) 
            {
            
                delegateManager.ExportTask(selectedTask, (task) =>
                {
                    string formatted = $"*** ОТЧЕТ ЗА ЗАДАЧА ***\n" +
                                       $"Заглавие: {task.Title}\n" +
                                       $"Предмет: {task.Subject}\n" +
                                       $"Статус: {(task.IsCompleted ? "Завършена" : "Активна")}";

                    MessageBox.Show(formatted, "Експорт (Delegate Callback)");
                    return formatted; 
                });
            }
            else
            {
                MessageBox.Show("Тази функционалност е реализирана само за Delegate Mode, за да се покаже Return Value свойството на делегатите.");
            }
        }
    }
}