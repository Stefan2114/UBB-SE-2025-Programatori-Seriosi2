namespace SocialApp.Tests
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SocialApp.Entities;
    using SocialApp.Repository;
    using SocialApp.Services;

    [TestFixture]
    public class GroupServiceTests
    {
        private IGroupRepository groupRepo;
        private IUserRepository userRepo;
        private GroupService service;

        [SetUp]
        public void Setup()
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

        #region ValidateAdd Tests

        [Test]
        public void ValidateAdd_GroupNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            this.userRepo.GetById(1).Returns(CreateTestUser());

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                this.service.ValidateAdd("", "Valid desc", "img.jpg", 1));

            Assert.That(ex.Message, Does.Contain("Group name cannot be empty"));
            Assert.That(ex.ParamName, Is.EqualTo("groupName"));
        }

        [Test]
        public void ValidateAdd_GroupNameIsWhitespace_ThrowsArgumentException()
        {
            // Arrange
            this.userRepo.GetById(1).Returns(CreateTestUser());

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                this.service.ValidateAdd("   ", "Valid desc", "img.jpg", 1));

            Assert.That(ex.Message, Does.Contain("Group name cannot be empty"));
        }

        [Test]
        public void ValidateAdd_AdminUserDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            this.userRepo.GetById(1).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                this.service.ValidateAdd("Group", "Desc", "img.jpg", 1));

            Assert.That(ex.Message, Does.Contain("User does not exist"));
            Assert.That(ex.ParamName, Is.EqualTo("adminUserId"));
        }

        [Test]
        public void ValidateAdd_ValidInput_SavesGroup()
        {
            // Arrange
            var adminId = 1L;
            var groupName = "Group Name";
            var description = "A description";
            var image = "image.jpg";

            var expectedGroup = new Group
            {
                Name = groupName.Trim(),
                Description = description?.Trim(),
                Image = image,
                AdminId = adminId
            };

            this.userRepo.GetById(adminId).Returns(CreateTestUser(adminId));
            this.groupRepo.Save(Arg.Any<Group>()).Returns(expectedGroup);

            // Act
            var result = this.service.ValidateAdd(groupName, description, image, adminId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(expectedGroup.Name));
            Assert.That(result.Description, Is.EqualTo(expectedGroup.Description));
            Assert.That(result.Image, Is.EqualTo(expectedGroup.Image));
            Assert.That(result.AdminId, Is.EqualTo(expectedGroup.AdminId));

            this.groupRepo.Received(1).Save(Arg.Is<Group>(g =>
                g.Name == groupName.Trim() &&
                g.Description == description?.Trim() &&
                g.Image == image &&
                g.AdminId == adminId));
        }

        [Test]
        public void ValidateAdd_NullDescription_StillCreatesGroup()
        {
            // Arrange
            this.userRepo.GetById(1).Returns(CreateTestUser());

            // Act
            var result = this.service.ValidateAdd("Group", null, "img.jpg", 1);

            // Assert
            Assert.That(result.Description, Is.Null);
        }

        #endregion

        #region ValidateDelete Tests

        [Test]
        public void ValidateDelete_InvalidGroupId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                this.service.ValidateDelete(0));

            Assert.That(ex.Message, Does.Contain("Group ID must be a positive number"));
            Assert.That(ex.ParamName, Is.EqualTo("groupId"));
        }

        [Test]
        public void ValidateDelete_NonexistentGroup_ThrowsArgumentException()
        {
            // Arrange
            this.groupRepo.GetById(Arg.Any<long>()).Returns((Group)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                this.service.ValidateDelete(999));

            Assert.That(ex.Message, Does.Contain("Group with ID 999 does not exist"));
            Assert.That(ex.ParamName, Is.EqualTo("groupId"));
        }

        [Test]
        public void ValidateDelete_ExistingGroup_DeletesSuccessfully()
        {
            // Arrange
            var groupId = 1L;
            this.groupRepo.GetById(groupId).Returns(CreateTestGroup(groupId));

            // Act
            this.service.ValidateDelete(groupId);

            // Assert
            this.groupRepo.Received(1).DeleteById(groupId);
        }

        #endregion
    }
}
