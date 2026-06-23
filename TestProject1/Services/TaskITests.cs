using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Models;

namespace TaskTests.Services
{
    [TestClass]
    public class TaskITests
    {
        [TestMethod]
        public void NewTaskItem_HasDefaultValues()
        {
            var task = new TaskItem();

            Assert.AreNotEqual(Guid.Empty, task.Id);
            Assert.AreEqual(string.Empty, task.Title);
            Assert.AreEqual(string.Empty, task.Description);
            Assert.AreEqual(TaskPriority.Medium, task.Priority);
            Assert.AreEqual(TaskStatus.New, task.Status);
            Assert.IsFalse(task.IsImportant);
        }

        [TestMethod]
        public void NewTaskItem_DueDate_DefaultsToTomorrow()
        {
            var task = new TaskItem();
            Assert.AreEqual(DateTime.Today.AddDays(1), task.DueDate.Date);
        }

        [TestMethod]
        public void TwoNewTasks_HaveDifferentIds()
        {
            var t1 = new TaskItem();
            var t2 = new TaskItem();
            Assert.AreNotEqual(t1.Id, t2.Id);
        }

        [TestMethod]
        public void IsOverdue_FutureDueDate_ReturnsFalse()
        {
            var task = new TaskItem
            {
                Status = TaskStatus.New,
                DueDate = DateTime.Today.AddDays(5)
            };
            Assert.IsFalse(task.IsOverdue);
        }

        [TestMethod]
        public void IsOverdue_TodayDueDate_ReturnsFalse()
        {
            var task = new TaskItem
            {
                Status = TaskStatus.New,
                DueDate = DateTime.Today
            };
            Assert.IsFalse(task.IsOverdue);
        }

        [TestMethod]
        public void IsOverdue_PastDueDateAndNotCompleted_ReturnsTrue()
        {
            var task = new TaskItem
            {
                Status = TaskStatus.InProgress,
                DueDate = DateTime.Today.AddDays(-3)
            };
            Assert.IsTrue(task.IsOverdue);
        }

        [TestMethod]
        public void IsOverdue_PastDueDateButCompleted_ReturnsFalse()
        {
            var task = new TaskItem
            {
                Status = TaskStatus.Completed,
                DueDate = DateTime.Today.AddDays(-3)
            };
            Assert.IsFalse(task.IsOverdue);
        }

        [TestMethod]
        public void CanSetAndGet_AllProperties()
        {
            var id = Guid.NewGuid();
            var created = DateTime.Now.AddDays(-1);
            var due = DateTime.Today.AddDays(7);

            var task = new TaskItem
            {
                Id = id,
                Title = "Купить продукты",
                Description = "Молоко, хлеб, яйца",
                Priority = TaskPriority.High,
                Status = TaskStatus.InProgress,
                DueDate = due,
                IsImportant = true,
                CreatedAt = created
            };

            Assert.AreEqual(id, task.Id);
            Assert.AreEqual("Купить продукты", task.Title);
            Assert.AreEqual("Молоко, хлеб, яйца", task.Description);
            Assert.AreEqual(TaskPriority.High, task.Priority);
            Assert.AreEqual(TaskStatus.InProgress, task.Status);
            Assert.AreEqual(due, task.DueDate);
            Assert.IsTrue(task.IsImportant);
            Assert.AreEqual(created, task.CreatedAt);
        }
    }
}