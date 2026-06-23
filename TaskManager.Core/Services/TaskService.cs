using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskService : ITaskService
    {
        private List<TaskItem> _tasks = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public void Add(TaskItem task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Название не может быть пустым.", nameof(task));
            _tasks.Add(task);
        }

        public void Update(TaskItem task)
        {
            if (task is null)
                throw new ArgumentNullException(nameof(task));
            int idx = _tasks.FindIndex(t => t.Id == task.Id);
            if (idx == -1)
                throw new KeyNotFoundException($"Задача Id={task.Id} не найдена.");
            _tasks[idx] = task;
        }

        public void Delete(Guid id)
        {
            int removed = _tasks.RemoveAll(t => t.Id == id);
            if (removed == 0)
                throw new KeyNotFoundException($"Задача Id={id} не найдена.");
        }

        public TaskItem? GetById(Guid id) =>
            _tasks.FirstOrDefault(t => t.Id == id);

        public IEnumerable<TaskItem> GetAll() =>
            _tasks.AsReadOnly();

        public IEnumerable<TaskItem> FilterByStatus(TaskStatus status) =>
            _tasks.Where(t => t.Status == status);

        public IEnumerable<TaskItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _tasks.AsReadOnly();
            return _tasks.Where(t =>
                t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<TaskItem> SortByPriority(bool descending = true) =>
            descending
                ? _tasks.OrderByDescending(t => t.Priority)
                : _tasks.OrderBy(t => t.Priority);

        public IEnumerable<TaskItem> SortByDueDate(bool descending = false) =>
            descending
                ? _tasks.OrderByDescending(t => t.DueDate)
                : _tasks.OrderBy(t => t.DueDate);

        public TaskStatistics GetStatistics() => new()
        {
            Total = _tasks.Count,
            New = _tasks.Count(t => t.Status == TaskStatus.New),
            InProgress = _tasks.Count(t => t.Status == TaskStatus.InProgress),
            Completed = _tasks.Count(t => t.Status == TaskStatus.Completed),
            Overdue = _tasks.Count(t => t.IsOverdue),
            Important = _tasks.Count(t => t.IsImportant)
        };

        public void SaveToFile(string filePath)
        {
            string json = JsonSerializer.Serialize(_tasks, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);
            string json = File.ReadAllText(filePath);
            _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions)
                     ?? new List<TaskItem>();
        }
    }
}