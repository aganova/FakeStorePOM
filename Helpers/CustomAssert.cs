using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class CustomAssert
    {
        public static void ThrowsWebDriverTimeoutException(TestDelegate code, string message)
        {
            Assert.Throws<WebDriverTimeoutException>(code, message);
        }
    }
}
