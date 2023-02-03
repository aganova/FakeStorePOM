using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FakestorePageObjects
{
    public abstract class BasePage
    {
        protected IWebDriver driver;
        protected readonly string baseUrl = "https://fakestore.testelka.pl";

        private By Loaders => By.CssSelector(".blockUI");

        private IWebElement DismissNoticeLink => driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link"), 2);

        public BasePage(IWebDriver driver, string baseUrl)
        {
            this.driver = driver;
            this.baseUrl = baseUrl;
        }

        protected void WaitForLoadersDisappear(int timeoutInSeconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            try
            {
                wait.Until(d => driver.FindElements(Loaders).Count == 0);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Elements located by " + Loaders + $" didn't disappear in {timeoutInSeconds} seconds.");
                throw;
            }
        }

        public void DismissNotice()
        {
            DismissNoticeLink.Click();
        }
    }
}
