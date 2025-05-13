using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DatesAndStuff.Web.Tests
{
    [TestFixture]
    public class WizzAirTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test]
        public void Should_Find_Two_Flights_From_TGM_To_BUD_NextWeek()
        {
            driver.Navigate().GoToUrl("https://wizzair.com");

            TryClick(By.Id("onetrust-accept-btn-handler"));
            RandomDelay();

            TryClick(By.CssSelector("input[data-test='oneway']"));
            RandomDelay();

            // FROM
            var fromInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[data-test='search-departure-station']")));
            fromInput.Click();
            HumanType(fromInput, " TGM");
            var fromOption = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[contains(., 'Tirgu Mures')]")));
            fromOption.Click();
            RandomDelay();

            // TO
            var toInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[data-test='search-arrival-station']")));
            toInput.Click();
            HumanType(toInput, "Bud");
            var toOption = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//label[contains(., 'Budapest')]")));
            toOption.Click();
            RandomDelay();

            TryClick(By.XPath("//input[@placeholder='Departure' and @readonly]"));
            Thread.Sleep(1000);

            var today = DateTime.Today;
            int daysToMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            var nextMonday = today.AddDays(daysToMonday == 0 ? 7 : daysToMonday);
            var ariaLabel = nextMonday.ToString("dddd d MMMM yyyy", new CultureInfo("en-US"));

            TryClick(By.XPath($"//span[@role='button' and @aria-label='{ariaLabel}']"));
            RandomDelay();

            TryClick(By.CssSelector("button[data-test='flight-search-submit']"));
            RandomDelay();

            string urlDate = nextMonday.ToString("yyyy-MM-dd");
            driver.Navigate().GoToUrl($"https://wizzair.com/en-gb/booking/select-flight/TGM/BUD/{urlDate}");

            // Check if there are 2 available flights for the next week
            var columns = wait.Until(d => d.FindElements(By.XPath("//div[contains(@class,'columns-inner')]//div[contains(@class,'column')]")));
            var validFlightDays = new List<DateTime>();

            foreach (var column in columns)
            {
                var timeAttr = column.FindElement(By.XPath(".//time")).GetAttribute("datetime");
                if (DateTime.TryParse(timeAttr, out var date))
                {
                    bool hasFlight = !column.GetAttribute("class").Contains("is-no-flight");
                    if (date >= nextMonday && date < nextMonday.AddDays(7) && hasFlight)
                    {
                        validFlightDays.Add(date);
                    }
                }
            }

            // Check if there are atleast 2 flights
            validFlightDays.Count.Should().BeGreaterThan(1, "because there should be more than 2 flights between TGM and BUD in the next week");

            // Price check and screenshot
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            int priceThreshold = 250;

            foreach (var flightDate in validFlightDays)
            {
                // Navigate to the flight selection page for each valid date
                string dateStr = flightDate.ToString("yyyy-MM-dd");
                driver.Navigate().GoToUrl($"https://wizzair.com/en-gb/booking/select-flight/TGM/BUD/{dateStr}");
                RandomDelay();

                try
                {
                    wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[contains(@class,'flight-select__flight__container')]")));

                    var priceElement = driver.FindElement(By.XPath("//div[contains(@class, 'current-price')]"));
                    string rawText = priceElement.Text;
                    string digitsOnly = new string(rawText.Where(char.IsDigit).ToArray());

                    // Check if the price is below the threshold and save the screentshot if it is
                    if (int.TryParse(digitsOnly, out int price) && price < priceThreshold)
                    {
                        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                        string fileName = $"flight_{flightDate:yyyyMMdd}_price_{price}.png";
                        string filePath = Path.Combine(desktopPath, fileName);

                        screenshot.SaveAsFile(filePath);
                        Console.WriteLine($"Screenshot saved: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not process {dateStr}: {ex.Message}");
                }
            }
        }

        private void TryClick(By by)
        {
            try
            {
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(by));
                element.Click();
            }
            catch { }
        }

        private void RandomDelay()
        {
            Thread.Sleep(new Random().Next(900, 1400));
        }

        private void HumanType(IWebElement element, string text)
        {
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(new Random().Next(80, 130));
            }
        }
    }
}
