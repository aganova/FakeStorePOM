using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class DriverFactory
    {
        private IWebDriver driver;

        public IWebDriver Create(string browser, bool isRemote, Uri remoteAddress = null, string platformName = null)
        {
            if (isRemote)
            {
                return GetRemoteDriver(browser, remoteAddress, platformName);
            }
            else
            {
                return GetLocalDriver(browser);
            }
        }

        private IWebDriver GetLocalDriver(string browser)
        {
            switch (browser)
            {
                case "chrome":
                    driver = new ChromeDriver();
                    break;
                case "firefox":
                    driver = new FirefoxDriver();
                    break;
                default:
                    throw new ArgumentException("Provided browser: " + browser + " is not supported. Available: chrome, firefox");
            }
            return driver;
        }

        private IWebDriver GetRemoteDriver(string browser, Uri remoteAddress, string platformName = null)
        {
            DriverOptions options;
            switch (browser)
            {
                case "chrome":
                    options = new ChromeOptions
                    {
                        PlatformName = platformName
                    };
                    ((ChromeOptions)options).AddArgument("--headless=new");
                    break;
                case "firefox":
                    options = new FirefoxOptions
                    {
                        PlatformName = platformName
                    };
                    break;
                default:
                    throw new ArgumentException("Provided browser: " + browser + " is not supported. Available: chrome, firefox");
            }

            _ = remoteAddress != null ? driver = new RemoteWebDriver(remoteAddress, options) : driver = new RemoteWebDriver(options);
            return driver;
        }
    }
}

