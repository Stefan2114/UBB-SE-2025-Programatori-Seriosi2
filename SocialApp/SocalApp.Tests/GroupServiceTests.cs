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
		private IGroupRepository _groupRepository;
		private IUserRepository _userRepository;
		private GroupService _groupService;

		[SetUp]
		public void Setup()
		{
			_groupRepository = Substitute.For<IGroupRepository>();
			_userRepository = Substitute.For<IUserRepository>();
			_groupService = new GroupService(_groupRepository, _userRepository);
		}

		#region ValidateAdd Tests

		[Test]
		public void ValidateAdd_GroupNameIsEmpty_ThrowsArgumentException()
		{
			// Arrange
			string groupName = string.Empty;
			string description = "Valid description";
			string image = "image.jpg";
			long adminId = 1;

			_userRepository.GetById(adminId).Returns(new User { Id = adminId });

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				_groupService.ValidateAdd(groupName, description, image, adminId));

			Assert.That(ex.Message, Does.Contain("Group name cannot be empty"));
			Assert.That(ex.ParamName, Is.EqualTo("groupName"));
		}

		[Test]
		public void ValidateAdd_GroupNameIsWhitespace_ThrowsArgumentException()
		{
			// Arrange
			string groupName = "   ";
			string description = "Valid description";
			string image = "image.jpg";
			long adminId = 1;

			_userRepository.GetById(adminId).Returns(new User { Id = adminId });

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				_groupService.ValidateAdd(groupName, description, image, adminId));

			Assert.That(ex.Message, Does.Contain("Group name cannot be empty"));
		}

		[Test]
		public void ValidateAdd_AdminUserDoesNotExist_ThrowsArgumentException()
		{
			// Arrange
			string groupName = "Valid Group";
			string description = "Valid description";
			string image = "image.jpg";
			long adminId = 1;

			_userRepository.GetById(adminId).Returns((User)null);

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				_groupService.ValidateAdd(groupName, description, image, adminId));

			Assert.That(ex.Message, Does.Contain("User does not exist"));
			Assert.That(ex.ParamName, Is.EqualTo("adminUserId"));
		}

		[Test]
		public void ValidateAdd_ValidParameters_CreatesGroup()
		{
			// Arrange
			string groupName = "Valid Group";
			string description = "Valid description";
			string image = "image.jpg";
			long adminId = 1;

			var expectedGroup = new Group
			{
				Name = groupName.Trim(),
				Description = description?.Trim(),
				Image = image,
				AdminId = adminId
			};

			_userRepository.GetById(adminId).Returns(new User { Id = adminId });
			_groupRepository.Save(Arg.Any<Group>()).Returns(expectedGroup);

			// Act
			var result = _groupService.ValidateAdd(groupName, description, image, adminId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Name, Is.EqualTo(groupName.Trim()));
			Assert.That(result.Description, Is.EqualTo(description?.Trim()));
			Assert.That(result.Image, Is.EqualTo(image));
			Assert.That(result.AdminId, Is.EqualTo(adminId));

			_groupRepository.Received(1).Save(Arg.Is<Group>(g =>
				g.Name == groupName.Trim() &&
				g.Description == description?.Trim() &&
				g.Image == image &&
				g.AdminId == adminId));
		}

		[Test]
		public void ValidateAdd_NullDescription_StillCreatesGroup()
		{
			// Arrange
			string groupName = "Valid Group";
			string description = null;
			string image = "image.jpg";
			long adminId = 1;

			_userRepository.GetById(adminId).Returns(new User { Id = adminId });

			// Act
			var result = _groupService.ValidateAdd(groupName, description, image, adminId);

			// Assert
			Assert.That(result.Description, Is.Null);
		}

		#endregion

		#region ValidateDelete Tests

		[Test]
		public void ValidateDelete_InvalidGroupId_ThrowsArgumentException()
		{
			// Arrange
			long invalidGroupId = 0;

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				_groupService.ValidateDelete(invalidGroupId));

			Assert.That(ex.Message, Does.Contain("Group ID must be a positive number"));
			Assert.That(ex.ParamName, Is.EqualTo("groupId"));
		}

		[Test]
		public void ValidateDelete_GroupDoesNotExist_ThrowsArgumentException()
		{
			// Arrange
			long nonExistentGroupId = 999;

			_groupRepository.GetById(nonExistentGroupId).Returns((Group)null);

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() =>
				_groupService.ValidateDelete(nonExistentGroupId));

			Assert.That(ex.Message, Does.Contain($"Group with ID {nonExistentGroupId} does not exist"));
			Assert.That(ex.ParamName, Is.EqualTo("groupId"));
		}

		[Test]
		public void ValidateDelete_ValidGroupId_DeletesGroup()
		{
			// Arrange
			long validGroupId = 1;
			var existingGroup = new Group { Id = validGroupId };

			_groupRepository.GetById(validGroupId).Returns(existingGroup);

			// Act
			_groupService.ValidateDelete(validGroupId);

			// Assert
			_groupRepository.Received(1).DeleteById(validGroupId);
		}

		#endregion

		
	}
}