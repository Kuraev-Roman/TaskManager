using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.WPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ITaskService _service;

        private string _searchQuery = string.Empty;
        private string _statusFilter = "Все";
        private string _sortMode = "По умолчанию";
        private TaskItem? _selected;
        private TaskStatistics _stats = new();

        public MainWindow()
        {
            InitializeComponent();

            _service = new TaskService();

            AddCommand = new RelayCommand(_ => OpenAdd());
            EditCommand = new RelayCommand(_ => OpenEdit(), _ => SelectedTask != null);
            DeleteCommand = new RelayCommand(_ => DeleteTask(), _ => SelectedTask != null);
            SaveCommand = new RelayCommand(_ => Save());
            LoadCommand = new RelayCommand(_ => Load());
            ClearSearchCommand = new RelayCommand(_ => SearchQuery = string.Empty);

            DataContext = this;
            Refresh();
        }

        public ObservableCollection<TaskItem> Tasks { get; } = new();

        public TaskItem? SelectedTask
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); Refresh(); }
        }

        public string StatusFilter
        {
            get => _statusFilter;
            set { _statusFilter = value; OnPropertyChanged(); Refresh(); }
        }

        public string SortMode
        {
            get => _sortMode;
            set { _sortMode = value; OnPropertyChanged(); Refresh(); }
        }

        public TaskStatistics Stats
        {
            get => _stats;
            private set { _stats = value; OnPropertyChanged(); }
        }

        public string[] StatusFilters { get; } =
            { "Все", "Новая", "В процессе", "Завершена" };

        public string[] SortModes { get; } =
            { "По умолчанию", "Приоритет ↓", "Приоритет ↑", "Срок ↑", "Срок ↓" };

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand ClearSearchCommand { get; }

        private void Refresh()
        {
            var query = StatusFilter switch
            {
                "Новая" => _service.FilterByStatus(TaskStatus.New),
                "В процессе" => _service.FilterByStatus(TaskStatus.InProgress),
                "Завершена" => _service.FilterByStatus(TaskStatus.Completed),
                _ => _service.GetAll()
            };

            if (!string.IsNullOrWhiteSpace(SearchQuery))
                query = query.Where(t =>
                    t.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            query = SortMode switch
            {
                "Приоритет ↓" => query.OrderByDescending(t => t.Priority),
                "Приоритет ↑" => query.OrderBy(t => t.Priority),
                "Срок ↑" => query.OrderBy(t => t.DueDate),
                "Срок ↓" => query.OrderByDescending(t => t.DueDate),
                _ => query
            };

            Tasks.Clear();
            foreach (var t in query) Tasks.Add(t);
            Stats = _service.GetStatistics();
        }

        private void OpenAdd()
        {
            var dlg = new Views.TaskEditWindow(new TaskItem());
            if (dlg.ShowDialog() == true)
            {
                _service.Add(dlg.Result!);
                Refresh();
            }
        }

        private void OpenEdit()
        {
            if (SelectedTask is null) return;
            var dlg = new Views.TaskEditWindow(Clone(SelectedTask));
            if (dlg.ShowDialog() == true)
            {
                _service.Update(dlg.Result!);
                Refresh();
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask is null) return;
            if (MessageBox.Show($"Удалить «{SelectedTask.Title}»?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _service.Delete(SelectedTask.Id);
                Refresh();
            }
        }

        private void Save()
        {
            var dlg = new SaveFileDialog { Filter = "JSON|*.json", FileName = "tasks.json" };
            if (dlg.ShowDialog() == true)
            {
                _service.SaveToFile(dlg.FileName);
                MessageBox.Show("Сохранено!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Load()
        {
            var dlg = new OpenFileDialog { Filter = "JSON|*.json" };
            if (dlg.ShowDialog() == true)
            {
                _service.LoadFromFile(dlg.FileName);
                Refresh();
            }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}