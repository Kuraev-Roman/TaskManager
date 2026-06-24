using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskService
    {
        private List<TaskItem> _tasks = new();

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
    }
}