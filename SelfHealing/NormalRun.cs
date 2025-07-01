using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SelfHealing
{
    public class NormalRun : CustomWebDriver
    {
        public static IWebDriver driver;
        private WebDriverWait wait;

        public NormalRun(IWebDriver driver)
        {
            NormalRun.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        CustomWebDriver cw = new CustomWebDriver(driver);

        

        public IWebElement? GetElement(string locatorName, By locatorByType)
        {
            RemoteDB db = new RemoteDB();

            string locatorValue = db.getLocator(locatorName);
            string locatorType = db.getLocatorType(locatorName);
            IWebElement? element = null;

            Console.WriteLine("Element not found using stored locator, Trying to heal");

            // Try alternate locator
            string alternateLocator = db.getAlternateLocator(locatorName);
            if (!string.IsNullOrEmpty(alternateLocator))
            {
                try
                {
                    element = driver.FindElement(By.CssSelector(alternateLocator));
                    if (element != null)
                    {
                        Console.WriteLine("Element Found using Alternate Locator: " + alternateLocator);
                        return element;
                    }
                }
                catch (NoSuchElementException)
                {
                    // Alternate locator didn't work, proceed to healing
                }
            }

            // Try healing using attributes
            HealX healingObj = new HealX(driver);
            string healedLoc = healingObj.getHealedLocatorUsingAttributes(locatorName);

            if (!string.IsNullOrEmpty(healedLoc))
            {
                db.setAlternateLocator(locatorName, healedLoc);
                Console.WriteLine("Locator Found using attributes: " + healedLoc);
                try
                {
                    element = driver.FindElement(By.CssSelector(healedLoc));
                    return element;
                }
                catch (NoSuchElementException)
                {
                    // Fall through
                }
            }

            // Try healing using coordinates
            element = healingObj.getHealedWebElementUsingPosition(locatorName);
            if (element != null)
            {
                Console.WriteLine("Locator Found using positional coordinates.");
                return element;
            }

            Console.WriteLine("Unable to find a healed locator. Please ensure a successful FirstRun.");
            return null;
        }
    }
}
