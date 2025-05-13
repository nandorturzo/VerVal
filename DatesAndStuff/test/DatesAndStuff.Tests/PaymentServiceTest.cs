using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DatesAndStuff.Tests
{
    internal class PaymentServiceTest
    {
        [Test]
        public void TestPaymentService_ManualMock_SufficientBalance()
        {
            // Arrange
            var paymentService = new TestPaymentService(600); // Sufficient balance of 600
            Person sut = new Person(
                "Test Pista",
                new EmploymentInformation(54, new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams() { CanEatChocolate = true, CanEatEgg = true, CanEatLactose = true, CanEatGluten = true }
            );

            bool result = sut.PerformSubsriptionPayment();

            result.Should().BeTrue("because the balance is sufficient to cover the subscription fee");
        }

        [Test]
        public void TestPaymentService_ManualMock_InsufficientBalance()
        {
            var paymentService = new TestPaymentService(400); // Insufficient balance of 400
            Person sut = new Person(
                "Test Pista",
                new EmploymentInformation(54, new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams() { CanEatChocolate = true, CanEatEgg = true, CanEatLactose = true, CanEatGluten = true }
            );

            Action act = () => sut.PerformSubsriptionPayment();
            act.Should().Throw<Exception>();
        }

        [Test]
        public void TestPaymentService_Mock_InsufficientBalance()
        {
            // Arrange
            var paymentSequence = new MockSequence();
            var paymentService = new Mock<IPaymentService>();

            paymentService.InSequence(paymentSequence).Setup(m => m.StartPayment());
            paymentService.InSequence(paymentSequence).Setup(m => m.SpecifyAmount(Person.SubscriptionFee));
            paymentService.InSequence(paymentSequence).SetupGet(m => m.Balance).Returns(400);  // Insufficient balance
            paymentService.InSequence(paymentSequence).Setup(m => m.Cancel());

            var paymentServiceMock = paymentService.Object;

            Person sut = new Person(
                "Test Pista",
                new EmploymentInformation(54, new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentServiceMock,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams() { CanEatChocolate = true, CanEatEgg = true, CanEatLactose = true, CanEatGluten = true }
            );

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeFalse();  // Ensure the result is false because balance is insufficient
            paymentService.Verify(m => m.StartPayment(), Times.Once);
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once);
            paymentService.Verify(m => m.Cancel(), Times.Once);  // Verify that Cancel is called instead of ConfirmPayment
        }


        [Test]
        public void TestPaymentService_Mock_SufficientBalance()
        {
            // Arrange
            var paymentSequence = new MockSequence();
            var paymentService = new Mock<IPaymentService>();

            paymentService.InSequence(paymentSequence).Setup(m => m.StartPayment());
            paymentService.InSequence(paymentSequence).Setup(m => m.SpecifyAmount(Person.SubscriptionFee));
            paymentService.InSequence(paymentSequence).SetupGet(m => m.Balance).Returns(1000);  // Sufficient balance
            paymentService.InSequence(paymentSequence).Setup(m => m.ConfirmPayment());

            var paymentServiceMock = paymentService.Object;

            Person sut = new Person(
                "Test Pista",
                new EmploymentInformation(54, new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentServiceMock,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams() { CanEatChocolate = true, CanEatEgg = true, CanEatLactose = true, CanEatGluten = true }
            );

            // Act                                                             
            bool result = sut.PerformSubsriptionPayment();                     
                                                                               
            // Assert
            result.Should().BeTrue();  // Ensure the result is true because balance is sufficient
            paymentService.Verify(m => m.StartPayment(), Times.Once);
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once);
            paymentService.Verify(m => m.ConfirmPayment(), Times.Once);
        }


        [Test]
        [CustomPersonCreationAutodataAttribute]
        public void TestPaymentService_MockWithAutodata(Person sut, Mock<IPaymentService> paymentService)
        {
            // Arrange

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeTrue();
            paymentService.Verify(m => m.StartPayment(), Times.Once);
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once);
            paymentService.Verify(m => m.ConfirmPayment(), Times.Once);
        }
    }
}
