using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IGroupService
    {
      
   
        List<Group> GetGroupsForUser(long userId);
        List<User> GetUsersFromGroup(long groupId);
        Group ValidateAdd(string name, string desc, string image, long adminId);
        void ValidateDelete(long groupId);
        void ValidateUpdate(long id, string name, string desc, string image, long adminId);
    }
}