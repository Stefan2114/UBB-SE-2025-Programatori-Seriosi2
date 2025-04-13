using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface ICommentService
    {
        List<Comment> GetAll();
        Comment GetById(int id);
        List<Comment> GetCommentForPost(long postId);
        Comment AddComment(string content, long userId, long postId);
        void ValidateDelete(long commentId);
        void ValidateUpdate(long commentId, string content);
    }
}