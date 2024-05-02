using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MusicApp.Authentication;
using MusicApp.Database;
using System;

namespace MusicApp.Tests.Authentication
{
    [TestClass]
    public class AuthenticationModuleTests
    {
        private Mock<IDatabaseManager> _mockDatabaseManager;

        [TestInitialize]
        public void Setup()
        {
            _mockDatabaseManager = new Mock<IDatabaseManager>();
            _mockDatabaseManager.Setup(m => m.GetCredentials(It.IsAny<string>())).Returns(("CorrectHashedPassword", "CorrectSalt"));
        }

        [TestMethod]
        public void RegisterUser_WithValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            string username = "testUser";
            string password = "testPassword123";
            string salt = AuthenticationModule.GenerateSalt();  // Consider mocking this if deterministic salt is needed
            string hashedPassword = AuthenticationModule.HashPassword(password, salt);

            _mockDatabaseManager.Setup(m => m.RegisterUser(username, hashedPassword, salt)).Returns(true);

            // Act
            bool result = AuthenticationModule.RegisterUser(username, password);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AuthenticateUser_WithCorrectCredentials_ShouldReturnTrue()
        {
            // Arrange
            string username = "existingUser";
            string password = "userPassword123";
            string salt = "VGhpcyBpcyBhIHNhbHQgdGV4dCBtZXNzYWdl";
            string hashedPassword = AuthenticationModule.HashPassword(password, salt);

            _mockDatabaseManager.Setup(m => m.GetCredentials(username)).Returns((hashedPassword, salt));

            // Act
            bool result = AuthenticationModule.AuthenticateUser(username, password);

            // Assert
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void AuthenticateUser_WithIncorrectCredentials_ShouldReturnFalse()
        {
            // Arrange
            string username = "existingUser";
            string password = "userPassword123";
            string wrongPassword = "wrongPassword";
            string salt = "randomSalt";
            string hashedPassword = AuthenticationModule.HashPassword(password, salt);

            _mockDatabaseManager.Setup(m => m.GetCredentials(username)).Returns((hashedPassword, salt));

            // Act
            bool result = AuthenticationModule.AuthenticateUser(username, wrongPassword);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
