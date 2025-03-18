using FluentAssertions;

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
        Assert.That(sut.Name, Is.EqualTo(newName));

        sut.Name.Should().Be(newName);
        sut.Should().BeEquivalentTo(beforeChanges, o => o.Excluding(p => p.Name));
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
        Assert.IsTrue(task.IsFaulted);
    }

    [Test]
    public void PersonHasSalary_IncreasedByPositiveValue_SalaryIncreased()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void PersonHasSalary_IncreasedByZeroPercent_SalaryRemainsSame()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void PersonHasSalary_DecreasedByNegativeValue_SalaryDecreased()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void PersonHasSalary_DecreasedByMoreThanTenPercent_Fails()
    {
        throw new NotImplementedException();
    }
}