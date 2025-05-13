using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DatesAndStuff.Web.Tests
{
    [TestFixture]
    public class PersonPageTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private const string BaseURL = "http://localhost:5091";
        private bool acceptNextAlert = true;

        private Process? _blazorProcess;

        [OneTimeSetUp]
        public void StartBlazorServer()
        {
            var webProjectPath = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                "../../../../../../src/DatesAndStuff.Web/DatesAndStuff.Web.csproj"
                ));

            var webProjFolderPath = Path.GetDirectoryName(webProjectPath);

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                //Arguments = $"run --project \"{webProjectPath}\"",
                Arguments = "dotnet run --no-build",
                WorkingDirectory = webProjFolderPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            _blazorProcess = Process.Start(startInfo);

            // Wait for the app to become available
            var client = new HttpClient();
            var timeout = TimeSpan.FromSeconds(30);
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    var result = client.GetAsync(BaseURL).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        [OneTimeTearDown]
        public void StopBlazorServer()
        {
            if (_blazorProcess != null && !_blazorProcess.HasExited)
            {
                _blazorProcess.Kill(true);
                _blazorProcess.Dispose();
            }
        }

        [SetUp]
        public void SetupTest()
        {
            driver = new ChromeDriver();
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
                driver.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        [TestCase(1, 5050)]
        [TestCase(5, 5250)]
        [TestCase(10, 5500)]
        [TestCase(20, 6000)]
        [TestCase(0, 5000)]
        [TestCase(-10,4500)]
        public void Person_SalaryIncrease_ShouldIncrease(double percentage, double expectedSalary)
        {
            // Navigate to the base URL
            driver.Navigate().GoToUrl(BaseURL);

            // Wait so page loads
            Thread.Sleep(1000);

            // Navigate to the Person page
            driver.FindElement(By.XPath("//*[@data-test='PersonPageNavigation']")).Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Wait for person page loads
            Thread.Sleep(1000);

            // Check that the initial salary is as expected
            var salaryLabelBefore = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='DisplayedSalary']")));
            var initialSalary = double.Parse(salaryLabelBefore.Text);
            initialSalary.Should().BeApproximately(5000, 0.001);

            // Fill in the percentage input field
            var input = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='SalaryIncreasePercentageInput']")));
            input.Clear();
            input.SendKeys(percentage.ToString());

            // Short wait after input to ensure the value is set
            Thread.Sleep(500);

            // Submit the form
            var submitButton = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='SalaryIncreaseSubmitButton']")));
            submitButton.Click();

            // Wait for the updated salary
            Thread.Sleep(1000);

            // Check that the updated salary is correct
            var salaryLabelAfter = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='DisplayedSalary']")));
            var salaryAfterSubmission = double.Parse(salaryLabelAfter.Text);
            salaryAfterSubmission.Should().BeApproximately(expectedSalary, 0.001);
        }

        [Test]
        public void Person_InvalidPercentageBelowMinus10_ShouldShowValidationErrors()
        {
            // Navigate to the base URL
            driver.Navigate().GoToUrl(BaseURL);
            Thread.Sleep(1000);

            // Go to the Person page
            driver.FindElement(By.XPath("//*[@data-test='PersonPageNavigation']")).Click();
            Thread.Sleep(1000);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Input -15%
            var input = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-test='SalaryIncreasePercentageInput']")));
            input.Clear();
            input.SendKeys("-15");

            // Submit the form
            var submitButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-test='SalaryIncreaseSubmitButton']")));
            submitButton.Click();
            Thread.Sleep(1000); // Allow time for validation to occur

            // // Wait for the validation summary list (<ul>) to appear anywhere inside the form, created by ValidationSummary, 
            var summaryError = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//form//ul[contains(@class, 'validation-errors')]")));
            summaryError.Text.Should().Contain("between -10 and infinity");

            // Wait for the inline validation message (<div>) that appears below the input field, created by ValidationMessage
            var fieldError = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//div[contains(@class, 'validation-message')]")));
            fieldError.Text.Should().Contain("between -10 and infinity");
        }




        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}