using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace DatesAndStuff.Mobile.Tests;

[TestFixture]
public class EmagStandaloneTest
{
    private AndroidDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new AppiumOptions();
        options.PlatformName = "Android";
        options.AutomationName = "UIAutomator2";
       // options.AddAdditionalAppiumOption("deviceName", "emulator-5554");
        options.AddAdditionalAppiumOption("appPackage", "ro.emag.android");
        options.AddAdditionalAppiumOption("appActivity", "ro.emag.android.cleancode.app.ActivityStart");
        options.AddAdditionalAppiumOption("noReset", true);

        driver = new AndroidDriver(new Uri("http://127.0.0.1:4723"), options);
    }

    [TearDown]
    public void Cleanup()
    {
        driver?.Quit();
        driver?.Dispose();
    }

    [Test]
    public void AddOnePlusToCart_WithScreenRecording()
    {
        if (driver == null)
            Assert.Fail("Driver not initialized");


        driver.StartRecordingScreen();

        // Search for "OnePlus Nord"
        // Tap on the search area to activate input
        var searchView = driver.FindElement(MobileBy.XPath("//android.view.ViewGroup[@resource-id='ro.emag.android:id/viewHomeSearch']"));
        searchView.Click();

        // Type into the search field
        Thread.Sleep(2000);
        var searchInput = driver.FindElement(MobileBy.XPath("//android.widget.EditText[@resource-id='ro.emag.android:id/etSearchQuery']"));
        searchInput.SendKeys("consola ps5");

        Thread.Sleep(3000);

        var searchResult = driver.FindElement(MobileBy.XPath("//android.widget.TextView[@resource-id=\"ro.emag.android:id/tvSearchSuggestion\" and @text=\"consola ps5\"]"));
        searchResult.Click();

        // Select a result
        Thread.Sleep(5000);
        var firstItem = driver.FindElement(MobileBy.XPath("(//android.widget.FrameLayout[@resource-id=\"ro.emag.android:id/parent\"])[1]/android.view.ViewGroup"));
        firstItem.Click();

        Thread.Sleep(2000);

        // Add to cart
        var addToCartButton = driver.FindElement(MobileBy.XPath("//androidx.recyclerview.widget.RecyclerView[@resource-id='ro.emag.android:id/rvProductDetailsContent']/android.widget.FrameLayout[2]/android.widget.LinearLayout"));
        addToCartButton.Click();

        // Open cart tab 
        Thread.Sleep(500);

        var cartTab = driver.FindElement(MobileBy.XPath("(//android.widget.ImageView[@resource-id='ro.emag.android:id/navigation_bar_item_icon_view'])[3]"));
        cartTab.Click();

        Thread.Sleep(2500);

        var videoBase64 = driver.StopRecordingScreen();
        File.WriteAllBytes("emag_order_test.mp4", Convert.FromBase64String(videoBase64));
    }
}
