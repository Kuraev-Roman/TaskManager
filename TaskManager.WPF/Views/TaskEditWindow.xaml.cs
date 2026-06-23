using System;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Core.Models;

namespace TaskManager.WPF.Views
{
    public partial class TaskEditWindow : Window
    {
        private readonly TaskItem _task;
        public TaskItem? Result { get; private set; }

        public TaskEditWindow(TaskItem task)
        {
            InitializeComponent();
            _task = task;
            LoadFields();
        }

        private void LoadFields()
        {
            TxtTitle.Text = _task.Title;
            TxtDescription.Text = _task.Description;
            DpDueDate.SelectedDate = _task.DueDate;
            ChkImportant.IsChecked = _task.IsImportant;

            foreach (ComboBoxItem item in CmbPriority.Items)
                if ((TaskPriority)item.Tag == _task.Priority)
                { CmbPriority.SelectedItem = item; break; }
            if (CmbPriority.SelectedItem is null) CmbPriority.SelectedIndex = 1;

            foreach (ComboBoxItem item in CmbStatus.Items)
                if ((TaskStatus)item.Tag == _task.Status)
                { CmbStatus.SelectedItem = item; break; }
            if (CmbStatus.SelectedItem is null) CmbStatus.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Введите название задачи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTitle.Focus();
                return;
            }

            Result = new TaskItem
            {
                Id = _task.Id,
                CreatedAt = _task.CreatedAt,
                Title = TxtTitle.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Priority = (TaskPriority)((ComboBoxItem)CmbPriority.SelectedItem).Tag,
                DueDate = DpDueDate.SelectedDate ?? DateTime.Today.AddDays(1),
                Status = (TaskStatus)((ComboBoxItem)CmbStatus.SelectedItem).Tag,
                IsImportant = ChkImportant.IsChecked == true
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;
    }
}