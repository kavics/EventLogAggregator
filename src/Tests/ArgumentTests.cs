using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenseNet.Tools.CommandLineArguments;
using SpaceBender.EventLogAggregator;

namespace Tests
{
    [TestClass]
    public class ArgumentTests
    {
        

        [TestMethod]
        public void Arguments_Empty()
        {
            var args = new string[0];
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.LocalComputer, config.SourceType);
            Assert.AreEqual(Environment.MachineName, config.ComputerName);
            Assert.AreEqual(Configuration.DefaultLogName, config.LogName);
            Assert.AreEqual($"{Environment.MachineName}_{Configuration.DefaultLogName}", config.SourceName);
            Assert.AreEqual($"{Environment.CurrentDirectory}\\{Environment.MachineName}_{Configuration.DefaultLogName}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{Environment.CurrentDirectory}\\{Environment.MachineName}_{Configuration.DefaultLogName}-events.txt", config.EventsFileName);
        }





        [TestMethod]
        public void Arguments_Remote()
        {
Assert.Inconclusive();

            var args = new[] { "NEPTUN" };
            var arguments = new Configuration();

            var result = ArgumentParser.Parse(args, arguments);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual("NEPTUN", arguments.ComputerName);
        }
        [TestMethod]
        public void Arguments_XmlFile()
        {
Assert.Inconclusive();

            var args = new[] { "-file:c:\\log.xml" };
            var arguments = new Configuration();

            var result = ArgumentParser.Parse(args, arguments);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual("c:\\log.xml", arguments.FileName);
        }
        [TestMethod]
        public void Arguments_EvtxFile()
        {
Assert.Inconclusive();

            var args = new[] { "-file:c:\\log.evtx" };
            var arguments = new Configuration();

            var result = ArgumentParser.Parse(args, arguments);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual("c:\\log.evtx", arguments.FileName);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Arguments_UnknownFile()
        {
Assert.Inconclusive();

            var args = new[] { "-file:c:\\log" };
            var arguments = new Configuration();

            var result = ArgumentParser.Parse(args, arguments);
        }
    }
}
