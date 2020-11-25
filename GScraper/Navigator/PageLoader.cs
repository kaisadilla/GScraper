using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading.Tasks;

namespace Kaisa.GScraper.Navigator {
    public class PageLoader {
        private ChromeOptions options = new ChromeOptions();
        private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headless">True by default. If true, Selenium won't open a Chrome window to load a page.</param>
        /// <param name="hideCmd">True by default. If true, Selenium won't open a cmd console to load a page.
        /// The cmd console is disrupted and can freeze or behave strangely if the user is using the computer while
        /// it runs, so it's adviced not to show it.</param>
        public PageLoader(bool headless = true, bool hideCmd = true) {
            if (headless) options.AddArgument("headless");
            if (hideCmd) driverService.HideCommandPromptWindow = true;
        }

        /// <summary>
        /// Loads a webpage, waits until a condition is met, and only then returns the source code for the page.
        /// </summary>
        /// <param name="url">The url of the page to load</param>
        /// <param name="conds">A condition to be met, usually created with SeleniumExtras.WaitHelpers.ExpectedConditions class</param>
        /// <param name="maxTries">The maximum number of tries if loading the page keeps failing.</param>
        public HtmlNode LoadDynamicWebpage(string url, Func<IWebDriver, IWebElement> conds, int maxTries = 10) {
            int timeout = 15 + (5 * (10 - maxTries));
            IWebDriver driver = new ChromeDriver(driverService, options, TimeSpan.FromSeconds(timeout));

            try {
                Console.WriteLine($"Loading webpage {url}");
                using (driver) {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    driver.Navigate().GoToUrl(url);
                    wait.Until(conds);
                    Console.WriteLine("Page loaded successfully.");
                    return HtmlNode.CreateNode(driver.PageSource);
                }
            }
            catch (Exception ex) when (ex is WebDriverException || ex is InvalidOperationException) {
                driver?.Close(); // TODO: Check if this prevents chromexplorer.exe processes from piling up when they crash.
                if (maxTries > 0) {
                    Console.WriteLine($"Error loading page {url}: {ex.Message}. {maxTries - 1} tries left.");
                    return LoadDynamicWebpage(url, conds, maxTries--);
                }
                else {
                    Console.WriteLine($"Error loading page. Page not loaded.");
                    return null;
                }
            }
        }
    }
}
