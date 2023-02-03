using FakestorePageObjects;
using Helpers;
using NUnit.Framework;
using System.Globalization;

namespace SeleniumTests
{
    public class CheckoutTests : BaseTest
    {
        IList<string> ProductsURLs => new List<string>() {
            testData.Products[0].Url,
            testData.Products[1].Url
        };
        IList<string> ProductsPricesText => new List<string>() {
            testData.Products[0].PriceText,
            testData.Products[1].PriceText,
        };

        IList<int> ProductsPrices => new List<int>() {
            testData.Products[0].Price,
            testData.Products[1].Price
        };

        string CardNumber => testData.Card.Number;
        string CardExpirationDate => testData.Card.ExpirationDate;
        string CardCvc => testData.Card.Cvc;

        [Test]
        public void FieldsValidationTests()
        {
            TestData.ValidationMessages messages = testData.ValidationMessages;

            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CheckoutPage checkoutPage = productPage.GoTo(ProductsURLs[0])
                .AddToCart()
                .GoToCart()
                .GoToCheckout()
                .FillInCardData(CardNumber, CardExpirationDate, CardCvc)
                .PlaceOrder<CheckoutPage>();

            IList<string> errorMessages = checkoutPage.ErrorMessagesElements.Select(element => element.Text).ToList();
            IList<string> expectedErrorMessages = new List<string> {
                messages.Name,
                messages.LastName,
                messages.Address,
                messages.Postcode,
                messages.City,
                messages.Phone,
                messages.Email,
                messages.Terms
            };

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => _ = checkoutPage.ErrorsList, "Error list was not found. There was no validation error.");
                Assert.AreEqual(8, checkoutPage.ErrorMessagesElements.Count, "Number of error messages is not correct.");
                Assert.AreEqual(expectedErrorMessages.OrderBy(message => message), errorMessages.OrderBy(message => message));
            });
        }

        [Test]
        public void ReviewOrderOneProductTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CheckoutPage checkoutPage = productPage.GoTo(ProductsURLs[0])
                .AddToCart()
                .GoToCart()
                .GoToCheckout();

            float tax = CalculateTax(ProductsPrices[0]);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(ProductsPricesText[0], checkoutPage.ProductTotalElement.Text);
                Assert.AreEqual(ProductsPricesText[0], checkoutPage.CartSubtotalElement.Text);
                Assert.AreEqual(ProductsPricesText[0] + " (zawiera " + FormatNumber(tax) + " VAT)", checkoutPage.OrderTotalElement.Text);
            });
        }

        [Test]
        public void ReviewOrderMultipleProductsTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CheckoutPage checkoutPage = productPage.GoTo(ProductsURLs[0])
                .AddToCart(2)
                .GoTo(ProductsURLs[1])
                .AddToCart(3)
                .GoToCart()
                .GoToCheckout();

            float totalPrice = ProductsPrices[0] * 2 + ProductsPrices[1] * 3;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(FormatNumber(ProductsPrices[0] * 2), checkoutPage.ProductTotalElements[0].Text);
                Assert.AreEqual(FormatNumber(ProductsPrices[1] * 3), checkoutPage.ProductTotalElements[1].Text);
                Assert.AreEqual(FormatNumber(totalPrice), checkoutPage.CartSubtotalElement.Text);
                Assert.AreEqual(FormatNumber(totalPrice) + " (zawiera " + FormatNumber(CalculateTax(totalPrice)) + " VAT)", checkoutPage.OrderTotalElement.Text);
            });
        }

        [Test]
        public void ChangingNumberOfItemsTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CheckoutPage checkoutPage = productPage.GoTo(ProductsURLs[0])
                .AddToCart()
                .GoToCart()
                .ChangeItemQuantity(2)
                .UpdateCart()
                .GoToCheckout();

            //Chrome issue with clicking on the button
            if (config.Browser == "chrome")
            {
                TestHelper.DoOnTimeout(
                    () => _ = checkoutPage.CheckoutForm,
                    () => _ = new CartPage(driver, config.BaseUrl).GoToCheckout().CheckoutForm
                 );
            }
            else _ = checkoutPage.CheckoutForm;

            float total = ProductsPrices[0] * 2;
            float tax = CalculateTax(total);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(FormatNumber(total), checkoutPage.ProductTotalElement.Text);
                Assert.AreEqual(FormatNumber(total), checkoutPage.CartSubtotalElement.Text);
                Assert.AreEqual(FormatNumber(total) + " (zawiera " + FormatNumber(tax) + " VAT)", checkoutPage.OrderTotalElement.Text);
            });
        }

        [Test]
        public void SuccesfullPaymentTest()
        {
            TestData.Customer customer = testData.Customer;

            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            SuccessfulOrderPage successfulOrderPage = productPage.GoTo(ProductsURLs[0])
                .AddToCart()
                .GoToCart()
                .GoToCheckout()
                .FillInBillingData(customer.Name, customer.LastName, customer.Address, customer.Postcode, customer.City, customer.Phone, customer.Email)
                //.FillInBillingData("Karolina", "Kowalska", "ul. Kwiatowa 12/44", "31-333", "Kraków", "6666666", "hoho@hohoho.pl")
                .FillInCardData(CardNumber, CardExpirationDate, CardCvc)
                .CheckTerms()
                .PlaceOrder<SuccessfulOrderPage>();

            Assert.AreEqual("Zamówienie otrzymane", successfulOrderPage.EntryHeader.Text, "Page header is not what expected. Order was not sucessful.");
        }


        private float CalculateTax(float total)
        {
            return (float)Math.Round(total - (total / 1.23), 2);
        }

        private string FormatNumber(float number)
        {
            return string.Format(CultureInfo.GetCultureInfo("pl-PL"), "{0:### ###.00}", number) + " zł";
        }
    }
}
