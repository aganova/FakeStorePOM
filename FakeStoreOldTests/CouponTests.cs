using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace FakeStoreOldTests
{
    class CouponTests
    {
        RemoteWebDriver driver;
        string baseURL = "https://fakestore.testelka.pl";
        IWebElement DismissNoticeLink => driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link"));
        IList<string> productsURLs = new List<string>() {
            "/product/gran-koscielcow/",
            "/product/windsurfing-w-lanzarote-costa-teguise/"
        };
        IList<float> productsPrices = new List<float>() {
            2999.99f,
            3000
        };

        IWebElement AddToCartButton => driver.FindElement(By.CssSelector("[name='add-to-cart']"), 2);
        IWebElement GoToCartButton => driver.FindElement(By.CssSelector(".woocommerce-message .wc-forward"), 2);
        IWebElement CartTable => driver.FindElement(By.CssSelector("table.shop_table.cart"), 2);
        IWebElement CouponCodeField => driver.FindElement(By.CssSelector("#coupon_code"), 2);
        IWebElement ConfirmCouponButton => driver.FindElement(By.CssSelector("[name='apply_coupon']"), 2);
        IWebElement ErrorsList => driver.FindElement(By.CssSelector("ul.woocommerce-error"), 2);
        IList<IWebElement> ErrorMessagesElements => driver.FindElements(By.CssSelector("ul.woocommerce-error li"), 2);
        IWebElement SuccessMessage => driver.FindElement(By.CssSelector("div.woocommerce-message"), 2);
        IWebElement CartDiscountRow => driver.FindElement(By.CssSelector("tr.cart-discount"), 2);
        IWebElement CouponNameField => CartDiscountRow.FindElement(By.CssSelector("th"), 2);
        IWebElement CouponAmountField => CartDiscountRow.FindElement(By.CssSelector("span.amount"), 2);
        IWebElement OrderTotalWithoutTaxElement => driver.FindElement(By.CssSelector(".order-total td strong"), 2);

        By Loaders => By.CssSelector(".blockUI");


        string minimalValueCouponName = "kwotowy300"; //Minimum 3000 zł in cart for this coupon
        int minimalValueCouponValue = 300;
        string percentCouponName = "10procent";
        float percentCouponValue = 0.1f;
        string categoryCouponName = "windsurfing350";
        int categoryCouponValue = 350;
        string oldCouponName = "starośćnieradość";

        [SetUp]
        public void Setup()
        {
            DriverOptions options = new ChromeOptions();
            //DriverOptions options = new FirefoxOptions();
            driver = new RemoteWebDriver(options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);            
            driver.Navigate().GoToUrl(baseURL);
            DismissNoticeLink.Click();
        }
        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        [Test]
        public void MinimalValueCouponTest() {
            driver.Navigate().GoToUrl(baseURL + productsURLs[1]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(minimalValueCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {                
                Assert.AreEqual("Kupon został pomyślnie użyty.", SuccessMessage.Text, "Success message is not correct. Did you get right message?");
                Assert.AreEqual("Kupon: " + minimalValueCouponName, CouponNameField.Text, "Coupon name is not correct.");
                Assert.AreEqual("300,00 zł", CouponAmountField.Text, "Coupon amount is not correct.");
                Assert.AreEqual(formatNumber(productsPrices[1] - minimalValueCouponValue), OrderTotalWithoutTaxElement.Text, "Total is not correct. Was the coup applied?");
            });
        }
        [Test]
        public void TooSmallSumForMinimalValueCouponTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(minimalValueCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, ErrorMessagesElements.Count, "Number of error messages is not 1.");
                Assert.AreEqual("Minimalna wartość zamówienia dla tego kuponu to 3 000,00 zł.", ErrorMessagesElements[0].Text, "Error message is not correct. Did you get right message?");
                Assert.Throws<WebDriverTimeoutException>(() => _ = CartDiscountRow, "Cart discount element was found in cart summary.");
            });
        }

        [Test]
        public void PercentCouponTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(percentCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            float couponValue = productsPrices[0] * percentCouponValue;
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Kupon został pomyślnie użyty.", SuccessMessage.Text, "Success message is not correct. Did you get right message?");
                Assert.AreEqual("Kupon: " + percentCouponName, CouponNameField.Text, "Coupon name is not correct.");
                Assert.AreEqual(formatNumber(couponValue), CouponAmountField.Text, "Coupon amount is not correct.");
                Assert.AreEqual(formatNumber(productsPrices[0] - couponValue), OrderTotalWithoutTaxElement.Text, "Total is not correct. Was the coupon applied?");
            });
        }

        [Test]
        public void CategoryCouponTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[1]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(categoryCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Kupon został pomyślnie użyty.", SuccessMessage.Text, "Success message is not correct. Did you get right message?");
                Assert.AreEqual("Kupon: " + categoryCouponName, CouponNameField.Text, "Coupon name is not correct.");
                Assert.AreEqual(formatNumber(categoryCouponValue), CouponAmountField.Text, "Coupon amount is not correct.");
                Assert.AreEqual(formatNumber(productsPrices[1] - categoryCouponValue), OrderTotalWithoutTaxElement.Text, "Total is not correct. Was the coup applied?");
            });
        }

        [Test]
        public void WrongCategoryCouponTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(oldCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, ErrorMessagesElements.Count, "Number of error messages is not 1.");
                Assert.AreEqual("Ten kupon stracił ważność.", ErrorMessagesElements[0].Text, "Error message is not correct. Did you get right message?");
                Assert.Throws<WebDriverTimeoutException>(() => _ = CartDiscountRow, "Cart discount element was found in cart summary.");
            });
        }

        [Test]
        public void OldCouponTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            CouponCodeField.SendKeys(categoryCouponName);
            ConfirmCouponButton.Click();
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, ErrorMessagesElements.Count, "Number of error messages is not 1.");
                Assert.AreEqual("Przepraszamy, tego kuponu nie można zastosować do wybranych produktów.", ErrorMessagesElements[0].Text, "Error message is not correct. Did you get right message?");
                Assert.Throws<WebDriverTimeoutException>(() => _ = CartDiscountRow, "Cart discount element was found in cart summary.");
            });
        }
        private void waitForElementsDisappear(By by)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(d => driver.FindElements(by).Count == 0);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Elements located by " + by + " didn't disappear in 10 seconds.");
                throw;
            }
        }
        private string formatNumber(float number)
        {
            if (number < 1000) { 
                return string.Format("{0:###.00}", number) + " zł";
            }
            else return string.Format("{0:### ###.00}", number) + " zł";
        }
    }
}
