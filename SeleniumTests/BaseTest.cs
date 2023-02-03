using FakestorePageObjects;
using Helpers;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumTests.Config;
using SeleniumTests.TestData;

namespace SeleniumTests
{
    public class BaseTest
    {
        protected IWebDriver driver;

        protected Configuration config;

        protected Data testData;

        [OneTimeSetUp]
        public void Setup()
        {
            SetupConfig();
            SetupTestData();
        }

        [SetUp]
        public void SetupDriver()
        {
            driver = new DriverFactory().Create(config.Browser, config.IsRemote, config.RemoteAddress);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);

            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            //wait.Until(d => DismissNoticeLink.Displayed);

            MainPage mainPage = new MainPage(driver, config.BaseUrl);
            mainPage.GoTo().DismissNotice();
        }

        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        private void SetupConfig()
        {
            config = new Configuration();
            IConfiguration configurationFile = new ConfigurationBuilder().AddJsonFile(@"Config\configuration.json").Build();
            configurationFile.Bind(config);
        }

        private void SetupTestData()
        {
            testData = new Data();
            IConfiguration testDataFile = new ConfigurationBuilder().AddJsonFile(@"TestData\testData.json").Build();
            testDataFile.Bind(testData);
        }
    }
}
