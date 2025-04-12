namespace SocialApp.Tests
{
    using NSubstitute;
    using SocialApp;
    using SocialApp.Entities;
    using SocialApp.Enums;
    using SocialApp.Repository;
    using SocialApp.Services;

    /// <summary>
    /// Contains unit tests for the CommentService class.
    /// </summary>
    public class CommentServiceTests
    {
        /// <summary>
        /// Validates that the UpdateComment method functions correctly when provided with valid arguments.
        /// </summary>
        [Test]
        public void ValidateUpdateComment_WithValidArguments()
        {
            // Arange
            var commentsRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            CommentService commentService = new CommentService(commentsRepository,postRepository,userRepository);

            long commentId = 1;
            long userId = 1;
            long postId = 1;
            string? content = "Test Content";

            Comment testComment = new Comment { Id = commentId, UserId=userId, PostId = postId, Content = content, CreatedDate = DateTime.Now };

            commentsRepository.GetById(commentId).Returns(testComment);

            // Act
            commentService.UpdateComment(commentId, content);

            // Assert
            commentsRepository.Received(1).GetById(commentId);
            commentsRepository.Received(1).UpdateById(commentId, content);
        }

        /// <summary>
        /// Validates that the UpdateComment function throws an exception when provided with an invalid comment Id.
        /// </summary>
        [Test]
        public void ValidateUpdateComment_WithInvalidCommentId_ThrowsException()
        {
            // Arange
            var commentsRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            CommentService commentService = new CommentService(commentsRepository, postRepository, userRepository);

            long commentId = 1;
            string? content = "Test Content";

            commentsRepository.GetById(commentId).Returns((Comment)null);

            // Act & Assert
            Assert.Throws<Exception>(() => commentService.UpdateComment(commentId,content), "Comment does not exist");
            commentsRepository.Received(1).GetById(commentId);
        }

        /// <summary>
        /// Validates that the UpdateComment function throws an exception when the provided content is empty.
        /// </summary>
        [Test]
        public void ValidateUpdateComment_WithInvalidContent_ThrowsException()
        {
            // Arange
            var commentsRepository = Substitute.For<ICommentRepository>();
            var postRepository = Substitute.For<IPostRepository>();
            var userRepository = Substitute.For<IUserRepository>();

            CommentService commentService = new CommentService(commentsRepository, postRepository, userRepository);

            long commentId = 1;
            long userId = 1;
            long postId = 1;
            string? content = "Test Content";
            string? emptycontent = string.Empty;

            Comment testComment = new Comment { Id = commentId, UserId = userId, PostId = postId, Content = content, CreatedDate = DateTime.Now };

            commentsRepository.GetById(commentId).Returns(testComment);

            // Act & Assert
            Assert.Throws<Exception>(() => commentService.UpdateComment(commentId, emptycontent), "Comment content cannot be empty");
            commentsRepository.Received(1).GetById(commentId);
        }
    }
}
