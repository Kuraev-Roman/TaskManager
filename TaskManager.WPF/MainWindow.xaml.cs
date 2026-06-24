using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.WPF
{
    public partial class MainWindow : Window
    {
        private readonly TaskService _service = new();
        private readonly FileService _fileService = new();

        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Views.TaskEditWindow(new TaskItem());
            if (dlg.ShowDialog() == true)
            {
                _service.Add(dlg.Result!);
                Refresh();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (TasksList.SelectedItem is not TaskItem selected)
            {
                MessageBox.Show("Выберите задачу для редактирования.", "Редактирование",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dlg = new Views.TaskEditWindow(Clone(selected));
            if (dlg.ShowDialog() == true)
            {
                _service.Update(dlg.Result!);
                Refresh();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (TasksList.SelectedItem is not TaskItem selected)
            {
                MessageBox.Show("Выберите задачу для удаления.", "Удаление",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить «{selected.Title}»?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _service.Delete(selected.Id);
                Refresh();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "JSON|*.json", FileName = "tasks.json" };
            if (dlg.ShowDialog() == true)
            {
                _fileService.SaveToFile(_service.GetAll().ToList(), dlg.FileName);
                MessageBox.Show("Сохранено!", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "JSON|*.json" };
            if (dlg.ShowDialog() == true)
            {
                var tasks = _fileService.LoadFromFile(dlg.FileName);
                foreach (var t in tasks) _service.Add(t);
                Refresh();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => Refresh();
        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => Refresh();

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            Refresh();
        }

        private void Refresh()
        {
            if (TasksList == null) return;

            var query = (StatusFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "Новая" => _service.FilterByStatus(TaskStatus.New),
                "В процессе" => _service.FilterByStatus(TaskStatus.InProgress),
                "Завершена" => _service.FilterByStatus(TaskStatus.Completed),
                _ => _service.GetAll()
            };

            string search = SearchBox?.Text ?? "";
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t =>
                    t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

            query = (SortCombo?.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "Приоритет ↓" => query.OrderByDescending(t => t.Priority),
                "Приоритет ↑" => query.OrderBy(t => t.Priority),
                "Срок ↑" => query.OrderBy(t => t.DueDate),
                "Срок ↓" => query.OrderByDescending(t => t.DueDate),
                _ => query
            };

            TasksList.ItemsSource = query.ToList();
            UpdateStats();
        }

        private void UpdateStats()
        {
            var all = _service.GetAll().ToList();
            StatsTotal.Text = $"Всего: {all.Count}";
            StatsNew.Text = $"Новых: {all.Count(t => t.Status == TaskStatus.New)}";
            StatsInProgress.Text = $"В работе: {all.Count(t => t.Status == TaskStatus.InProgress)}";
            StatsCompleted.Text = $"Завершено: {all.Count(t => t.Status == TaskStatus.Completed)}";
            StatsOverdue.Text = $"Просрочено: {all.Count(t => t.IsOverdue)}";
            StatsImportant.Text = $"Важных: {all.Count(t => t.IsImportant)}";
        }

        private static TaskItem Clone(TaskItem s) => new()
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            Priority = s.Priority,
            DueDate = s.DueDate,
            Status = s.Status,
            IsImportant = s.IsImportant,
            CreatedAt = s.CreatedAt
        };
    }
}