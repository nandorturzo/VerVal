using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using System;
using System.Threading;

namespace DatesAndStuff.Mobile.Tests
{
    internal class PersonPageMobileTests : BaseTest
    {
        [TestCase(1, 5050)]
        [TestCase(10, 5500)]
        [TestCase(20, 6000)]
        public void Person_SalaryIncrease_ShouldIncrease(double percentage, double expectedSalary)
        {
            // Open navigation drawer
            var drawer = App.FindElement(MobileBy.XPath("//android.widget.ImageButton[@content-desc=\"Open navigation drawer\"]"));
            drawer.Click();

            // Select "Person" menu item
            var personMenu = App.FindElement(MobileBy.XPath("//android.widget.TextView[@text=\"Person\"]"));
            personMenu.Click();

            // Read initial salary
            var salaryLabelBefore = App.FindElement(MobileBy.XPath("//android.widget.TextView[@text='5000']"));
            double initialSalary = double.Parse(salaryLabelBefore.Text);
            initialSalary.Should().BeApproximately(5000, 0.01);

            // Enter the percentage value
            var inputField = App.FindElement(MobileBy.XPath("//android.widget.EditText[@text='Enter a percentage value']"));
            inputField.Clear();
            inputField.SendKeys(percentage.ToString());

            // Click the submit button
            var submitButton = App.FindElement(MobileBy.XPath("//android.widget.Button[@text='Submit']"));
            submitButton.Click();

            // Wait for the update to reflect
            Thread.Sleep(1000);

            // Verify updated salary is displayed
            var salaryLabelAfter = App.FindElement(MobileBy.XPath($"//android.widget.TextView[@text='{expectedSalary}']"));
            double newSalary = double.Parse(salaryLabelAfter.Text);
            newSalary.Should().BeApproximately(expectedSalary, 0.01);

        }

        [TestCase(-10, "The specified percentag should be between -10 and infinity.")]
        [TestCase(-15, "The specified percentag should be between -10 and infinity.")]
        public void Person_InvalidPercentage_ShouldShowValidationErrors(double percentage, string expectedError)
        {
            // Open navigation drawer
            var drawer = App.FindElement(MobileBy.XPath("//android.widget.ImageButton[@content-desc=\"Open navigation drawer\"]"));
            drawer.Click();

            // Navigate to Person page
            var personMenu = App.FindElement(MobileBy.XPath("//android.widget.TextView[@text=\"Person\"]"));
            personMenu.Click();

            // Enter invalid input
            var inputField = App.FindElement(MobileBy.XPath("//android.widget.EditText[@text='Enter a percentage value']"));
            inputField.Clear();
            inputField.SendKeys(percentage.ToString());

            Thread.Sleep(500);

            var errorLabel = App.FindElement(MobileBy.XPath("//android.widget.TextView[contains(@text, 'should be between -10')]"));
            errorLabel.Text.Should().Contain(expectedError);


            // Tap the submit button
            var submitBtn = App.FindElement(MobileBy.XPath("//android.widget.Button[@text='Submit']"));
            submitBtn.Click();

            // Optionally re-check that salary did NOT update
            var salaryLabel = App.FindElement(MobileBy.XPath("//android.widget.TextView[contains(@text, '5000')]"));
            salaryLabel.Text.Should().Contain("5000");
        }

    }
}
