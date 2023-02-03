using NUnit.Framework;
using OpenQA.Selenium;

namespace Helpers
{
    public class TestHelper
    {
        public static void DoOnTimeout(TestDelegate throwsTimeout, TestDelegate catchAction)
        {
            try
            {
                _ = throwsTimeout;
            }
            catch (WebDriverTimeoutException)
            {
                _ = catchAction;
            }
        }
    }
}