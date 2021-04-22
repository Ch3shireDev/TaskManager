using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TaskManagerLibrary;

namespace TaskManagerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var time = new TimeOfDay(DateTime.Now);
            var csv = Tools.GetCsv("../../../database.yml");
        }
    }
}
