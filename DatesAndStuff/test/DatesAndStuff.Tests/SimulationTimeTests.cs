namespace DatesAndStuff.Tests
{
    public sealed class SimulationTimeTests
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

        [Test]
        public void Constructor_WithoutParameters_SetsDefaultTime()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ComparisonOperators_WithDifferentTimes_ReturnExpectedResults()
        {
            throw new NotImplementedException();
        }

        private class TimeSpanArithmeticTests
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
                Assert.AreEqual(expectedDateTime, result.ToAbsoluteDateTime());
            }

            [Test]
            public void SubtractTimeSpan_FromSimulationTime_ReturnsShiftedTime()
            {
                // code kozelibb
                // RegisterOrder_SignedInUserSendsOrder_OrderIsRegistered
                throw new NotImplementedException();
            }
        }

        [Test]
        public void SubtractSimulationTime_FromAnotherSimulationTime_ReturnsTimeSpan()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void MinValue_AddMilliseconds_CreatesValidTime()
        {
            //var t1 = SimulationTime.MinValue.AddMilliseconds(10);
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        [Test]
        public void ToString_OnSimulationTime_ReturnsFormattedString()
        {
            throw new NotImplementedException();
        }
    }
}