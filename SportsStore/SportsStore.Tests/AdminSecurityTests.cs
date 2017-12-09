using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
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
    public class AdminSecurityTests
    {
        [TestMethod]
        public void CanLoginWithValidCredentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "sekret")).Returns(true);

            LoginViewModel model = new LoginViewModel
            {
                Username = "admin",
                Password = "sekret"
            };

            var target = new AccountController(mock.Object);

            ActionResult result = target.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void CannotLoginWithInvalidCredentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("invalidUser", "invalidPassword")).Returns(false);

            LoginViewModel model = new LoginViewModel
            {
                Username = "invalidUser",
                Password = "invalidPassword"
            };

            var target = new AccountController(mock.Object);

            ActionResult result = target.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
