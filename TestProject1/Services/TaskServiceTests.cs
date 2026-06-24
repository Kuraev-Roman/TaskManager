using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskTests.Services
{
    [TestClass]
    public class TaskServiceTests
    {
        private static TaskService Make() => new();

        private static TaskItem NewTask(
            string title = "Тест",
            Priority priority = Priority.Medium,
            TaskStatus status = TaskStatus.New,
            int days = 1) => new()
            {
                Title = title,
                Priority = priority,
                Status = status,
                DueDate = DateTime.Today.AddDays(days)
            };

        [TestMethod]
        public void Add_ValidTask_AddsToCollection()
        {
            var s = Make();
            s.Add(NewTask());
            Assert.AreEqual(1, s.GetAll().Count());
        }

        [TestMethod]
        public void Update_ExistingTask_ChangesTitle()
        {
            var s = Make();
            var t = NewTask("Старое");
            s.Add(t);
            t.Title = "Новое";
            s.Update(t);
            Assert.AreEqual("Новое", s.GetById(t.Id)!.Title);
        }

        [TestMethod]
        public void Delete_ExistingTask_RemovesFromCollection()
        {
            var s = Make();
            var t = NewTask();
            s.Add(t);
            s.Delete(t.Id);
            Assert.AreEqual(0, s.GetAll().Count());
        }

        [TestMethod]
        public void GetById_ExistingId_ReturnsTask()
        {
            var s = Make();
            var t = NewTask();
            s.Add(t);
            Assert.IsNotNull(s.GetById(t.Id));
        }

        [TestMethod]
        public void GetAll_ReturnsAllTasks()
        {
            var s = Make();
            s.Add(NewTask("A"));
            s.Add(NewTask("B"));
            Assert.AreEqual(2, s.GetAll().Count());
        }

        [TestMethod]
        public void FilterByStatus_ReturnsOnlyMatchingStatus()
        {
            var s = Make();
            s.Add(NewTask("A", status: TaskStatus.New));
            s.Add(NewTask("B", status: TaskStatus.Completed));
            var result = s.FilterByStatus(TaskStatus.Completed).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("B", result[0].Title);
        }

        [TestMethod]
        public void Search_ByTitle_ReturnsMatchingTask()
        {
            var s = Make();
            s.Add(NewTask("Купить молоко"));
            s.Add(NewTask("Сделать отчёт"));
            Assert.AreEqual(1, s.Search("молоко").Count());
        }

        [TestMethod]
        public void SortByPriority_Descending_HighFirst()
        {
            var s = Make();
            s.Add(NewTask("L", Priority.Low));
            s.Add(NewTask("H", Priority.High));
            var result = s.SortByPriority(true).ToList();
            Assert.AreEqual(Priority.High, result[0].Priority);
        }

        [TestMethod]
        public void SortByDueDate_Ascending_EarliestFirst()
        {
            var s = Make();
            s.Add(NewTask("Поздно", days: 10));
            s.Add(NewTask("Рано", days: 1));
            var result = s.SortByDueDate(false).ToList();
            Assert.AreEqual("Рано", result[0].Title);
        }
    }
}