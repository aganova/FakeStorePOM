using Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace FakestorePageObjects
{
    public class CheckoutPage : BasePage
    {
        private IWebElement CardNumberFrame => driver.FindElement(By.CssSelector("#stripe-card-element iframe"), 2);
        private IWebElement CardNumberInput => driver.FindElement(By.CssSelector("input[name='cardnumber']"), 5);
        private IWebElement CardExpirationDateFrame => driver.FindElement(By.CssSelector("#stripe-exp-element iframe"), 2);
        private IWebElement CardExpirationDateInput => driver.FindElement(By.CssSelector("input[name='exp-date']"), 2);
        private IWebElement CardCvcFrame => driver.FindElement(By.CssSelector("#stripe-cvc-element iframe"), 2);
        private IWebElement CardCvcInput => driver.FindElement(By.CssSelector("input[name='cvc']"), 2);
        public IWebElement CheckoutForm => driver.FindElement(By.CssSelector("[name='checkout']"), 2);
        public IList<IWebElement> ErrorMessagesElements => driver.FindElements(By.CssSelector("ul.woocommerce-error li"), 2);
        public IWebElement ErrorsList => driver.FindElement(By.CssSelector("ul.woocommerce-error"), 2);
        public IWebElement PlaceOrderButton => driver.FindElement(By.CssSelector("button#place_order"), 2);
        public IWebElement ProductTotalElement => driver.FindElement(By.CssSelector(".product-total bdi"), 5);
        public IList<IWebElement> ProductTotalElements => driver.FindElements(By.CssSelector(".product-total bdi"), 2);
        public IWebElement CartSubtotalElement => driver.FindElement(By.CssSelector(".cart-subtotal bdi"), 2);
        public IWebElement OrderTotalElement => driver.FindElement(By.CssSelector(".order-total td"), 2);

        private IWebElement BillingFirstNameInput => driver.FindElement(By.CssSelector("input#billing_first_name"), 2);
        private IWebElement BillingLastNameInput => driver.FindElement(By.CssSelector("input#billing_last_name"), 2);
        private IWebElement BillingAddressInput => driver.FindElement(By.CssSelector("input#billing_address_1"), 2);
        private IWebElement BillingPostcodeInput => driver.FindElement(By.CssSelector("input#billing_postcode"), 2);
        private IWebElement BillingCityInput => driver.FindElement(By.CssSelector("input#billing_city"), 2);
        private IWebElement BillingPhoneInput => driver.FindElement(By.CssSelector("input#billing_phone"), 2);
        private IWebElement BillingEmailInput => driver.FindElement(By.CssSelector("input#billing_email"), 2);
        private IWebElement TermsCheckbox => driver.FindElement(By.CssSelector("input#terms"), 2);


        public CheckoutPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) {}

        public CheckoutPage FillInCardData(string cardNumber, string cardExpirationDate, string cardCvc)
        {
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
            return this;
        }

        //public CheckoutPage PlaceOrder()
        //{
        //    PlaceOrderButton.Click();
        //    WaitForElementsDisappear();
        //    return this;
        //}

        public T PlaceOrder<T>()
        {
            PlaceOrderButton.Click();
            WaitForLoadersDisappear(10);
            return (T)Activator.CreateInstance(typeof(T), driver, baseUrl);
        }

        public CheckoutPage FillInBillingData(string name, string lastName, string street, string postalCode, string city, string phone, string email)
        {
            BillingFirstNameInput.SendKeys(name);
            BillingLastNameInput.SendKeys(lastName);
            BillingAddressInput.SendKeys(street);
            BillingPostcodeInput.SendKeys(postalCode);
            BillingCityInput.SendKeys(city);
            BillingPhoneInput.SendKeys(phone);
            BillingEmailInput.SendKeys(email);
            return this;
        }

        public CheckoutPage CheckTerms()
        {
            TermsCheckbox.Click();
            return this;
        }
    }
}