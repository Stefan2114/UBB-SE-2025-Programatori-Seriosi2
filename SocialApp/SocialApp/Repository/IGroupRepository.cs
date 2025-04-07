using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Repository
{
    public interface IGroupRepository
    {
        void DeleteById(long id);
        List<Group> GetAll();
        Group GetById(long id);
        List<Group> GetGroupsForUser(long userId);
        List<User> GetUsersFromGroup(long id);
        void Save(Group entity);
        void UpdateById(long id, string name, string image, string description, long adminId);
    }
}