using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApp.Entities;
using SocialApp.Repository;

namespace SocialApp.Services
{
    public class CommentService : ICommentService
    {
        ICommentRepository CommentRepository;
        IPostRepository PostRepository;
        IUserRepository UserRepository;
        public CommentService(ICommentRepository cr, IPostRepository pr, IUserRepository userRepository)
        {
            this.CommentRepository = cr;
            this.PostRepository = pr;
            this.UserRepository = userRepository;
        }
        public Comment ValidateAdd(string content, long userId, long postId)
        {
            if (content == null || content.Length == 0)
            {
                throw new Exception("Comment content cannot be empty");
            }
            if (UserRepository.GetById(userId) == null)
            {
                throw new Exception("User does not exist");
            }
            if (PostRepository.GetPostById(postId) == null)
            {
                throw new Exception("Post does not exist");
            }
            Comment comment = new Comment() { Content = content, UserId = userId, PostId = postId, CreatedDate = DateTime.Now };
            CommentRepository.Save(comment);
            return comment;
        }
        public void ValidateDelete(long commentId)
        {
            if (CommentRepository.GetById(commentId) == null)
            {
                throw new Exception("Comment does not exist");
            }
            CommentRepository.DeleteById(commentId);

        }

        /// <summary>
        /// Validates if an update is possible.
        /// </summary>
        /// <param name="commentId">The ID of the comment that updates.</param>
        /// <param name="content">The content which we want to update the comment with.</param>
        /// <exception cref="Exception">Throw when comment with given Id is not found.</exception>
        /// <exception cref="Exception">Throw when the given content is empty.</exception>
        public void UpdateComment(long commentId, string content)
        {
            if (this.CommentRepository.GetById(commentId) == null)
            {
                throw new Exception("Comment does not exist");
            }
            if (content == null || content.Length == 0)
            {
                throw new Exception("Comment content cannot be empty");
            }
            this.CommentRepository.UpdateById(commentId, content);
        }

        /// <summary>
        /// Gets all comments.
        /// </summary>
        /// <returns> A list of all the comments.</returns>
        public List<Comment> GetAllComments()
        {
            return this.CommentRepository.GetAll();
        }

        /// <summary>
        /// Gets a comment by ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to retrieve.</param>
        /// <returns>The comment with the specified ID.</returns>
        public Comment GetCommentById(int commentId)
        {
            return this.CommentRepository.GetById(commentId);
        }

        /// <summary>
        /// Gets comments by post ID.
        /// </summary>
        /// <param name="postId">The ID of the post which the comments are retrieved from.</param>
        /// <returns>A list of comments specified by the post.</returns>
        public List<Comment> GetCommentsByPostId(long postId)
        {
            return this.CommentRepository.GetCommentsForPost(postId);
        }
    }
}
