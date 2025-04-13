namespace SocialApp.Tests
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SocialApp.Entities;
    using SocialApp.Enums;
    using SocialApp.Repository;
    using SocialApp.Services;

    /// <summary>
    /// Contains unit tests for the ReactionService class.
    /// </summary>
    public class ReactionServiceTests
    {
        private IReactionRepository reactionRepository;
        private ReactionService reactionService;

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.reactionRepository = Substitute.For<IReactionRepository>();
            this.reactionService = new ReactionService(this.reactionRepository);
        }

        /// <summary>
        /// Validates that ValidateAdd updates the reaction if one already exists.
        /// </summary>
        [Test]
        public void ValidateAdd_ReactionExists_UpdatesReaction()
        {
            // Arrange
            long userId = 1;
            long postId = 10;
            ReactionType newType = ReactionType.Like;

            var existingReaction = new Reaction { UserId = userId, PostId = postId, Type = ReactionType.Like };
            this.reactionRepository.GetByUserAndPost(userId, postId).Returns(existingReaction);

            // Act
            var result = this.reactionService.ValidateAdd(userId, postId, newType);

            // Assert
            this.reactionRepository.Received(1).UpdateByUserAndPost(userId, postId, newType);
            this.reactionRepository.Received(2).GetByUserAndPost(userId, postId); // once before, once after update
            Assert.That(result, Is.EqualTo(existingReaction));
        }

        /// <summary>
        /// Validates that ValidateAdd adds a new reaction if one does not exist.
        /// </summary>
        [Test]
        public void ValidateAdd_ReactionDoesNotExist_CreatesReaction()
        {
            // Arrange
            long userId = 1;
            long postId = 10;
            ReactionType type = ReactionType.Love;

            this.reactionRepository.GetByUserAndPost(userId, postId).Returns(null as Reaction);

            // Act
            var result = this.reactionService.ValidateAdd(userId, postId, type);

            // Assert
            this.reactionRepository.Received(1).Save(Arg.Is<Reaction>(r =>
                r.UserId == userId && r.PostId == postId && r.Type == type));

            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.PostId, Is.EqualTo(postId));
            Assert.That(result.Type, Is.EqualTo(type));
        }

        /// <summary>
        /// Validates that ValidateDelete throws an exception if the reaction does not exist.
        /// </summary>
        [Test]
        public void ValidateDelete_ReactionDoesNotExist_ThrowsException()
        {
            // Arrange
            long userId = 1;
            long postId = 10;

            this.reactionRepository.GetByUserAndPost(userId, postId).Returns(null as Reaction);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => this.reactionService.ValidateDelete(userId, postId));
            Assert.That(ex.Message, Is.EqualTo("Reaction does not exist"));
        }

        /// <summary>
        /// Validates that ValidateDelete deletes the reaction if it exists.
        /// </summary>
        [Test]
        public void ValidateDelete_ReactionExists_DeletesReaction()
        {
            // Arrange
            long userId = 1;
            long postId = 10;

            this.reactionRepository.GetByUserAndPost(userId, postId).Returns(new Reaction { UserId = userId, PostId = postId });

            // Act
            this.reactionService.ValidateDelete(userId, postId);

            // Assert
            this.reactionRepository.Received(1).DeleteByUserAndPost(userId, postId);
        }

        /// <summary>
        /// Validates that GetAll returns all reactions.
        /// </summary>
        [Test]
        public void GetAll_ReturnsAllReactions()
        {
            // Arrange
            var reactions = new List<Reaction>
            {
                new Reaction { UserId = 1, PostId = 1, Type = ReactionType.Like },
                new Reaction { UserId = 2, PostId = 2, Type = ReactionType.Anger },
            };
            this.reactionRepository.GetAll().Returns(reactions);

            // Act
            var result = this.reactionService.GetAll();

            // Assert
            Assert.That(result, Is.EqualTo(reactions));
        }

        /// <summary>
        /// Validates that GetReactionsForPost returns the correct reactions.
        /// </summary>
        [Test]
        public void GetReactionsForPost_ReturnsCorrectReactions()
        {
            // Arrange
            long postId = 1;
            var reactions = new List<Reaction>
            {
                new Reaction { UserId = 1, PostId = postId, Type = ReactionType.Like },
                new Reaction { UserId = 2, PostId = postId, Type = ReactionType.Love },
            };
            this.reactionRepository.GetByPost(postId).Returns(reactions);

            // Act
            var result = this.reactionService.GetReactionsForPost(postId);

            // Assert
            Assert.That(result, Is.EqualTo(reactions));
        }
    }
}
