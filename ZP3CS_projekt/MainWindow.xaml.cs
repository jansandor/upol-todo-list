using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Newtonsoft.Json;
using ZP3CS_projekt.DataClasses;
using Path = System.IO.Path;
using ZP3CS_projekt.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZP3CS_projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _todoTasksJSONFilePath = Path.Combine(AppContext.BaseDirectory, "todo_tasks.json");
        private ObservableCollection<TodoTask> _todoTasks = new ObservableCollection<TodoTask>();
        private ObservableCollection<TodoTask> _finishedTasks = new ObservableCollection<TodoTask>();
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(_todoTasksJSONFilePath) == false)
            {
                FileStream fs = File.Create(_todoTasksJSONFilePath);
                var emptyJsonCollection = Encoding.UTF8.GetBytes("[]");
                fs.Write(emptyJsonCollection);
                fs.Close();
            }
            LoadJsonToCollections();
        }

        private void TodoTaskListChanged()
        {
            EmptyTodoTaskList_label.Visibility = _todoTasks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            EnsureTodoTasksFilteringAndOrder();
        }
        private void FinishedTaskListChanged()
        {
            EmptyFinishedTaskList_label.Visibility = _finishedTasks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            FinishedTask_ListView.ItemsSource = _finishedTasks.OrderByDescending(t => t.Finished);
        }

        private void LoadJsonToCollections()
        {
            using (var sr = new StreamReader(_todoTasksJSONFilePath))
            {
                var content = sr.ReadToEnd();
                var collection = JsonConvert.DeserializeObject<ObservableCollection<TodoTask>>(content);
                foreach (var item in collection)
                {
                    if(item.Finished == null)
                    {
                        _todoTasks.Add(item);
                    }
                    else
                    {
                        _finishedTasks.Add(item);
                    }
                }
            }
            TodoTaskListChanged();
            FinishedTaskListChanged();
        }
        private void SaveCollectionsToJson()
        {
            var collection = _todoTasks.Concat(_finishedTasks);
            var json = JsonConvert.SerializeObject(collection);
            using (var sw = new StreamWriter(_todoTasksJSONFilePath))
            {
                sw.Write(json);
            }
        }

        private void AddTask_OnClick(object sender, RoutedEventArgs e)
        {
            CreateUpdateTaskDialog addTaskDialog = new CreateUpdateTaskDialog();
            if (addTaskDialog.ShowDialog() == true)
            {
                _todoTasks.Add(new TodoTask(addTaskDialog.NewTaskDescription, addTaskDialog.NewTaskDeadline, addTaskDialog.NewTaskDeadlineTime));
                SaveCollectionsToJson();
                TodoTaskListChanged();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskFilter_ComboBox.IsEnabled = TodoTaskListPage.IsSelected;
            MarkTaskFinished_btn.IsEnabled = FinishedTaskListPage.IsSelected ? false : true;
            EditTask_btn.IsEnabled = FinishedTaskListPage.IsSelected ? false : true;
        }

        private void EditTaskCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TodoTask_ListView != null && TodoTask_ListView.SelectedItem != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void EditTaskCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TodoTask_ListView.SelectedItem as TodoTask;
            CreateUpdateTaskDialog updateTaskDialog = new CreateUpdateTaskDialog(item);
            if (updateTaskDialog.ShowDialog() == true)
            {
                _todoTasks.Single(t => t.Equals(item)).UpdateTodoTask(
                            updateTaskDialog.NewTaskDescription, updateTaskDialog.NewTaskDeadline, updateTaskDialog.NewTaskDeadlineTime, item.ProgressValue);
                SaveCollectionsToJson();
                TodoTaskListChanged();
            }
        }

        private void RemoveTaskCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TodoTask_ListView != null && TodoTask_ListView.SelectedItem != null && TodoTaskListPage.IsSelected)
            {
                e.CanExecute = true;
            }
            else if (FinishedTask_ListView != null && FinishedTask_ListView.SelectedItem != null && FinishedTaskListPage.IsSelected)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void RemoveTaskCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TodoTaskListPage.IsSelected)
            {
                var item = TodoTask_ListView.SelectedItem as TodoTask;
                _todoTasks.Remove(item);
                SaveCollectionsToJson();
                TodoTaskListChanged();
            }
            else if (FinishedTaskListPage.IsSelected)
            {
                var item = FinishedTask_ListView.SelectedItem as TodoTask;
                _finishedTasks.Remove(item);
                SaveCollectionsToJson();
                FinishedTaskListChanged();
            }
        }

        private void MarkTaskAsFinishedCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TodoTask_ListView != null && TodoTask_ListView.SelectedItem != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void MarkTaskAsFinishedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = TodoTask_ListView.SelectedItem as TodoTask;
            _todoTasks.Remove(item);
            TodoTaskListChanged();

            item.MarkAsFinished();
            _finishedTasks.Add(item);
            SaveCollectionsToJson();            
            FinishedTaskListChanged();
        }

        private void TaskProgress_DecrCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void TaskProgress_DecrCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = e.Parameter as int?;
            _todoTasks.FirstOrDefault(t => t.ID == id).ProgressDecr();
            SaveCollectionsToJson();
            TodoTaskListChanged();
        }

        private void TaskProgress_IncrCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void TaskProgress_IncrCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = e.Parameter as int?;
            var t = _todoTasks.FirstOrDefault(t => t.ID == id);
            t.ProgressIncr();
            TodoTaskListChanged();
            if (t.ProgressValue == t.MaxProgressValue)
            {
                var dialogResult = MessageBox.Show("Do you want to mark task as finished?", "Task is done!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    _todoTasks.Remove(t);
                    TodoTaskListChanged();

                    t.MarkAsFinished();
                    _finishedTasks.Add(t);
                    FinishedTaskListChanged();
                }
            }
            SaveCollectionsToJson();
        }

        private void EnsureTodoTasksFilteringAndOrder()
        {
            if (TodoTask_ListView == null)
            {
                return;
            }
            if (TaskFilterOption_AllTasks.IsSelected)
            {
                TodoTask_ListView.ItemsSource = _todoTasks.OrderBy(t => t.Deadline).ThenBy(t => t.DeadlineTime);
            }
            else if (TaskFilterOption_TodaysTasks.IsSelected)
            {
                TodoTask_ListView.ItemsSource = _todoTasks.Where(t => t.Deadline == DateTime.Today).OrderBy(t => t.Deadline).ThenBy(t => t.DeadlineTime);
            }
            else if (TaskFilterOption_TomorrowsTasks.IsSelected)
            {
                TodoTask_ListView.ItemsSource = _todoTasks.Where(t => t.Deadline == DateTime.Today.AddDays(1)).OrderBy(t => t.Deadline).ThenBy(t => t.DeadlineTime);
            }
        }
        private void TaskFilter_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnsureTodoTasksFilteringAndOrder();
        }
    }
}
