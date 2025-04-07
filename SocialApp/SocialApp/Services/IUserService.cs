using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IUserService
    {
        void FollowUser(long userId, long whoToFollowId);
        List<User> GetAll();
        User GetById(long id);
        List<User> GetUserFollowers(long id);
        List<User> GetUserFollowing(long id);
        List<User> SearchUsers(long userId, string query);
        void UnfollowUser(long userId, long whoToUnfollowId);
        void ValidateAdd(string username, string email, string password, string image);
        void ValidateDelete(long id);
        void ValidateUpdate(long id, string username, string email, string password, string? image);
    }
}