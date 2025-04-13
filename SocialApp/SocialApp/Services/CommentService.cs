using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApp.Entities;
using SocialApp.Repository;


namespace SocialApp.Services
{

    /// <summary>
    /// Service for managing comments.
    /// </summary>
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentService"/> class.
        /// </summary>
        /// <param name="cr">The comment repository.</param>
        /// <param name="pr">The post repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public CommentService(ICommentRepository cr, IPostRepository pr, IUserRepository userRepository)
        {
            this.commentRepository = cr;
            this.postRepository = pr;    // Added null checks
            this.userRepository = userRepository; // Added null checks
        }

        /// <summary>
        /// Validates and adds a new comment.
        /// </summary>
        /// <param name="content">The content of the comment.</param>
        /// <param name="userId">The ID of the user adding the comment.</param>
        /// <param name="postId">The ID of the post to which the comment is added.</param>
        /// <returns>The created Comment object.</returns>
        public Comment AddComment(string content, long userId, long postId)
        {
            if (content == null || content.Length == 0)
            {
                throw new ArgumentException("Comment content cannot be empty or null.", nameof(content));
            }

            if (this.userRepository.GetById(userId) == null)
            {
                throw new InvalidOperationException($"User with ID {userId} does not exist.");
            }

            if (this.postRepository.GetById(postId) == null)
            {
                throw new InvalidOperationException($"Post with ID {postId} does not exist.");
            }

            Comment comment = new Comment
            {
                Content = content,
                UserId = userId,
                PostId = postId,
                CreatedDate = DateTime.Now,
            };

            this.commentRepository.Save(comment);
            return comment;
        }

        /// <summary>
        /// Validates and deletes a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to be deleted.</param>
        /// <exception cref="InvalidOperationException">Thrown when the comment does not exist.</exception>
        public void ValidateDelete(long commentId)
        {
            if (commentRepository.GetById(commentId) == null)
            {
                throw new InvalidOperationException($"Comment with ID {commentId} does not exist.");
            }

            commentRepository.DeleteById(commentId);
        }

        public void ValidateUpdate(long commentId, string content)
        {
            if (commentRepository.GetById(commentId) == null)
            {
                throw new Exception("Comment does not exist");
            }
            if (content == null || content.Length == 0)
            {
                throw new Exception("Comment content cannot be empty");
            }
            commentRepository.UpdateById(commentId, content);
        }
        public List<Comment> GetAll()
        {
            return commentRepository.GetAll();
        }
        public Comment GetById(int id)
        {
            return commentRepository.GetById(id);
        }

        public List<Comment> GetCommentForPost(long postId)
        {
            return commentRepository.GetCommentsForPost(postId);
        }
    }
}
