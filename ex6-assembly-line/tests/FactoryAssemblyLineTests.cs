using NUnit.Framework;
using System;

namespace AssemblyLineSimulation.Tests
{
    [TestFixture]
    public class FactoryAssemblyLineTests
    {
        [Test]
        public void Initialization_ShouldSetCorrectValues()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.AreEqual(5, line.GetNumStations());
            Assert.AreEqual(0, line.GetNumActiveStations());
            Assert.AreEqual(5, line.GetNumInactiveStations());
        }

        [Test]
        public void Initialization_WithNegativeStations_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new FactoryAssemblyLine(-1));
        }

        [Test]
        public void AddStation_ShouldAddStationCorrectly()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            Assert.AreEqual(10, line.GetProcessingTime(1));
            Assert.AreEqual(4, line.GetNumInactiveStations());
        }

        [Test]
        public void RemoveStation_ShouldRemoveStationCorrectly()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            line.RemoveStation(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.GetProcessingTime(1));
            Assert.AreEqual(5, line.GetNumInactiveStations());
        }

        [Test]
        public void AddStation_WithInvalidId_ShouldThrowArgumentException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentException>(() => line.AddStation(-1, 10));
        }

        [Test]
        public void RemoveStation_WithNonExistentId_ShouldThrowArgumentOutOfRangeException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.RemoveStation(1));
        }

        [Test]
        public void StartAssembly_ShouldActivateStation()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            line.StartAssembly(1);
            Assert.IsTrue(line.IsStationActive(1));
            Assert.AreEqual(1, line.GetNumActiveStations());
        }

        [Test]
        public void StopAssembly_ShouldDeactivateStation()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            line.StartAssembly(1);
            line.StopAssembly(1);
            Assert.IsFalse(line.IsStationActive(1));
            Assert.AreEqual(0, line.GetNumActiveStations());
        }

        [Test]
        public void StartAssembly_WithInvalidId_ShouldThrowArgumentOutOfRangeException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.StartAssembly(10));
        }

        [Test]
        public void StopAssembly_WithInvalidId_ShouldThrowArgumentOutOfRangeException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.StopAssembly(10));
        }

        [Test]
        public void TotalProcessingTime_ShouldReturnCorrectValue()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            line.AddStation(2, 20);
            line.StartAssembly(1);
            line.StartAssembly(2);
            Assert.AreEqual(30, line.GetTotalProcessingTime());
        }

        [Test]
        public void InactiveStations_ShouldNotAffectTotalProcessingTime()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            line.StartAssembly(1);
            Assert.AreEqual(10, line.GetTotalProcessingTime());
        }

        [Test]
        public void AddDuplicateStation_ShouldThrowArgumentException()
        {
            var line = new FactoryAssemblyLine(5);
            line.AddStation(1, 10);
            Assert.Throws<ArgumentException>(() => line.AddStation(1, 15));
        }

        [Test]
        public void StartAssembly_NonExistentStation_ShouldThrowArgumentOutOfRangeException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.StartAssembly(1));
        }

        [Test]
        public void StopAssembly_NonExistentStation_ShouldThrowArgumentOutOfRangeException()
        {
            var line = new FactoryAssemblyLine(5);
            Assert.Throws<ArgumentOutOfRangeException>(() => line.StopAssembly(1));
        }
    }
}