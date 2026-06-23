using System;
using System.Collections.Generic;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public interface ITaskService
    {
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(Guid id);
        TaskItem? GetById(Guid id);
        IEnumerable<TaskItem> GetAll();
        IEnumerable<TaskItem> FilterByStatus(TaskStatus status);
        IEnumerable<TaskItem> Search(string query);
        IEnumerable<TaskItem> SortByPriority(bool descending = true);
        IEnumerable<TaskItem> SortByDueDate(bool descending = false);
        TaskStatistics GetStatistics();
        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);
    }
}