using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApp.Entities;
using SocialApp.Repository;
//test
namespace SocialApp.Services
{
    public class CommentService : ICommentService // Changed to public
    {
        private readonly ICommentRepository CommentRepository; // Changed to readonly and camelCase
        private readonly IPostRepository PostRepository;       // Changed to readonly and camelCase
        private readonly IUserRepository UserRepository;       // Changed to readonly and camelCase

        public CommentService(ICommentRepository cr, IPostRepository pr, IUserRepository userRepository)
        {
            this.CommentRepository = cr ?? throw new ArgumentNullException(nameof(cr)); // Added null checks
            this.PostRepository = pr ?? throw new ArgumentNullException(nameof(pr));    // Added null checks
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); // Added null checks
        }

        public Comment ValidateAdd(string content, long userId, long postId)
        {
            if (string.IsNullOrWhiteSpace(content)) // Used IsNullOrWhiteSpace for better validation
            {
                throw new ArgumentException("Comment content cannot be empty or null.", nameof(content));
            }

            if (UserRepository.GetById(userId) == null)
            {
                throw new InvalidOperationException($"User with ID {userId} does not exist.");
            }

            if (PostRepository.GetById(postId) == null)
            {
                throw new InvalidOperationException($"Post with ID {postId} does not exist.");
            }

            Comment comment = new Comment
            {
                Content = content,
                UserId = userId,
                PostId = postId,
                CreatedDate = DateTime.Now
            };

            CommentRepository.Save(comment);
            return comment;
        }

        public void ValidateDelete(long commentId)
        {
            if (CommentRepository.GetById(commentId) == null)
            {
                throw new InvalidOperationException($"Comment with ID {commentId} does not exist.");
            }

            CommentRepository.DeleteById(commentId);
        }

        public void ValidateUpdate(long commentId, string content)
        {
            if (CommentRepository.GetById(commentId) == null)
            {
                throw new Exception("Comment does not exist");
            }
            if (content == null || content.Length == 0)
            {
                throw new Exception("Comment content cannot be empty");
            }
            CommentRepository.UpdateById(commentId, content);
        }
        public List<Comment> GetAll()
        {
            return CommentRepository.GetAll();
        }
        public Comment GetById(int id)
        {
            return CommentRepository.GetById(id);
        }

        public List<Comment> GetCommentForPost(long postId)
        {
            return CommentRepository.GetCommentsForPost(postId);
        }
    }
}
