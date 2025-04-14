using System;
using NSubstitute;
using NUnit.Framework;
using SocialApp.Entities;
using SocialApp.Repository;
using SocialApp.Services;

namespace SocialApp.Tests
{
    /// <summary>
    /// Contains unit tests for the CommentService class.
    /// </summary>
    public class CommentServiceTests
    {
        /// <summary>
        /// Validates that the ValidateAdd method successfully adds a comment when provided with valid arguments.
        /// </summary>
        [Test]
        public void TestAddCommentValid()
        {
            // Arrange
            var commentRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            var commentService = new CommentService(commentRepository, postRepository, userRepository);

            string content = "TestContent";
            long userId = 1;
            long postId = 2;

            var user = new User { Id = userId, Username = "TestUser",Email="TestEmail@test.com", PasswordHash = "TestPassword", Image = "TestImage" };
            var post = new Post { Id = postId, Title = "TestPost", Content = "TestContent", CreatedDate = DateTime.Now, UserId = userId, GroupId = 1, Visibility = Enums.PostVisibility.Public, Tag = Enums.PostTag.Misc };

            userRepository.GetById(userId).Returns(user);
            postRepository.GetById(postId).Returns(post);

            // Act
            var result = commentService.AddComment(content, userId, postId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Content, Is.EqualTo(content));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.PostId, Is.EqualTo(postId));
            commentRepository.Received(1).Save(Arg.Any<Comment>());
        }

        /// <summary>
        /// Validates that the ValidateAdd method throws an exception when the content is empty.
        /// </summary>
        [Test]
        public void TestAddCommentNoContent()
        {
            // Arrange
            var commentRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            var commentService = new CommentService(commentRepository, postRepository, userRepository);

            string emptyContent = "";
            long userId = 1;
            long postId = 2;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => commentService.AddComment(emptyContent, userId, postId));
            Assert.That(ex.Message, Is.EqualTo("Comment content cannot be empty or null. (Parameter 'content')"));
        }

        /// <summary>
        /// Validates that the ValidateAdd method throws an exception when the user does not exist.
        /// </summary>
        [Test]
        public void TestAddContentNoUser()
        {
            // Arrange
            var commentRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            var commentService = new CommentService(commentRepository, postRepository, userRepository);

            string content = "Test Content";
            long invalidUserId = 999;
            long postId = 2;

            userRepository.GetById(invalidUserId).Returns((User)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => commentService.AddComment(content, invalidUserId, postId));
            Assert.That(ex.Message, Is.EqualTo($"User with ID {invalidUserId} does not exist."));
            userRepository.Received(1).GetById(invalidUserId);
        }

        /// <summary>
        /// Validates that the ValidateAdd method throws an exception when the post does not exist.
        /// </summary>
        [Test]
        public void TestAddCommentInvalidPost()
        {
            // Arrange
            var commentRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            var commentService = new CommentService(commentRepository, postRepository, userRepository);

            string content = "Test Content";
            long userId = 1;
            long invalidPostId = 999;

            var user = new User { Id = userId, Username = "TestUser", Email = "testemail", PasswordHash = "TestPassword", Image = "TestImage" };


            userRepository.GetById(userId).Returns(user);
            postRepository.GetById(invalidPostId).Returns((Post)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => commentService.AddComment(content, userId, invalidPostId));
            Assert.That(ex.Message, Is.EqualTo($"Post with ID {invalidPostId} does not exist."));
            postRepository.Received(1).GetById(invalidPostId);
        }

        ///// <summary>
        ///// Validates that the ValidateDelete method successfully deletes a comment when provided with a valid comment ID.
        ///// </summary>
        //[Test]
        //public void ValidateDelete_WithValidCommentId_SuccessfullyDeletes()
        //{
        //    // Arrange
        //    var commentRepository = Substitute.For<ICommentRepository>();
        //    var postRepository = Substitute.For<IPostRepository>();
        //    var userRepository = Substitute.For<IUserRepository>();

        //    var commentService = new CommentService(commentRepository, postRepository, userRepository);

        //    long commentId = 1;
        //    var comment = new Comment { Id = commentId, Content = "Test Content", CreatedDate = DateTime.Now, UserId = 1, PostId = 2 };

        //    commentRepository.GetById(commentId).Returns(comment);

        //    // Act
        //    commentService.ValidateDelete(commentId);

        //    // Assert
        //    commentRepository.Received(1).DeleteById(commentId);
        //}

        ///// <summary>
        ///// Validates that the ValidateDelete method throws an exception when the comment does not exist.
        ///// </summary>
        //[Test]
        //public void ValidateDelete_WithInvalidCommentId_ThrowsException()
        //{
        //    // Arrange
        //    var commentRepository = Substitute.For<ICommentRepository>();
        //    var postRepository = Substitute.For<IPostRepository>();
        //    var userRepository = Substitute.For<IUserRepository>();

        //    var commentService = new CommentService(commentRepository, postRepository, userRepository);

        //    long invalidCommentId = 999;

        //    commentRepository.GetById(invalidCommentId).Returns((Comment)null);

        //    // Act & Assert
        //    var ex = Assert.Throws<InvalidOperationException>(() => commentService.ValidateDelete(invalidCommentId));
        //    Assert.That(ex.Message, Is.EqualTo($"Comment with ID {invalidCommentId} does not exist."));
        //    commentRepository.Received(1).GetById(invalidCommentId);
        //}
    }
}