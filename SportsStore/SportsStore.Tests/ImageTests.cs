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
    public class ImageTests
    {
        [TestMethod]
        public void CanRetrieveImageData()
        {
            var product = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            var mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                product,
                new Product { ProductID = 3, Name = "P3"}
            }.AsQueryable());
            var target = new ProductController(mock.Object);

            ActionResult result = target.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(product.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void CannotRetrieveImageDataForInvalidID()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"}
            }.AsQueryable());
            var target = new ProductController(mock.Object);

            ActionResult result = target.GetImage(100);

            Assert.IsNull(result);
        }
    }
}
