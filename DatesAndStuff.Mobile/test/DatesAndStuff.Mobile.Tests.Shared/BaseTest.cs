using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;

namespace DatesAndStuff.Mobile.Tests;

public abstract class BaseTest
{
    protected AppiumDriver App => AppiumSetup.App;

    // This could also be an extension method to AppiumDriver if you prefer

    protected AppiumElement FindUIElement(string id)
    {
        if (App is WindowsDriver)
        {
            return App.FindElement(MobileBy.AccessibilityId(id));
        }

        return App.FindElement(MobileBy.Id(id));
    }

    [SetUp]
    public void RelaunchApp()
    {
        var appId = "com.BBTE.VerVal";

        App.ExecuteScript("mobile: terminateApp", new Dictionary<string, object>
        {
            { "appId", appId }
        });

        App.ExecuteScript("mobile: activateApp", new Dictionary<string, object>
        {
            { "appId", appId }
        });

        // Wait until main UI is available
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(10));
        wait.Until(driver => driver.FindElement(MobileBy.XPath("//android.widget.ImageButton[@content-desc='Open navigation drawer']")));
    }
}