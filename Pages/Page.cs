using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;

namespace SelfHealing.Pages
{
    public class Page : Details
    {
        private CustomWebDriver driver;
        private WebDriverWait wait;

        // Locators
        private By RelLogo = By.CssSelector("img[class='img-fluid logo']");
        private By searchBarRel = By.CssSelector("div[class='search-wrapper-cs'] a[aria-label='Close']");

        public void Initialize(CustomWebDriver driver, WebDriverWait wait)
        {
            this.driver = driver;
            this.wait = wait;
        }

        public void ValidateHomePage()
        {
            driver.FindElement(RelLogo);
        }

        public void ClickBtn()
        {
            driver.FindElement(searchBarRel).Click();
        }

        public string GetLocatorName(By locator)
        {
            Type type = this.GetType();
            while (type != null)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(this);
                    if (fieldValue != null && fieldValue.Equals(locator))
                    {
                        return field.Name;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
