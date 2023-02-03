using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FakeStoreOldTests
{
    class CheckoutTests
    {
        RemoteWebDriver driver;        

        string baseURL = "https://fakestore.testelka.pl";
        IList<string> productsURLs = new List<string>() {
            "/product/wyspy-zielonego-przyladka-sal/",
            "/product/zmien-swoja-sylwetke-yoga-na-malcie/"
        };
        IList<string> productsPricesText = new List<string>() {
            "5 399,00 zł",
            "3 299,00 zł"
        };

        IList<int> productsPrices = new List<int>() {
            5399,
            3299
        };

        IWebElement AddToCartButton => driver.FindElement(By.CssSelector("[name='add-to-cart']"), 2);
        IWebElement GoToCartButton => driver.FindElement(By.CssSelector(".woocommerce-message .wc-forward"), 2);
        IWebElement GoToCheckoutButton => driver.FindElement(By.CssSelector(".checkout-button"), 2);
        IWebElement DismissNoticeLink => driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link"), 2);
        IWebElement CardNumberFrame => driver.FindElement(By.CssSelector("#stripe-card-element iframe"), 2);
        IWebElement CardNumberInput => driver.FindElement(By.CssSelector("input[name='cardnumber']"), 5);
        IWebElement CardExpirationDateFrame => driver.FindElement(By.CssSelector("#stripe-exp-element iframe"), 2);
        IWebElement CardExpirationDateInput => driver.FindElement(By.CssSelector("input[name='exp-date']"), 2);
        IWebElement CardCvcFrame => driver.FindElement(By.CssSelector("#stripe-cvc-element iframe"), 2);
        IWebElement CardCvcInput => driver.FindElement(By.CssSelector("input[name='cvc']"), 2);
        IWebElement PlaceOrderButton => driver.FindElement(By.CssSelector("button#place_order"), 2);
        IWebElement ErrorsList => driver.FindElement(By.CssSelector("ul.woocommerce-error"), 2);
        IList<IWebElement> ErrorMessagesElements => driver.FindElements(By.CssSelector("ul.woocommerce-error li"), 2);
        IWebElement ProductTotalElement => driver.FindElement(By.CssSelector(".product-total bdi"), 5);
        IList<IWebElement> ProductTotalElements => driver.FindElements(By.CssSelector(".product-total bdi"), 2);
        IWebElement CartSubtotalElement => driver.FindElement(By.CssSelector(".cart-subtotal bdi"), 2);
        IWebElement OrderTotalElement => driver.FindElement(By.CssSelector(".order-total td"), 2);
        IWebElement QuantityField => driver.FindElement(By.CssSelector("input.qty"), 2);
        IWebElement CartTable => driver.FindElement(By.CssSelector("table.shop_table.cart"), 2);
        IWebElement UpdateCartButton => driver.FindElement(By.CssSelector("[name='update_cart']"), 2);
        IWebElement BillingFirstNameInput => driver.FindElement(By.CssSelector("input#billing_first_name"), 2);
        IWebElement BillingLastNameInput => driver.FindElement(By.CssSelector("input#billing_last_name"), 2);
        IWebElement BillingAddressInput => driver.FindElement(By.CssSelector("input#billing_address_1"), 2);
        IWebElement BillingPostcodeInput => driver.FindElement(By.CssSelector("input#billing_postcode"), 2);
        IWebElement BillingCityInput => driver.FindElement(By.CssSelector("input#billing_city"), 2);
        IWebElement BillingPhoneInput => driver.FindElement(By.CssSelector("input#billing_phone"), 2);
        IWebElement BillingEmailInput => driver.FindElement(By.CssSelector("input#billing_email"), 2);
        IWebElement BillingTermsCheckbox => driver.FindElement(By.CssSelector("input#terms"), 2);
        IWebElement ShowLoginLink => driver.FindElement(By.CssSelector("a.showlogin"), 2);
        IWebElement UsernameField => driver.FindElement(By.CssSelector("#username"), 2);
        IWebElement PasswordField => driver.FindElement(By.CssSelector("#password"), 2);
        IWebElement LoginButton => driver.FindElement(By.CssSelector("[name='login']"), 2);
        IWebElement EntryHeader => driver.FindElement(By.CssSelector(".entry-header"), 2);
        IWebElement LoginForm => driver.FindElement(By.CssSelector(".login[style='']"), 3);
        IWebElement CheckoutForm => driver.FindElement(By.CssSelector("[name='checkout']"), 2);

        By Loaders => By.CssSelector(".blockUI");

        string cardNumber = "4242424242424242";
        string cardExpirationDate = "0222";
        string cardCvc = "222";

        [SetUp]
        public void Setup()
        {
            //DriverOptions options = new ChromeOptions();
            DriverOptions options = new FirefoxOptions();
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
        public void FieldsValidationTests()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToCheckoutButton.Click();
            _ = CheckoutForm;
            driver.SwitchTo().Frame(CardNumberFrame);
            CardNumberInput.SendKeys(cardNumber);
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(CardExpirationDateFrame);
            CardExpirationDateInput.SendKeys(cardExpirationDate);
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(CardCvcFrame);
            CardCvcInput.SendKeys(cardCvc);
            driver.SwitchTo().DefaultContent();
            PlaceOrderButton.Click();

            IList<string> errorMessages = ErrorMessagesElements.Select(element => element.Text).ToList();
            IList<string> expectedErrorMessages = new List<string> { 
                "Imię płatnika jest wymaganym polem.", 
                "Nazwisko płatnika jest wymaganym polem.", 
                "Ulica płatnika jest wymaganym polem.", 
                "Kod pocztowy płatnika nie jest prawidłowym kodem pocztowym.", 
                "Miasto płatnika jest wymaganym polem.", 
                "Telefon płatnika jest wymaganym polem.", 
                "Adres email płatnika jest wymaganym polem.", 
                "Proszę przeczytać i zaakceptować regulamin sklepu aby móc sfinalizować zamówienie." 
            };
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => _ = ErrorsList, "Error list was not found. There was no validation error.");
                //Assert.AreEqual(8, ErrorMessagesElements.Count, "Number of error messages is not correct.");
                Assert.AreEqual(expectedErrorMessages.OrderBy(message => message), errorMessages.OrderBy(message => message));                
            });            
        }

        [Test]
        public void ReviewOrderOneProductTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToCheckoutButton.Click();
            _ = CheckoutForm;            
            waitForElementsDisappear(Loaders);
            float tax = calculateTax(productsPrices[0]);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(productsPricesText[0], ProductTotalElement.Text);
                Assert.AreEqual(productsPricesText[0], CartSubtotalElement.Text);
                Assert.AreEqual(productsPricesText[0] + " (zawiera " + formatNumber(tax) + " VAT)", OrderTotalElement.Text);
            });
        }
        [Test]
        public void ReviewOrderMultipleProductsTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            QuantityField.Clear();
            QuantityField.SendKeys("2");
            AddToCartButton.Click();
            _ = GoToCartButton;
            driver.Navigate().GoToUrl(baseURL + productsURLs[1]);
            QuantityField.Clear();
            QuantityField.SendKeys("3");
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToCheckoutButton.Click();
            _ = CheckoutForm;
            
            waitForElementsDisappear(Loaders);
            float totalPrice = productsPrices[0] * 2 + productsPrices[1] * 3;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(formatNumber(productsPrices[0] * 2), ProductTotalElements[0].Text);
                Assert.AreEqual(formatNumber(productsPrices[1] * 3), ProductTotalElements[1].Text);
                Assert.AreEqual(formatNumber(totalPrice), CartSubtotalElement.Text);
                Assert.AreEqual(formatNumber(totalPrice) + " (zawiera " + formatNumber(calculateTax(totalPrice)) + " VAT)", OrderTotalElement.Text);
            });
        }
        [Test]
        public void ChangingNumberOfItemsTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            QuantityField.Clear();
            QuantityField.SendKeys("2");
            UpdateCartButton.Click();
            waitForElementsDisappear(Loaders);
            
            GoToCheckoutButton.Click();
            
            //Chrome issue with clicking on the button
            if ((string)driver.Capabilities.GetCapability("browserName") == "chrome")
            {
                try
                {
                    _ = CheckoutForm;
                }
                catch (WebDriverTimeoutException)
                {
                    GoToCheckoutButton.Click();
                    _ = CheckoutForm;
                }
            }
            else _ = CheckoutForm;

            float total = productsPrices[0] * 2;
            float tax = calculateTax(total);
            waitForElementsDisappear(Loaders);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(formatNumber(total), ProductTotalElement.Text);
                Assert.AreEqual(formatNumber(total), CartSubtotalElement.Text);
                Assert.AreEqual(formatNumber(total) + " (zawiera " + formatNumber(tax) + " VAT)", OrderTotalElement.Text);
            });
        }

        [Test]
        public void SuccesfullPaymentTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToCheckoutButton.Click();
            _ = CheckoutForm;
            driver.SwitchTo().Frame(CardNumberFrame);
            CardNumberInput.SendKeys(cardNumber);
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(CardExpirationDateFrame);
            CardExpirationDateInput.SendKeys(cardExpirationDate);
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(CardCvcFrame);
            CardCvcInput.SendKeys(cardCvc);
            driver.SwitchTo().DefaultContent();

            BillingFirstNameInput.SendKeys("Karolina");
            BillingLastNameInput.SendKeys("Kowalska");
            BillingAddressInput.SendKeys("ul. Kwiatowa 12/44");
            BillingPostcodeInput.SendKeys("31-333");
            BillingCityInput.SendKeys("Kraków");
            BillingPhoneInput.SendKeys("6666666");
            BillingEmailInput.SendKeys("hoho@hohoho.pl");
            waitForElementsDisappear(Loaders);
            BillingTermsCheckbox.Click();
            PlaceOrderButton.Click();            
            waitForElementsDisappear(Loaders);

            Assert.AreEqual("Zamówienie otrzymane", EntryHeader.Text, "Page header is not what expected. Order was not sucessful.");
        }

        [Test]
        public void SuccesfullPaymentExistingUserTest()
        {
            driver.Navigate().GoToUrl(baseURL + productsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToCheckoutButton.Click();
            _ = CheckoutForm;
            ShowLoginLink.Click();
            _ = LoginForm;
            UsernameField.SendKeys("kisoroc826@dmeproject.com");
            PasswordField.SendKeys("bardzotestowehasło");
            LoginButton.Click();
            waitForElementsDisappear(Loaders);
            BillingTermsCheckbox.Click();
            PlaceOrderButton.Click();
            waitForElementsDisappear(Loaders);

            Assert.AreEqual("Zamówienie otrzymane", EntryHeader.Text, "Page header is not what expected. Order was not sucessful.");
        }

        private float calculateTax(float total)
        {
            return (float)Math.Round(total - (total / 1.23), 2);
        }

        private string formatNumber(float number)
        {
            return string.Format("{0:### ###.00}", number) + " zł";
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

    }
}
