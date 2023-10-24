using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DutchServisMCV;
using DutchServisMCV.Controllers;
using DutchServisMCV.Models;

namespace DutchServisMCV.Tests.Controllers
{
    namespace HomeControllerTests
    {
        [TestClass]
        public class Index
        {
            [TestMethod]
            public void Default()
            {
                // Arrange
                HomeController controller = new HomeController();
                // Assert
                ViewResult result = controller.Index() as ViewResult;
                Assert.IsNotNull(result);
            }
        }

        [TestClass]
        public class Login
        {
            [TestMethod]
            public void Default()
            {
                // Arrange
                HomeController controller = new HomeController();
                // Assert
                ViewResult result = controller.Login() as ViewResult;
                Assert.IsNotNull(result);
            }

            [TestMethod]
            public void CorrectLogin()
            {
                // Arrange
                HomeController controller = new HomeController();
                ViewResult indexResult = controller.Index() as ViewResult;
                Users user = new Users();
                user.Username = "admin";
                user.Pass = "123456";
                // Assert
                ViewResult result = controller.Login(user) as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreEqual(result, indexResult);
            }

            [TestMethod]
            public void IncorrectPassword()
            {
                // Arrange
                HomeController controller = new HomeController();
                ViewResult indexResult = controller.Index() as ViewResult;
                Users user = new Users();
                user.Username = "admin";
                user.Pass = "12d45f";
                // Assert
                ViewResult result = controller.Login(user) as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreNotEqual(result, indexResult);
            }

            [TestMethod]
            public void EmptyUsername()
            {
                // Arrange
                HomeController controller = new HomeController();
                ViewResult indexResult = controller.Index() as ViewResult;
                Users user = new Users();
                user.Username = "";
                user.Pass = "123456";
                // Assert
                ViewResult result = controller.Login(user) as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreNotEqual(result, indexResult);
            }
        }

        [TestClass]
        public class Logout 
        {
            [TestMethod]
            public void Default()
            {
                // Arrange
                HomeController controller = new HomeController();
                ViewResult indexResult = controller.Index() as ViewResult;

                // Test 1
                ViewResult result = controller.Logout() as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreEqual(result, indexResult);
            }
        }
    }
}
