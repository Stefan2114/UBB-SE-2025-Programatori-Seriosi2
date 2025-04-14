using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IGroupService
    {


        List<Group> GetGroupsForUser(long userId);
        List<User> GetUsersFromGroup(long groupId);
        Group addGroup(string name, string desc, string image, long adminId);
        void deleteGroup(long groupId);
        void UpdateUser(long id, string name, string desc, string image, long adminId);
    }
}