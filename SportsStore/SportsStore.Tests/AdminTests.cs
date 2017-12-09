using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SportsStore.Tests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void IndexContainsAllProducts()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1"},
                new Product {ProductID=2, Name="P2"},
                new Product {ProductID=3, Name="P3"}
            });

            var target = new AdminController(mock.Object);
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public void CanEditProduct()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1"},
                new Product {ProductID=2, Name="P2"},
                new Product {ProductID=3, Name="P3"}
            });

            var target = new AdminController(mock.Object);
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void CannotEditNonExistentProduct()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1"},
                new Product {ProductID=2, Name="P2"},
                new Product {ProductID=3, Name="P3"}
            });

            var target = new AdminController(mock.Object);
            Product p4 = (Product)target.Edit(4).ViewData.Model;

            Assert.IsNull(p4);
        }

        [TestMethod]
        public void CanSaveValidChanges()
        {
            var mock = new Mock<IProductRepository>();
            var target = new AdminController(mock.Object);
            var product = new Product { Name = "Test" };

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CannotSaveInvalidChanges()
        {
            var mock = new Mock<IProductRepository>();
            var target = new AdminController(mock.Object);
            var product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanDeleteValidProducts()
        {
            var product = new Product { ProductID = 2, Name = "Test" };
            var mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                product,
                new Product { ProductID = 3, Name = "P3"}
            });
            var target = new AdminController(mock.Object);

            ActionResult result = target.Delete(product.ProductID);

            mock.Verify(m => m.DeleteProduct(product.ProductID));
        }
    }
}
