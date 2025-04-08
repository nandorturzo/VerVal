using FluentAssertions;
using FluentAssertions.Execution;
using Moq;

namespace DatesAndStuff.Tests;

public class PersonTests
{
    Person sut;

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Constructor_DefaultParams_ShouldBeAbleToEatChocolate()
    {
        // Arrange

        // Act
        Person sut = PersonFactory.CreateTestPerson();

        // Assert
        sut.CanEatChocolate.Should().BeTrue();
    }

    [Test]
    public void Constructor_DontLikeChocolate_ShouldNotBeAbleToEatChocolate()
    {
        // Arrange

        // Act
        Person sut = PersonFactory.CreateTestPerson(fp => fp.CanEatChocolate = false);

        // Assert
        sut.CanEatChocolate.Should().BeFalse();
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

            sut.Salary.Should().Be(expectedSalary);
        }

        [TestCase(1)]         // Test minimal valid salary increase (1%)
        [TestCase(10)]        // Test valid salary increase (10%)
        [TestCase(50)]        // Test valid salary increase (50%)
        [TestCase(200)]       // Test valid salary increase (100%)
        [TestCase(-1)]        // Test valid salary decrease (-1%)
        [TestCase(-5)]        // Test valid salary decrease (-5%)
        [TestCase(-10)]       // Test maximum valid salary decrease (-10%)
        public void IncreaseSalary_ValidPercentage_ShouldChangeSalary(double salaryIncreasePercentage)
        {
            // Arrange
            var employmentInfo = new EmploymentInformation(
                1000,
                new Employer(
                    taxId: "RO12345678",
                    address: "Strada Libertatii 45, Cluj-Napoca",
                    ownername: "Maria Ionescu",
                    activityDomains: new List<int> { 303, 404 }
                )
            );

            var taxData = new LocalTaxData("RO-CJ")
            {
                DiscountPercentage = 0,
                TaxItems = new List<TaxItem>()
            };

            var mockPaymentService = new Mock<IPaymentService>();
            mockPaymentService.Setup(x => x.SuccessFul()).Returns(true);

            var sut = new Person(
                "TestPerson",
                employmentInfo,
                mockPaymentService.Object,
                taxData,
                new FoodPreferenceParams
                {
                    CanEatGluten = true,
                    CanEatLactose = true,
                    CanEatEgg = true,
                    CanEatChocolate = true
                }
            );

            // Act
            double initialSalary = sut.Salary;
            sut.IncreaseSalary(salaryIncreasePercentage);

            // Assert
            sut.Salary.Should().BeApproximately(initialSalary * (100 + salaryIncreasePercentage) / 100, 0.00000001);
        }

        [TestCase(0)]         // Test invalid salary increase (equals 0)
        [TestCase(-11)]       // Test invalid salary decrease (less than -10%)
        [TestCase(-20)]       // Test invalid salary decrease (less than -10%)
        public void IncreaseSalary_InvalidPercentageOrZero_ShouldThrowException(double salaryIncreasePercentage)
        {
            // Arrange
            var employmentInfo = new EmploymentInformation(
                1000,
                new Employer(
                    taxId: "RO12345678",
                    address: "Strada Libertatii 45, Cluj-Napoca",
                    ownername: "Maria Ionescu",
                    activityDomains: new List<int> { 303, 404 }
                )
            );

            var taxData = new LocalTaxData("RO-CJ")
            {
                DiscountPercentage = 0,
                TaxItems = new List<TaxItem>()
            };

            var mockPaymentService = new Mock<IPaymentService>();
            mockPaymentService.Setup(x => x.SuccessFul()).Returns(true);

            var sut = new Person(
                "TestPerson",
                employmentInfo,
                mockPaymentService.Object,
                taxData,
                new FoodPreferenceParams
                {
                    CanEatGluten = true,
                    CanEatLactose = true,
                    CanEatEgg = true,
                    CanEatChocolate = true
                }
            );

            // Act
            Action act = () => sut.IncreaseSalary(salaryIncreasePercentage);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("Specified argument was out of the range of valid values. (Parameter 'percentage')");
        }

        [Test]
        public void PersonHasSalary_DecreasedByMoreThanTenPercent_Fails()
        {
            double decreasePercentage = -10.1;

            Action act = () => sut.IncreaseSalary(decreasePercentage);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}