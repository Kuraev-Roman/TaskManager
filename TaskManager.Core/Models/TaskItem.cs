using System;
using System.Text.Json.Serialization;

namespace TaskManager.Core.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);
        public TaskStatus Status { get; set; } = TaskStatus.New;
        public bool IsImportant { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool IsOverdue =>
            Status != TaskStatus.Completed && DueDate.Date < DateTime.Today;

        [JsonIgnore]
        public string PriorityText => Priority switch
        {
            Priority.Low => "Низкий",
            Priority.Medium => "Средний",
            Priority.High => "Высокий",
            _ => Priority.ToString()
        };

        [JsonIgnore]
        public string StatusText => Status switch
        {
            TaskStatus.New => "Новая",
            TaskStatus.InProgress => "В процессе",
            TaskStatus.Completed => "Завершена",
            _ => Status.ToString()
        };

        [JsonIgnore]
        public string ImportantText => IsImportant ? "★" : "☆";
    }
}