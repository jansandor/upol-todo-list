using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ZP3CS_projekt.DataClasses;
using Newtonsoft.Json;
using System.IO;
using Path = System.IO.Path;

namespace ZP3CS_projekt.Views
{
    /// <summary>
    /// Interaction logic for AddTask.xaml
    /// </summary>
    public partial class CreateUpdateTaskDialog : Window
    {
        public string NewTaskDescription
        {
            get { return TaskDescription.Text; }
            set { TaskDescription.Text = value; }
        }
        public DateTime? NewTaskDeadline
        {
            get { return DeadlineDatePicker.SelectedDate; }
            set { DeadlineDatePicker.SelectedDate = value; }
        }
        public TimeSpan? NewTaskDeadlineTime { get; set; }

        public CreateUpdateTaskDialog()
        {
            InitializeComponent();
            this.Title = "Add New ToDo Task";
            NewTaskDeadline = DateTime.Today;
        }
        public CreateUpdateTaskDialog(TodoTask t)
        {
            InitializeComponent();
            this.Title = "Edit Task";
            NewTaskDescription = t.Description;
            NewTaskDeadline = t.Deadline;
            DeadlineTime_TextBox.Text = t.DeadlineTime?.ToString("hh':'mm");           
        }

        private void CancelTaskAddition_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void ConfirmTaskAddition_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void TaskDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            TaskDescription.SelectAll();
        }
        private void DeadlineTime_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DeadlineTime_TextBox.SelectAll();
        }
        private void DeadlineTime_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = DeadlineTime_TextBox.Text;
            var ts = new TimeSpan();
            if (TimeSpan.TryParseExact(str, "hh':'mm", null, out ts))
            {
                NewTaskDeadlineTime = ts;
                DeadlineTime_TextBox.Foreground = Brushes.Black;
                ConfirmTaskAddition_btn.IsEnabled = true;
            }
            else if (str == string.Empty)
            {
                NewTaskDeadlineTime = null;
                ConfirmTaskAddition_btn.IsEnabled = true;
            }
            else
            {
                DeadlineTime_TextBox.Foreground = Brushes.Red;
                ConfirmTaskAddition_btn.IsEnabled = false;
            }
        }
    }
}
