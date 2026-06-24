using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskTests.Services
{
    [TestClass]
    public class FileServiceTests
    {
        private static TaskItem NewTask(string title = "Тест") => new()
        {
            Title = title,
            Priority = Priority.Medium,
            Status = TaskStatus.New,
            DueDate = DateTime.Today.AddDays(1)
        };

        [TestMethod]
        public void SaveToFile_AndLoadFromFile_PreservesData()
        {
            var service = new FileService();
            var tasks = new List<TaskItem> { NewTask("Задача 1"), NewTask("Задача 2") };
            string path = Path.GetTempFileName();
            try
            {
                service.SaveToFile(tasks, path);
                var loaded = service.LoadFromFile(path);
                Assert.AreEqual(2, loaded.Count);
                Assert.AreEqual("Задача 1", loaded[0].Title);
                Assert.AreEqual("Задача 2", loaded[1].Title);
            }
            finally { File.Delete(path); }
        }

        [TestMethod]
        public void LoadFromFile_MissingFile_ThrowsFileNotFoundException()
        {
            var service = new FileService();
            Assert.ThrowsException<FileNotFoundException>(
                () => service.LoadFromFile("nope.json"));
        }

        [TestMethod]
        public void SaveToFile_EmptyList_SavesAndLoadsEmpty()
        {
            var service = new FileService();
            string path = Path.GetTempFileName();
            try
            {
                service.SaveToFile(new List<TaskItem>(), path);
                var loaded = service.LoadFromFile(path);
                Assert.AreEqual(0, loaded.Count);
            }
            finally { File.Delete(path); }
        }
    }
}