// <copyright file="UserServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SocalApp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NSubstitute;
    using SocialApp.Entities;
    using SocialApp.Pages;
    using SocialApp.Repository;
    using SocialApp.Services;

    /// <summary>
    /// Tests for the UserService class.
    /// </summary>
    internal class UserServiceTests
    {
        [Test]
        public void TestGetAll()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var fakeUsers = new List<User>  // Mock data
                {
                    new User { Id = 2, Username = "follower1", Email = "follower1@example.com", PasswordHash = "hashedpassword1", Image = "default1.png" },
                    new User { Id = 3, Username = "follower2", Email = "follower2@example.com", PasswordHash = "hashedpassword2", Image = "default2.png" },
                };

            // Tell the mock to return fakeUsers when GetAll() is called
            userRepository.GetAll().Returns(fakeUsers);

            var userService = new UserService(userRepository);

            // Act
            var result = userService.GetAllUsers();

            // Assert
            Assert.IsNotNull(result);  // Now it won't be null
            Assert.IsInstanceOf<List<User>>(result);  // Correct type
            Assert.AreEqual(2, result.Count);  // Verify data
        }

        [Test]
        public void TestGetById()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var userId = 1;
            var expectedUser = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword",
                Image = "default.png",
            };
            userRepository.GetById(userId).Returns(expectedUser);

            // Act
            var result = userService.GetById(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedUser));
        }

        [Test]
        public void TestGetByIdNonExistentUser()
        {
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            int nonExistentUserId = 999;

            userRepository.GetById(nonExistentUserId).Returns((User)null);

            var result = userService.GetById(nonExistentUserId);

            Assert.IsNull(result);
        }

        [Test]
        public void TestGetUserFollowers()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var userId = 1;
            var expectedFollowers = new List<User>
                {
                    new User { Id = 2, Username = "follower1", Email = "follower1@example.com", PasswordHash = "hashedpassword1", Image = "default1.png" },
                    new User { Id = 3, Username = "follower2", Email = "follower2@example.com", PasswordHash = "hashedpassword2", Image = "default2.png" },
                };
            userRepository.GetUserFollowers(userId).Returns(expectedFollowers);
            // Act
            var result = userService.GetUserFollowersFromId(userId);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expectedFollowers));
        }

        [Test]
        public void TestGetUserFollowers_InvalidUserId()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            int invalidUserId = -1;
            userRepository.GetUserFollowers(invalidUserId).Returns(new List<User>());

            // Act
            var result = userService.GetUserFollowersFromId(invalidUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            userRepository.Received(1).GetUserFollowers(invalidUserId);
        }

        [Test]
        public void GetUserFollowing_ValidId_ReturnsFollowingList()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            long validUserId = 1;
            var expectedFollowing = new List<User>
            {
                new User { Id = 2, Username = "follower1", Email = "follower1@example.com", PasswordHash = "hashedpassword1", Image = "default1.png" },
                new User { Id = 3, Username = "follower2", Email = "follower2@example.com", PasswordHash = "hashedpassword2", Image = "default2.png" },
            };

            userRepository.GetUserFollowing(validUserId).Returns(expectedFollowing);

            // Act
            var result = userService.GetUserFollowing(validUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Verify count
            Assert.AreEqual("follower1", result[0].Username); // Verify first user
            userRepository.Received(1).GetUserFollowing(validUserId); // Verify repo call
        }

        [Test]
        public void GetUserFollowing_InvalidId_ReturnsEmptyList()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);

            long invalidUserId = -1; // or 0, depending on your validation logic
            userRepository.GetUserFollowing(invalidUserId).Returns(new List<User>());

            // Act
            var result = userService.GetUserFollowing(invalidUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result); // Expect empty list
        }

        [Test]
        public void TestFollowUser_ValidUsers_Success()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var followerId = 1;
            var followedId = 2;

            userRepository.GetById(followerId).Returns(new User { Id = followerId, Username = "follower", Email = "follower@example.com", PasswordHash = "hash1", Image = "img1.png" });
            userRepository.GetById(followedId).Returns(new User { Id = followedId, Username = "followed", Email = "followed@example.com", PasswordHash = "hash2", Image = "img2.png" });

            // Act & Assert
            Assert.DoesNotThrow(() => userService.FollowUserById(followerId, followedId));
            userRepository.Received(1).Follow(followerId, followedId);
        }

        [Test]
        public void TestFollowUser_FollowerDoesNotExist_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var nonExistentFollowerId = 1;
            var followedId = 2;

            userRepository.GetById(nonExistentFollowerId).Returns((User)null);
            userRepository.GetById(followedId).Returns(new User { Id = followedId, Username = "followed", Email = "followed@example.com", PasswordHash = "hash2", Image = "img2.png" });

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => userService.FollowUserById(nonExistentFollowerId, followedId));
            Assert.That(ex.Message, Is.EqualTo("User does not exist"));
            userRepository.DidNotReceive().Follow(Arg.Any<long>(), Arg.Any<long>());
        }

        [Test]
        public void TestFollowUser_FollowedDoesNotExist_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var followerId = 1;
            var nonExistentFollowedId = 2;

            userRepository.GetById(followerId).Returns(new User { Id = followerId, Username = "follower", Email = "follower@example.com", PasswordHash = "hash1", Image = "img1.png" });
            userRepository.GetById(nonExistentFollowedId).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => userService.FollowUserById(followerId, nonExistentFollowedId));
            Assert.That(ex.Message, Is.EqualTo("User to follow does not exist"));
            userRepository.DidNotReceive().Follow(Arg.Any<long>(), Arg.Any<long>());
        }

        [Test]
        public void TestUnfollowUser_ValidUsers_Success()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var followerId = 1;
            var followedId = 2;

            userRepository.GetById(followerId).Returns(new User { Id = followerId, Username = "follower", Email = "follower@example.com", PasswordHash = "hash1", Image = "img1.png" });
            userRepository.GetById(followedId).Returns(new User { Id = followedId, Username = "followed", Email = "followed@example.com", PasswordHash = "hash2", Image = "img2.png" });

            // Act & Assert
            Assert.DoesNotThrow(() => userService.UnfollowUserById(followerId, followedId));
            userRepository.Received(1).Unfollow(followerId, followedId);
        }

        [Test]
        public void TestUnfollowUser_FollowerDoesNotExist_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var nonExistentFollowerId = 1;
            var followedId = 2;

            userRepository.GetById(nonExistentFollowerId).Returns((User)null);
            userRepository.GetById(followedId).Returns(new User { Id = followedId, Username = "followed", Email = "followed@example.com", PasswordHash = "hash2", Image = "img2.png" });

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => userService.UnfollowUserById(nonExistentFollowerId, followedId));
            Assert.That(ex.Message, Is.EqualTo("User does not exist"));
            userRepository.DidNotReceive().Unfollow(Arg.Any<long>(), Arg.Any<long>());
        }

        [Test]
        public void TestUnfollowUser_FollowedDoesNotExist_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var followerId = 1;
            var nonExistentFollowedId = 2;

            userRepository.GetById(followerId).Returns(new User { Id = followerId, Username = "follower", Email = "follower@example.com", PasswordHash = "hash1", Image = "img1.png" });
            userRepository.GetById(nonExistentFollowedId).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => userService.UnfollowUserById(followerId, nonExistentFollowedId));
            Assert.That(ex.Message, Is.EqualTo("User to unfollow does not exist"));
            userRepository.DidNotReceive().Unfollow(Arg.Any<long>(), Arg.Any<long>());
        }

        [Test]
        public void TestSearchUsers_ValidQuery_ReturnsMatchingFollowingUsers()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var userId = 1;
            var searchQuery = "test";
            var followingUsers = new List<User>
            {
                new User { Id = 2, Username = "testuser1", Email = "test1@example.com", PasswordHash = "hash1", Image = "img1.png" },
                new User { Id = 3, Username = "testuser2", Email = "test2@example.com", PasswordHash = "hash2", Image = "img2.png" },
                new User { Id = 4, Username = "otheruser", Email = "other@example.com", PasswordHash = "hash3", Image = "img3.png" }
            };

            userRepository.GetUserFollowing(userId).Returns(followingUsers);

            // Act
            var result = userService.SearchUsersById(userId, searchQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.That(result.Select(u => u.Username), Is.EquivalentTo(new[] { "testuser1", "testuser2" }));
        }

        [Test]
        public void TestSearchUsers_NoMatches_ReturnsEmptyList()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var userId = 1;
            var searchQuery = "nonexistent";
            var followingUsers = new List<User>
            {
                new User { Id = 2, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1", Image = "img1.png" },
                new User { Id = 3, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2", Image = "img2.png" }
            };

            userRepository.GetUserFollowing(userId).Returns(followingUsers);

            // Act
            var result = userService.SearchUsersById(userId, searchQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void TestSearchUsers_EmptyQuery_ReturnsAllFollowingUsers()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var userService = new UserService(userRepository);
            var userId = 1;
            var searchQuery = string.Empty;
            var followingUsers = new List<User>
            {
                new User { Id = 2, Username = "user1", Email = "user1@example.com", PasswordHash = "hash1", Image = "img1.png" },
                new User { Id = 3, Username = "user2", Email = "user2@example.com", PasswordHash = "hash2", Image = "img2.png" }
            };

            userRepository.GetUserFollowing(userId).Returns(followingUsers);

            // Act
            var result = userService.SearchUsersById(userId, searchQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Should return all following users
            Assert.That(result.Select(u => u.Username), Is.EquivalentTo(new[] { "user1", "user2" }));
        }
    }
}
