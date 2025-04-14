using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface ICommentService
    {

        List<Comment> GetAllComments();
        Comment GetCommentById(int commentId);
        List<Comment> GetCommentsByPostId(long postId);
        Comment ValidateAdd(string content, long userId, long postId);
        void ValidateDelete(long commentId);
        void UpdateComment(long commentId, string content);
    }
}