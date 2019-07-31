using System;
using System.Globalization;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib.Services.Logging;
using AVS.CoreLib.Services.Logging.LogBuffers;
using AVS.CoreLib.Services.Logging.Loggers;
using AVS.CoreLib.Services.Logging.LogWriters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests
{
    [TestClass]
    public class TaskLogWriterTests
    {
        protected TaskLogWriter writer;
        protected TextLogBuffer buffer;
        protected TextLogger logger;
        protected AppConfig config;

        [TestInitialize]
        public void Init()
        {
            logger = new TextLogger() {AddPrefix = false};
            buffer = new TextLogBuffer(){AddPrefix = false};
            config = new AppConfig();
        }

        TaskLogWriter CreateTaskLogWriter()
        {
            return new TaskLogWriter(buffer, logger, config, CultureInfo.CurrentCulture);
        }


        [TestMethod]
        public void UseBufferTest()
        {
            //arrange
            var taskname = "testtask";
            writer = CreateTaskLogWriter();

            //act
            writer.StartTask(taskname);
            writer.Write("message");
            
            //assert
            Assert.AreEqual(logger.Count, 0);
            Assert.AreEqual(buffer.Count, 1);
        }

        [TestMethod]
        public void FlushBufferTest()
        {
            //arrange
            var taskname = "testtask";
            var message = "message1";
            writer = CreateTaskLogWriter();
            writer.StartTask(taskname);
            writer.Write(message);
            writer.EndTask(taskname);
            //act
            writer.Flush();

            //assert
            Assert.AreEqual(0, buffer.Count,"Buffer is not empty");
            Assert.AreEqual(true, logger.Count > 0, "TextLogger must contain a log");
            var log = logger.ToString();
            Assert.AreEqual(true, log.Contains(message), $"Log does not contain '{message}'");
            }

        [TestMethod]
        public void DoNotUseBufferTest()
        {
            //arrange
            var taskname = "testtask";
            writer = CreateTaskLogWriter();
            //act
            writer.Write("message");

            //assert
            Assert.AreEqual(logger.Count, 1);
            Assert.AreEqual(buffer.Count, 0);
        }

        
    }
}
