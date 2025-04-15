using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IGroupService
    {

        Group GetGroupById(long id);

        List<Group> GetGroups(long userId);
        List<User> GetUsersFromGroup(long groupId);
        Group AddGroup(string name, string desccription, string image, long adminId);
        void DeleteGroup(long groupId);
        void UpdateGroup(long id, string name, string description, string image, long adminId);
    }
}