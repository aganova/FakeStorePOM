using Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace FakestorePageObjects
{
    public class CartPage : BasePage
    {
        IJavaScriptExecutor js;
        private string CartUrl => baseUrl + "/koszyk";
        //public IList<IWebElement> CartItems2 => driver.FindElements(By.CssSelector("tr.cart_item"), 2);
        public IList<IWebElement> CartItems
        {
            get
            {
                _ = CartTable;
                return driver.FindElements(By.CssSelector("tr.cart_item"), 2);
            }
        }
        public string ItemId => CartItems[0].FindElement(By.CssSelector("a")).GetAttribute("data-product_id");
        public IList<string> ItemIds => CartItems.Select(item => item.FindElement(By.CssSelector("a")).GetAttribute("data-product_id")).ToList();
        private IWebElement CartTable => driver.FindElement(By.CssSelector("table.shop_table.cart"), 2);

        public IWebElement QuantityField
        {
            get
            {
                _ = CartTable;
                return driver.FindElement(By.CssSelector("input.qty"), 2);
            }
        }

        public IWebElement CartEmptyMessage => driver.FindElement(By.CssSelector(".cart-empty.woocommerce-info"), 3);
        private IWebElement UpdateCartButton => driver.FindElement(By.CssSelector("[name='update_cart']"), 2);

        IWebElement GoToCheckoutButton => driver.FindElement(By.CssSelector(".checkout-button"), 2);

        public CartPage(IWebDriver driver, string baseUrl) : base(driver, baseUrl) {}

        public CartPage GoTo()
        {
            driver.Navigate().GoToUrl(CartUrl);
            return this;
        }

        public CartPage RemoveItem(string productId)
        {
            driver.FindElement(By.CssSelector("a[data-product_id='" + productId + "']"), 2).Click();
            WaitForLoadersDisappear(5);
            return this;
        }

        public CartPage ChangeItemQuantity(int quantity)
        {
            QuantityField.Clear();
            QuantityField.SendKeys(quantity.ToString());
            return this;
        }

        public CartPage UpdateCart()
        {
            UpdateCartButton.Click();
            WaitForLoadersDisappear(5);
            return this;
        }

        public bool IsQuantityFieldRangeOverflowPresent()
        {
            js = (IJavaScriptExecutor)driver;
            return (bool)js.ExecuteScript("return arguments[0].validity.rangeOverflow", QuantityField);
        }

        public CheckoutPage GoToCheckout()
        {
            GoToCheckoutButton.Click();
            //WaitForLoadersDisappear(5);
            return new CheckoutPage(driver, baseUrl);
        }
    }
}