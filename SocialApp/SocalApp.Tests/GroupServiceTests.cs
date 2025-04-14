namespace SocialApp.Tests
{
    using System.Collections.Generic;
    using SocialApp;
    using NSubstitute;
    using SocialApp.Repository;
    using SocialApp.Services;
    using SocialApp.Entities;
   

    public class GroupServiceTests
    {
        private readonly IGroupRepository groupRepo;
        private readonly IUserRepository userRepo;
        private readonly GroupService service;

        public GroupServiceTests()
        {
            this.groupRepo = Substitute.For<IGroupRepository>();
            this.userRepo = Substitute.For<IUserRepository>();
            this.service = new GroupService(this.groupRepo, this.userRepo);
        }

        private Group CreateTestGroup(long id = 1, long adminId = 1)
        {
            return new Group
            {
                Id = id,
                Name = $"Group {id}",
                Image = $"image{id}.jpg",
                Description = $"Description {id}",
                AdminId = adminId,
            };
        }

        private User CreateTestUser(long id = 1)
        {
            return new User
            {
                Id = id,
                Username = $"user{id}",
                Email = $"user{id}@example.com",
                PasswordHash = $"hash{id}",
                Image = $"avatar{id}.jpg"
            };
        }

        [Test]
        public void ValidateUpdate_WithValidData_UpdatesGroup()
        {
            // Arrange
            var groupId = 1L;
            var adminId = 1L;
            this.groupRepo.GetById(groupId).Returns(CreateTestGroup());
            this.userRepo.GetById(adminId).Returns(CreateTestUser());

            // Act
            this.service.ValidateUpdate(groupId, "New Name", "New Desc", "new.jpg", adminId);

            // Assert
            this.groupRepo.Received(1).UpdateById(groupId, "New Name", "new.jpg", "New Desc", adminId);
        }

        [Test]
        public void ValidateUpdate_WithNonexistentGroup_ThrowsException()
        {
            // Arrange
            this.groupRepo.GetById(Arg.Any<long>()).Returns((Group)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.ValidateUpdate(1, "Name", "Desc", "img.jpg", 1));
            Assert.That(ex.Message, Is.EqualTo("Group does not exist"));
        }

        [Test]
        public void ValidateUpdate_WithNonexistentAdmin_ThrowsException()
        {
            // Arrange
            this.groupRepo.GetById(Arg.Any<long>()).Returns(this.CreateTestGroup());
            this.userRepo.GetById(Arg.Any<long>()).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.ValidateUpdate(1, "Name", "Desc", "img.jpg", 1));
            Assert.That(ex.Message, Is.EqualTo("User does not exist"));
        }

        [Test]
        public void ValidateUpdate_WithEmptyName_ThrowsException()
        {
            // Arrange
            this.groupRepo.GetById(Arg.Any<long>()).Returns(CreateTestGroup());
            this.userRepo.GetById(Arg.Any<long>()).Returns(CreateTestUser());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.ValidateUpdate(1, "", "Desc", "img.jpg", 1));
            Assert.That(ex.Message, Is.EqualTo("Group name cannot be empty"));
        }

        [Test]
        public void GetAll_ReturnsAllGroups()
        {
            // Arrange
            var expectedGroups = new List<Group>
            {
                this.CreateTestGroup(1),
                this.CreateTestGroup(2),
            };
            this.groupRepo.GetAll().Returns(expectedGroups);

            // Act
            var result = this.service.GetAll();

            // Assert
            Assert.That(result, Is.EqualTo(expectedGroups));
        }

        [Test]
        public void GetById_ReturnsCorrectGroup()
        {
            // Arrange
            var expectedGroup = this.CreateTestGroup();
            this.groupRepo.GetById(1).Returns(expectedGroup);

            // Act
            var result = this.service.GetById(1);

            // Assert
            Assert.That(result, Is.EqualTo(expectedGroup));
        }

        [Test]
        public void GetUsersFromGroup_ReturnsGroupMembers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                this.CreateTestUser(1),
                this.CreateTestUser(2),
            };
            this.groupRepo.GetUsersFromGroup(1).Returns(expectedUsers);

            // Act
            var result = this.service.GetUsersFromGroup(1);

            // Assert
            Assert.That(result, Is.EqualTo(expectedUsers));
        }

        [Test]
        public void GetGroupsForUser_ReturnsUserGroups()
        {
            // Arrange
            var expectedGroups = new List<Group>
            {
                this.CreateTestGroup(1),
                this.CreateTestGroup(2),
            };
            this.groupRepo.GetGroupsForUser(1).Returns(expectedGroups);

            // Act
            var result = this.service.GetGroupsForUser(1);

            // Assert
            Assert.That(result, Is.EqualTo(expectedGroups));
        }
    }
}