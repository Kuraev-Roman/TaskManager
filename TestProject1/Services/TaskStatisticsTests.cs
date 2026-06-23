using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Services;

namespace TaskTests.Services
{
    [TestClass]
    public class TaskStatisticsTests
    {
        [TestMethod]
        public void NewTaskStatistics_AllFieldsAreZero()
        {
            var stats = new TaskStatistics();

            Assert.AreEqual(0, stats.Total);
            Assert.AreEqual(0, stats.New);
            Assert.AreEqual(0, stats.InProgress);
            Assert.AreEqual(0, stats.Completed);
            Assert.AreEqual(0, stats.Overdue);
            Assert.AreEqual(0, stats.Important);
        }

        [TestMethod]
        public void CanSetAndGet_AllProperties()
        {
            var stats = new TaskStatistics
            {
                Total = 10,
                New = 4,
                InProgress = 3,
                Completed = 2,
                Overdue = 1,
                Important = 5
            };

            Assert.AreEqual(10, stats.Total);
            Assert.AreEqual(4, stats.New);
            Assert.AreEqual(3, stats.InProgress);
            Assert.AreEqual(2, stats.Completed);
            Assert.AreEqual(1, stats.Overdue);
            Assert.AreEqual(5, stats.Important);
        }

        [TestMethod]
        public void StatusCounts_CanSumToTotal()
        {
            var stats = new TaskStatistics
            {
                Total = 6,
                New = 2,
                InProgress = 2,
                Completed = 2
            };

            Assert.AreEqual(stats.Total, stats.New + stats.InProgress + stats.Completed);
        }
    }
}