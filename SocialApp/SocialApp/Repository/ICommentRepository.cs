using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Repository
{
    public interface ICommentRepository
    {
        void DeleteById(long id);
        List<Comment> GetAll();
        Comment GetById(long id);
        List<Comment> GetCommentsForPost(long postId);
        void Save(Comment entity);
        void UpdateById(long id, string content);
    }
}