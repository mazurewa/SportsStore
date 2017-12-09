using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SportsStore.Tests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void CanAddNewLines()
        {
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };

            Cart objectUnderTest = new Cart();

            objectUnderTest.AddItem(p1, 1);
            objectUnderTest.AddItem(p2, 1);

            CartLine[] results = objectUnderTest.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void CanAddQuantityForExistingLines()
        {
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };

            Cart objectUnderTest = new Cart();

            objectUnderTest.AddItem(p1, 1);
            objectUnderTest.AddItem(p2, 1);
            objectUnderTest.AddItem(p1, 10);

            CartLine[] results = objectUnderTest.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void CanRemoveLine()
        {
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };
            var p3 = new Product { ProductID = 3, Name = "P3" };

            Cart objectUnderTest = new Cart();

            objectUnderTest.AddItem(p1, 1);
            objectUnderTest.AddItem(p2, 1);
            objectUnderTest.AddItem(p3, 5);
            objectUnderTest.AddItem(p2, 1);

            objectUnderTest.RemoveLine(p2);


            Assert.AreEqual(objectUnderTest.Lines.Where(x=>x.Product == p2).Count(), 0);
            Assert.AreEqual(objectUnderTest.Lines.Count(), 2);
        }

        [TestMethod]
        public void CanCalculateTotal()
        {
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart objectUnderTest = new Cart();

            objectUnderTest.AddItem(p1, 1);
            objectUnderTest.AddItem(p2, 1);
            objectUnderTest.AddItem(p1, 3);

            decimal result = objectUnderTest.ComputeTotalValue();

            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void CanClearContents()
        {
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart objectUnderTest = new Cart();

            objectUnderTest.AddItem(p1, 1);
            objectUnderTest.AddItem(p2, 1);

            objectUnderTest.Clear();

            Assert.AreEqual(objectUnderTest.Lines.Count(), 0);
        }

        [TestMethod]
        public void CanAddToCart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jablka"}
            }.AsQueryable());

            Cart cart = new Cart();
            CartController objectUnderTest = new CartController(mock.Object, null);
            objectUnderTest.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void RedirectToCartScreenAfterAdding()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jablka"}
            }.AsQueryable());

            Cart cart = new Cart();
            CartController objectUnderTest = new CartController(mock.Object, null);

            RedirectToRouteResult result = objectUnderTest.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void CanViewCartContents()
        {
            Cart cart = new Cart();
            CartController objectUnderTest = new CartController(null, null);

            CartIndexViewModel result = (CartIndexViewModel) objectUnderTest.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void CannotCheckoutEmptyCart()
        {
            var mock = new Mock<IOrderProcessor>();
            var cart = new Cart();
            var shippingDetails = new ShippingDetails();

            CartController target = new CartController(null, mock.Object);

            ViewResult result = target.Checkout(cart, shippingDetails);

            mock.Verify(x => x.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()));
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void CannotCheckoutInvalidShippingDetailst()
        {
            var mock = new Mock<IOrderProcessor>();
            var cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);
            target.ModelState.AddModelError("error", "error");

            ViewResult result = target.Checkout(cart, new ShippingDetails());

            mock.Verify(x => x.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void CanCheckoutAndSubmitOrder()
        {
            var mock = new Mock<IOrderProcessor>();
            var cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            mock.Verify(x => x.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
