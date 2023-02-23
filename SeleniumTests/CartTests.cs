using FakestorePageObjects;
using Helpers;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace SeleniumTests
{
    public class CartTests : BaseTest
    {
        public IList<string> ProductsIDs => new List<string>() { testData.Products[0].Id, testData.Products[1].Id };
        IList<string> ProductsURLs => new List<string>() {
            testData.Products[0].Url,
            testData.Products[1].Url,
        };

        [Test]
        public void ProductAddedToCartTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage.GoTo(ProductsURLs[0]).AddToCart().GoToCart();

            Assert.Multiple(() =>
            {
                Assert.That(cartPage.CartItems.Count, Is.EqualTo(1), "Number of product in cart is not 1");
                Assert.That(cartPage.ItemId, Is.EqualTo(ProductsIDs[0]),
                    "Product's in cart id is not " + ProductsIDs[0]);
            });
        }

        [Test]
        public void TwoItemsOfProductAddedToCartTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage.GoTo(ProductsURLs[0]).AddToCart(2).GoToCart();

            Assert.Multiple(() =>
            {
                Assert.That(cartPage.CartItems.Count, Is.EqualTo(1), "Number of product in cart is not 1");
                Assert.That(cartPage.ItemId, Is.EqualTo(ProductsIDs[0]), "Product's in cart id is not " + ProductsIDs[0]);
                Assert.That(cartPage.QuantityField.GetAttribute("value"), Is.EqualTo("2"), "Number of items of the product is not 2");
            });
        }

        [Test]
        public void TwoProductsAddedToCartTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage.GoTo(ProductsURLs[0]).AddToCart().GoTo(ProductsURLs[1]).AddToCart().GoToCart();

            Assert.Multiple(() =>
            {
                Assert.That(cartPage.CartItems.Count, Is.EqualTo(2), "Number of product in cart is not 1");
                Assert.That(cartPage.ItemIds[0], Is.EqualTo(ProductsIDs[0]),
                    "Product's in cart id is not " + ProductsIDs[0]);
                Assert.That(cartPage.ItemIds[1], Is.EqualTo(ProductsIDs[1]),
                    "Product's in cart id is not " + ProductsIDs[1]);
            });
        }
        [Test]
        public void CartEmptyAtStartTest()
        {
            CartPage cartPage = new CartPage(driver, config.BaseUrl);
            cartPage.GoTo();
            Assert.DoesNotThrow(() => _ = cartPage.CartEmptyMessage, "There is no \"Empty Cart\" message");
        }
        [Test]
        public void CantAddZeroItemsTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            productPage.GoTo(ProductsURLs[0]).AddToCart(0);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(productPage.IsQuantityFieldRangeUnderflowPresent(), "Test was probably able to add 0 items to cart. Range Underflow validation didn't return \"true\".");
                CustomAssert.ThrowsWebDriverTimeoutException(() => _ = productPage.GoToCartButton,
                    "\"Go to cart\" link was found, but it shouldn't. Nothing should be added to cart when you try add 0 items.");
                CustomAssert.ThrowsWebDriverTimeoutException(() => _ = productPage.GoToCartButton,
                    "\"Go to cart\" link was found, but it shouldn't. Nothing should be added to cart when you try add 0 items.");
            });
        }
        [Test]
        public void CanRemoveProductFromCartTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage
                .GoTo(ProductsURLs[0])
                .AddToCart()
                .GoToCart()
                .RemoveItem(ProductsIDs[0]);

            Assert.DoesNotThrow(() => _ = cartPage.CartEmptyMessage, "There is no \"Empty Cart\" message. Product was not removed from cart.");
        }
        [Test]
        public void CanIncreaseNumberOfItemsTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage.GoTo(ProductsURLs[0]).AddToCart().GoToCart().ChangeItemQuantity(5).UpdateCart();

            Assert.That(cartPage.QuantityField.GetAttribute("value"), Is.EqualTo("5"), "Number of items didn't change");
        }
        [Test]
        public void ChangingNumberOfItemsToZeroRemovesProductTest()
        {

            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            CartPage cartPage = productPage.GoTo(ProductsURLs[0]).AddToCart().GoToCart().ChangeItemQuantity(0).UpdateCart();

            Assert.DoesNotThrow(() => _ = cartPage.CartEmptyMessage, "There is no \"Empty Cart\" message. Product was not removed from cart.");
        }
        [Test]
        public void CantChangeToMoreThanStockTest()
        {
            ProductPage productPage = new ProductPage(driver, config.BaseUrl);
            int stockNumber = productPage.GoTo(ProductsURLs[0]).NumberOfProductsInStock;
            CartPage cartPage = productPage.AddToCart().GoToCart().ChangeItemQuantity(stockNumber + 1).UpdateCart();

            Assert.IsTrue(cartPage.IsQuantityFieldRangeOverflowPresent(), "Test was probably able to add more items than available in stock. Range Overflow validation didn't return \"true\".");
        }
    }
}