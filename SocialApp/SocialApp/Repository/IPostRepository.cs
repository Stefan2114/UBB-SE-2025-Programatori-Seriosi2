using SocialApp.Entities;
using SocialApp.Enums;
using System.Collections.Generic;

namespace SocialApp.Repository
{
    public interface IPostRepository
    {
        void DeleteById(long id);
        List<Post> GetAll();
        List<Post> GetByGroup(long groupId);
        Post GetById(long id);
        List<Post> GetByUser(long userId);
        List<Post> GetGroupsFeed(long userId);
        List<Post> GetHomeFeed(long userId);
        void Save(Post entity);
        void UpdateById(long id, string title, string content, PostVisibility visibility, PostTag tag);
    }
}