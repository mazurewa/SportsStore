using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void CanGeneratePageLinks()
        {
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Strona " + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona 1"">1</a>" +
                @"<a class=""btn btn-default btn-primary selected"" href=""Strona 2"">2</a>" +
                @"<a class=""btn btn-default"" href=""Strona 3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void CanSendPaginationViewModel()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;
            PagingInfo pagingInfo = result.PagingInfo;

            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CanFilterProducts()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            Product[] result = ((ProductListViewModel)controller.List(null, 2).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P4");
            Assert.IsTrue(result[1].Name == "P5");
        }

        [TestMethod]
        public void CanCreateCategories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category="Sat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Sat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Pat3"}
                });

            NavController controller = new NavController(mock.Object);

            string[] result = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.IsTrue(result[0] == "Cat1");
            Assert.IsTrue(result[1] == "Pat3");
            Assert.IsTrue(result[2] == "Sat2");
        }

        [TestMethod]
        public void IndicatesSelectedCategory()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category="Sat2"}
                });

            NavController controller = new NavController(mock.Object);
            var categoryToSelect = "Sat2";

            string result = controller.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(result, categoryToSelect);
        }

        [TestMethod]
        public void CanGenerateCategorySpecificProductsCount()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            int result1 = ((ProductListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int result2 = ((ProductListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int result3 = ((ProductListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resultAll = ((ProductListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(result1, 2);
            Assert.AreEqual(result2, 2);
            Assert.AreEqual(result3, 1);
            Assert.AreEqual(resultAll, 5);
        }

    }
}
