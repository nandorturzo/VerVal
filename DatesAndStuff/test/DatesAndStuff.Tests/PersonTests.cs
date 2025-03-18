using FluentAssertions;
using FluentAssertions.Execution;

namespace DatesAndStuff.Tests;

public class PersonTests
{
    Person sut;

    [SetUp]
    public void Setup()
    {
        this.sut = new Person("Test Pista", 54);
    }

    //Used the Given_When_then terminology for all the test

    [TestFixture] //Marks that the class is used for testing
    public class MarriageTests : PersonTests
    {
        [Test]
        public void Person_GotMarried_NameIsUpdated()
        {
            // Arrange
            string newName = "Test-Eleso Pista";
            double salaryBeforeMarriage = sut.Salary;
            var beforeChanges = Person.Clone(sut);

            // Act
            sut.GotMarried(newName);

            // Assert
            using (new AssertionScope())
            {
                sut.Name.Should().Be(newName);
                sut.Should().BeEquivalentTo(beforeChanges, o => o.Excluding(p => p.Name));
            }
        }

        [Test]
        public void Person_GotMarriedTwice_Fails()
        {
            // Arrange
            sut.GotMarried("");

            // Act
            var task = Task.Run(() => sut.GotMarried(""));
            try { task.Wait(); } catch { }

            // Assert
            task.IsFaulted.Should().BeTrue();
        }
    }

    [TestFixture] //Marks that the class is used for testing
    public class SalaryTests : PersonTests
    {
        [Test]
        public void PersonHasSalary_IncreasedByPositiveValue_SalaryIncreased()
        {
            double initialSalary = sut.Salary;
            double increasePercentage = 5.0;
            double expectedSalary = initialSalary * (1 + increasePercentage / 100);

            sut.IncreaseSalary(increasePercentage);

            Assert.AreEqual(expectedSalary, sut.Salary);
        }

        [Test]
        public void PersonHasSalary_IncreasedByZeroPercent_SalaryRemainsSame()
        {
            double initialSalary = sut.Salary;
            double increasePercentage = 0.0;

            sut.IncreaseSalary(increasePercentage);

            Assert.AreEqual(initialSalary, sut.Salary);
        }

        [Test]
        public void PersonHasSalary_DecreasedByNegativeValue_SalaryDecreased()
        {
            double initialSalary = sut.Salary;
            double decreasePercentage = -5.0;
            double expectedSalary = initialSalary * (1 + decreasePercentage / 100);

            sut.IncreaseSalary(decreasePercentage);

            Assert.AreEqual(expectedSalary, sut.Salary);
        }

        [Test]
        public void PersonHasSalary_DecreasedByMoreThanTenPercent_Fails()
        {
            double decreasePercentage = -10.1;

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => sut.IncreaseSalary(decreasePercentage));
        }
    }
}