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
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;

            var args = new string[0];
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.LocalComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_RemoteLocal()
        {
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;

            var args = new[] { machine };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.LocalComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_Remote()
        {
            var machine = Environment.MachineName + "-1";
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;

            var args = new[] { machine };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.RemoteComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_EmptyComputer()
        {
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;

            var args = new[] { "" };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.LocalComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_LocalSpecificLog()
        {
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName + "-1";;
            var dir = Environment.CurrentDirectory;

            var args = new[] { $"-log:{log}" };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.LocalComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_RemoteSpecificLog()
        {
            var machine = Environment.MachineName + "-1";
            var log = Configuration.DefaultLogName + "-1"; ;
            var dir = Environment.CurrentDirectory;

            var args = new[] { machine, $"-log:{log}" };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.RemoteComputer, config.SourceType);
            Assert.IsFalse(config.SourceIsFile);
            Assert.IsNull(config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual($"{machine}_{log}", config.SourceName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{dir}\\{machine}_{log}-events.txt", config.EventsFileName);
        }

        [TestMethod]
        public void Arguments_XmlFile()
        {
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;
            var file = "c:\\log.xml";

            var args = new[] { $"-file:{file}" };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.XmlFile, config.SourceType);
            Assert.IsTrue(config.SourceIsFile);
            Assert.AreEqual(file, config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual(file, config.SourceName);
            Assert.AreEqual($"{file}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{file}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_EvtxFile()
        {
            var machine = Environment.MachineName;
            var log = Configuration.DefaultLogName;
            var dir = Environment.CurrentDirectory;
            var file = "c:\\log.evtx";

            var args = new[] { $"-file:{file}" };
            var config = new Configuration();

            var result = ArgumentParser.Parse(args, config);

            Assert.IsFalse(result.IsHelp);
            Assert.AreEqual(SourceType.EvtxFile, config.SourceType);
            Assert.IsTrue(config.SourceIsFile);
            Assert.AreEqual(file, config.FileName);
            Assert.AreEqual(machine, config.ComputerName);
            Assert.AreEqual(log, config.LogName);
            Assert.AreEqual(file, config.SourceName);
            Assert.AreEqual($"{file}-errors.txt", config.ErrorsFileName);
            Assert.AreEqual($"{file}-events.txt", config.EventsFileName);
        }
        [TestMethod]
        public void Arguments_UnknownFile()
        {
            var args = new[] { "-file:c:\\log" };
            var config = new Configuration();

            try
            {
                var result = ArgumentParser.Parse(args, config);
                Assert.Fail("ArgumentException was not thrown.");
            }
            catch(Exception e)
            {
                if (e is System.Reflection.TargetInvocationException)
                    e = e.InnerException;
                if(!(e is ArgumentException))
                    Assert.Fail("ArgumentException was not thrown.");
            }
        }
    }
}
