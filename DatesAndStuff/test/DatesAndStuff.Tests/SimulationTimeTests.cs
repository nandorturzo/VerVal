using System;
using NUnit.Framework;
using FluentAssertions;

namespace DatesAndStuff.Tests
{
    [TestFixture]
    public class SimulationTimeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetupStuff()
        {
            // 
        }

        [SetUp]
        public void Setup()
        {
            // minden teszt felteheti, hogz elotte lefutott ez
        }

        [TearDown]
        public void TearDown()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        // Method_Should_Then terminology used for renaming the tests
        // Method: The method or functionality being tested
        // Should: The expected behavior or condition
        // Then: The expected outcome or result

        [TestFixture]
        public class BasicFunctionalityTests : SimulationTimeTests
        {
            [Test]
            public void Constructor_WithoutParameters_SetsDefaultTime()
            {
                throw new NotImplementedException();
            }

            [Test]
            // equal
            // not equal
            // < 
            // > 
            // <= different 
            // >= different 
            // <= same 
            // >= same 
            // max
            // min
            public void ComparisonOperators_WithDifferentTimes_ReturnExpectedResults()
            {
                throw new NotImplementedException();
            }

            [Test]
            public void EqualOperator_WithSameTimes_ReturnsExpectedResults()
            {
                DateTime date = new DateTime(2022, 5, 13, 12, 0, 0);
                SimulationTime time1 = new SimulationTime(date);
                SimulationTime time2 = new SimulationTime(date);
                time1.Should().Be(time2, "Expected both SimulationTime instances to be equal.");
            }

            [Test]
            public void NotEqualOperator_WithDifferentTimes_ReturnsExpectedResults()
            {
                DateTime date1 = new DateTime(2021, 4, 6, 18, 16, 0);
                DateTime date2 = new DateTime(2021, 4, 6, 18, 16, 1);
                SimulationTime time1 = new SimulationTime(date1);
                SimulationTime time2 = new SimulationTime(date2);
                time1.Should().NotBe(time2, "Expected different SimulationTime instances to not be equal.");
            }

            [Test]
            public void ToString_OnSimulationTime_ReturnsFormattedString()
            {
                throw new NotImplementedException();
            }
        }

        [TestFixture]
        public class TimeManipulationTests : SimulationTimeTests
        {
            [Test]
            public void AddTimeSpan_ToSimulationTime_ReturnsShiftedTime()
            {
                // UserSignedIn_OrderSent_OrderIsRegistered
                // DBB, specflow, cucumber, gherkin
                // Arrange
                DateTime baseDate = new DateTime(2010, 8, 23, 9, 4, 49);
                SimulationTime sut = new SimulationTime(baseDate);
                var ts = TimeSpan.FromMilliseconds(4544313);
                // Act
                var result = sut + ts;
                // Assert
                var expectedDateTime = baseDate + ts;
                result.ToAbsoluteDateTime().Should().Be(expectedDateTime);
            }

            [Test]
            public void SubtractTimeSpan_FromSimulationTime_ReturnsShiftedTime()
            {
                DateTime baseDate = new DateTime(2010, 8, 23, 9, 4, 49);
                SimulationTime sut = new SimulationTime(baseDate);
                var ts = TimeSpan.FromMilliseconds(4544313);

                var result = sut - ts;

                var expectedDateTime = baseDate - ts;
                result.ToAbsoluteDateTime().Should().Be(expectedDateTime);
            }

            [Test]
            public void SubtractSimulationTime_FromAnotherSimulationTime_ReturnsTimeSpan()
            {
                DateTime baseDate1 = new DateTime(2010, 8, 23, 9, 4, 49);
                DateTime baseDate2 = new DateTime(2010, 8, 20, 5, 30, 0);
                SimulationTime time1 = new SimulationTime(baseDate1);
                SimulationTime time2 = new SimulationTime(baseDate2);

                TimeSpan result = time1 - time2;

                TimeSpan expected = baseDate1 - baseDate2;

                //checks with 1 second tolerance
                result.TotalMilliseconds.Should().BeApproximately(expected.TotalMilliseconds, 1);
            }

            [Test]
            public void MinValue_AddMilliseconds_CreatesValidTime()
            {
                SimulationTime minValue = SimulationTime.MinValue;
                double millisToAdd = 10;

                SimulationTime result = minValue.AddMilliseconds(millisToAdd);

                result.TotalMilliseconds.Should().Be(minValue.TotalMilliseconds + (long)millisToAdd);

                // Checks if result is valid and greater than date minValue
                result.ToAbsoluteDateTime().Should().BeAfter(DateTime.MinValue); // BeAfter is the replacement for GreaterThan

                // Checks if result is greater than minValue
                result.Should().BeGreaterThan(minValue);
            }

            [Test]
            public void NextMillisec_OnSimulationTime_IncreasesTimeByOneMillisecond()
            {
                //Assert.AreEqual(t1.TotalMilliseconds + 1, t1.NextMillisec.TotalMilliseconds);
                throw new NotImplementedException();
            }

            [Test]
            public void AddMilliseconds_ToSimulationTime_IncreasesTimeBySpecifiedAmount()
            {
                DateTime baseDate = new DateTime(2010, 8, 23, 9, 4, 49);
                SimulationTime sut = new SimulationTime(baseDate);
                double millisToAdd = 5000;

                SimulationTime result = sut.AddMilliseconds(millisToAdd);

                result.ToAbsoluteDateTime().Should().Be(baseDate.AddMilliseconds(millisToAdd));
            }

            [Test]
            public void AddSeconds_ToSimulationTime_IncreasesTimeBySpecifiedAmount()
            {
                throw new NotImplementedException();
            }

            [Test]
            public void AddTimeSpan_ToSimulationTime_IncreasesTimeBySpecifiedAmount()
            {
                throw new NotImplementedException();
            }
        }
    }
}
