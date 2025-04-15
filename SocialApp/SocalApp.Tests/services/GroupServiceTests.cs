namespace SocialApp.Tests
{
    using System.Collections.Generic;
    using NSubstitute;
    using SocialApp.Repository;
    using SocialApp.Services;
    using SocialApp.Entities;


    public class GroupServiceTests
    {
        private readonly IGroupRepository groupRepository;
        private readonly IUserRepository userRepository;
        private readonly GroupService service;

        public GroupServiceTests()
        {
            this.groupRepository = Substitute.For<IGroupRepository>();
            this.userRepository = Substitute.For<IUserRepository>();
            this.service = new GroupService(this.groupRepository, this.userRepository);
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
        public void AddGroup_WithValidData_CreatesGroup()
        {
            // Arrange
            var adminId = 1L;
            var name = "New Group";
            var description = "A description";
            var image = "group.jpg";

            this.userRepository.GetById(adminId).Returns(this.CreateTestUser());

            // We don't need to test the result of SaveGroup directly here
            // since it's not returning a value – but we will check the returned Group object
            Group savedGroup = null;
            this.groupRepository.When(x => x.SaveGroup(Arg.Do<Group>(g => savedGroup = g)));

            // Act
            var result = this.service.AddGroup(name, description, image, adminId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(description, result.Description);
            Assert.AreEqual(image, result.Image);
            Assert.AreEqual(adminId, result.AdminId);
        }

        [Test]
        public void AddGroup_WithEmptyName_ThrowsException()
        {
            // Arrange
            var adminId = 1L;

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.AddGroup("", "description", "img.jpg", adminId));
            Assert.That(ex.Message, Is.EqualTo("Group name cannot be empty"));
        }

        [Test]
        public void AddGroup_WithNonexistentAdmin_ThrowsException()
        {
            // Arrange
            this.userRepository.GetById(Arg.Any<long>()).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.AddGroup("Name", "Description", "img.jpg", 999));
            Assert.That(ex.Message, Is.EqualTo("User does not exist"));
        }

        [Test]
        public void DeleteGroup_WithExistingGroup_DeletesGroup()
        {
            // Arrange
            var groupId = 1L;
            this.groupRepository.GetGroupById(groupId).Returns(this.CreateTestGroup());

            // Act
            this.service.DeleteGroup(groupId);

            // Assert
            this.groupRepository.Received(1).DeleteGroupById(groupId);
        }

        [Test]
        public void DeleteGroup_WithNonexistentGroup_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns((Group)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => this.service.DeleteGroup(123));
            Assert.That(ex.Message, Is.EqualTo("Group does not exist"));
        }

        [Test]
        public void UpdateUser_WithValidData_UpdatesGroup()
        {
            // Arrange
            var groupId = 1L;
            var adminId = 1L;
            this.groupRepository.GetGroupById(groupId).Returns(this.CreateTestGroup());
            this.userRepository.GetById(adminId).Returns(this.CreateTestUser());

            // Act
            this.service.UpdateGroup(groupId, "New Name", "New Description", "new.jpg", adminId);

            // Assert
            this.groupRepository.Received(1).UpdateGroup(groupId, "New Name", "new.jpg", "New Description", adminId);
        }

        [Test]
        public void UpdateUser_WithNonexistentGroup_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns((Group)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "Name", "Description", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("Group does not exist"));
        }

        [Test]
        public void UpdateUser_WithNonexistentAdmin_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns(this.CreateTestGroup());
            this.userRepository.GetById(Arg.Any<long>()).Returns((User)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "Name", "Description", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("User does not exist"));
        }

        [Test]
        public void UpdateUser_WithEmptyName_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns(this.CreateTestGroup());
            this.userRepository.GetById(Arg.Any<long>()).Returns(this.CreateTestUser());

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "", "Description", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("Group name cannot be empty"));
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
            this.groupRepository.GetAllGroups().Returns(expectedGroups);

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
            this.groupRepository.GetGroupById(1).Returns(expectedGroup);

            // Act
            var result = this.service.GetGroupById(1);

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
            this.groupRepository.GetUsersFromGroup(1).Returns(expectedUsers);

            // Act
            var result = this.service.GetUsersFromGroup(1);

            // Assert
            Assert.That(result, Is.EqualTo(expectedUsers));
        }


        [Test]
        public void UpdateGroup_WithValidData_UpdatesGroup()
        {
            // Arrange
            var groupId = 1L;
            var adminId = 1L;
            this.groupRepository.GetGroupById(groupId).Returns(this.CreateTestGroup());
            this.userRepository.GetById(adminId).Returns(this.CreateTestUser());

            // Act
            this.service.UpdateGroup(groupId, "New Name", "New Desc", "new.jpg", adminId);

            // Assert
            this.groupRepository.Received(1).UpdateGroup(groupId, "New Name", "new.jpg", "New Desc", adminId);
        }

        [Test]
        public void UpdateGroup_WithNonexistentGroup_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns((Group)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "Name", "Desc", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("Group does not exist"));
        }

        [Test]
        public void UpdateGroup_WithNonexistentAdmin_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns(this.CreateTestGroup());
            this.userRepository.GetById(Arg.Any<long>()).Returns((User)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "Name", "Desc", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("User does not exist"));
        }

        [Test]
        public void UpdateGroup_WithEmptyName_ThrowsException()
        {
            // Arrange
            this.groupRepository.GetGroupById(Arg.Any<long>()).Returns(this.CreateTestGroup());
            this.userRepository.GetById(Arg.Any<long>()).Returns(this.CreateTestUser());

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                this.service.UpdateGroup(1, "", "Desc", "img.jpg", 1));
            Assert.That(exception.Message, Is.EqualTo("Group name cannot be empty"));
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
            this.groupRepository.GetGroupsForUser(1).Returns(expectedGroups);

            // Act
            var result = this.service.GetGroups(1);

            // Assert
            Assert.That(result, Is.EqualTo(expectedGroups));
        }
    }
}