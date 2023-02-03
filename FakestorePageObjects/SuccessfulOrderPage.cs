using Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakestorePageObjects
{
    public class SuccessfulOrderPage : BasePage
    {
        public IWebElement EntryHeader => driver.FindElement(By.CssSelector(".entry-header"), 2);

        public SuccessfulOrderPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) {}
    }
}
