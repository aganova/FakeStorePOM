using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace FakestorePageObjects
{
    public class MainPage : BasePage
    {
        public MainPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) { }

        public MainPage GoTo()
        {
            driver.Navigate().GoToUrl(baseUrl);
            return this;
        }
    }
}