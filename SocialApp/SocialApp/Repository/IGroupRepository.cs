using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Repository
{
    public interface IGroupRepository
    {
        void DeleteGroupById(long id);

        List<Group> GetAllGroups();

        Group GetGroupsById(long id);

        List<Group> GetGroupsForUser(long userId);

        List<User> GetUsersFromGroup(long id);

        void SaveGroup(Group entity);

        void UpdateGroupById(long id, string name, string image, string description, long adminId);
    }
}