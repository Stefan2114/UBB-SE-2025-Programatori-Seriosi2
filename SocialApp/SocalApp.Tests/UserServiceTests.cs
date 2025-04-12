namespace SocalApp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NSubstitute;
    using SocalApp;
    using SocialApp.Entities;
    using SocialApp.Enums;
    using SocialApp.Repository;
    using SocialApp.Services;

    /// <summary>
    /// Contains unit tests for the UserService class.
    /// </summary>
    public class UserServiceTests
    {
        /// <summary>
        /// Validates that the AddUser method successfully creates a user when provided with valid arguments.
        /// </summary>
        [Test]
        public void ValidateAddUser_WithValidArguments_CreatesUser()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            string username = "testuser";
            string email = "testuser@gmail.com";
            string password = "password123";
            string image = "testimage.png";

            // Act
            userService.AddUser(username, email, password, image);

            // Assert
            userRepository.Received(1).Save(Arg.Is<User>(u =>
                u.Username == username &&
                u.Email == email &&
                u.PasswordHash == password &&
                u.Image == image));
        }

        /// <summary>
        /// Validates that the AddUser method throws an exception when provided with an empty username.
        /// </summary>
        [Test]
        public void ValidateAddUser_WithEmptyUsername_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                userService.AddUser(string.Empty, "valid@email.com", "password", "image"));
            Assert.That(ex.Message, Is.EqualTo("Username cannot be empty"));

            userRepository.DidNotReceive().Save(Arg.Any<User>());
        }

        /// <summary>
        /// Validates that the AddUser method throws an exception when provided with an empty email.
        /// </summary>
        [Test]
        public void ValidateAddUser_WithEmptyEmail_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                userService.AddUser("validuser", string.Empty, "password", "image"));
            Assert.That(ex.Message, Is.EqualTo("Email cannot be empty"));

            userRepository.DidNotReceive().Save(Arg.Any<User>());
        }

        /// <summary>
        /// Validates that the AddUser method throws an exception when provided with an empty password.
        /// </summary>
        [Test]
        public void ValidateAddUser_WithEmptyPassword_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                userService.AddUser("validuser", "valid@email.com", string.Empty, "image"));
            Assert.That(ex.Message, Is.EqualTo("Password cannot be empty"));

            userRepository.DidNotReceive().Save(Arg.Any<User>());
        }

        /// <summary>
        /// Validates that the DeleteUser method successfully deletes a user when provided with a valid ID.
        /// </summary>
        [Test]
        public void ValidateDeleteUser_WithValidId_DeletesUser()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            long userId = 1;

            User user = new User { Id = userId, Email = "asdsad@gmail.com", Image = "lalal", PasswordHash = "asdasd", Username = "George" };
            
            userRepository.GetById(userId).Returns(user);

            // Act
            userService.DeleteUser(userId);

            // Assert
            userRepository.Received(1).DeleteById(userId);
        }

        /// <summary>
        /// Validates that the DeleteUser method throws an exception when provided with an invalid user ID.
        /// </summary>
        [Test]
        public void ValidateDeleteUser_WithInvalidId_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            long userId = 1;

            // Act
            userRepository.GetById(userId).Returns((User)null);

            // Assert
            var ex = Assert.Throws<Exception>(() => userService.DeleteUser(userId));
            userRepository.Received(1).GetById(userId);
        }

        /// <summary>
        /// Validates that the UpdateUser method successfully updates a user when provided with a valid user ID.
        /// </summary>
        [Test]
        public void ValidateUpdateUser_WithValidId()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            string username = "testuser";
            string email = "testuser@gmail.com";
            string password = "password123";
            string image = "testimage.png";

            long userId = 1;

            User user = new User
            {
                Id = userId,
                Username = username,
                Email = email,
                PasswordHash = password,
                Image = image
            };

            userRepository.GetById(userId).Returns(user);

            // Act
            userService.UpdateUser(userId, username, email, password, image);

            // Assert
            userRepository.Received(1).GetById(userId);
            userRepository.Received(1).UpdateById(userId, username, email, password, image);
        }

        /// <summary>
        /// Validates that the UpdateUser method throws an exception when provided with an invalid user ID.
        /// </summary>
        [Test]
        public void ValidateUpdateUser_WithInvalidID_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            long userId = 1;
            string username = "testuser";
            string email = "testuser@gmail.com";
            string password = "password123";
            string image = "testimage.png";

            userRepository.GetById(userId).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                userService.UpdateUser(userId, username, email, password, image));
            Assert.That(ex.Message, Is.EqualTo("User does not exist"));
            userRepository.Received(1).GetById(userId);
        }
    }
}
