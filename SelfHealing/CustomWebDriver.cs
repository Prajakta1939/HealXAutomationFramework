using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SelfHealing
{
    public class CustomWebDriver : IWebDriver, ITakesScreenshot, IJavaScriptExecutor
    {
        public static IWebDriver driver;

        public CustomWebDriver(IWebDriver delegateDriver)
        {
            driver = delegateDriver;
        }

        public CustomWebDriver()
        {
        }

        public IWebElement FindElement(By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                string locatorName = null;
                try
                {
                    var stackTrace = new System.Diagnostics.StackTrace();
                    var callerFrame = stackTrace.GetFrame(2); // index may vary
                    var method = callerFrame.GetMethod();
                    var callerType = method.DeclaringType;

                    object instance = Activator.CreateInstance(callerType);
                    MethodInfo getLocatorNameMethod = callerType.GetMethod("GetLocatorName", new[] { typeof(By) });

                    locatorName = (string)getLocatorNameMethod.Invoke(instance, new object[] { by });

                    Console.WriteLine(locatorName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to find the locator Name using StackTrace");
                    throw new Exception("Locator name resolution failed", ex);
                }

                if (!string.IsNullOrEmpty(locatorName))
                {
                    NormalRun nr = new NormalRun(driver);
                    IWebElement healedElement = nr.GetElement(locatorName, by);
                    if (healedElement != null)
                    {
                        return healedElement;
                    }
                }

                Console.WriteLine("Not found the following Locator: " + locatorName);
            }

            return null;
        }

        public void Get(string url) => driver.Url = url;

        public string Url
        {
            get => driver.Url;
            set => driver.Url = value;
        }

        public string Title => driver.Title;

        public string PageSource => driver.PageSource;

        public string CurrentWindowHandle => driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => driver.WindowHandles;

        public void Close() => driver.Close();

        public void Quit() => driver.Quit();

        public IOptions Manage() => driver.Manage();

        public INavigation Navigate() => driver.Navigate();

        public ITargetLocator SwitchTo() => driver.SwitchTo();

        public IReadOnlyCollection<IWebElement> FindElements(By by) => driver.FindElements(by);

        public Screenshot GetScreenshot()
        {
            return ((ITakesScreenshot)driver).GetScreenshot();
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript(script, args);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)driver).ExecuteAsyncScript(script, args);
        }

        // Dispose for IWebDriver
        public void Dispose()
        {
            driver.Dispose();
        }
    }
}
