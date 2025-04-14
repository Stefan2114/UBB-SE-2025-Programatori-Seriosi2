namespace SocialApp.Services
{

    using System;
    using System.Collections.Generic;
    using SocialApp.Entities;
    using SocialApp.Repository;

    public class GroupService : IGroupService
    {
        private IGroupRepository groupRepository;
        private IUserRepository userRepository;

        public GroupService(IGroupRepository groupRepository, IUserRepository userRepository)
        {
            this.groupRepository = groupRepository;
            this.userRepository = userRepository;
        }

        public Group addGroup(string name, string desc, string image, long adminId)
        {
            if (name == null || name.Length == 0)
            {
                throw new Exception("Group name cannot be empty");
            }

            if (userRepository.GetById(adminId) == null)
            {
                throw new Exception("User does not exist");
            }

            Group group = new Group() { Name = name, AdminId = adminId, Image = image, Description = desc };

            GroupRepository.SaveGroup(group);
            return group;
        }

        public void deleteGroup(long groupId)
        {
            if (this.groupRepository.GetById(groupId) == null)
                throw new Exception("Group does not exist");

            this.groupRepository.DeleteById(groupId);
        }

        public void UpdateUser(long id, string name, string desc, string image, long adminId)
        {
            if (this.groupRepository.GetById(id) == null)
            {
                throw new Exception("Group does not exist");
            }

            if (this.userRepository.GetById(adminId) == null)
            {
                throw new Exception("User does not exist");
            }

            if (name == null || name.Length == 0)
            {
                throw new Exception("Group name cannot be empty");
            }

            this.groupRepository.UpdateById(id, name, image, desc, adminId);
        }

        public List<Group> GetAll()
        {
            return this.groupRepository.GetAll();
        }

        public Group GetById(long id)
        {
            return this.groupRepository.GetById(id);
        }

        public List<User> GetUsersFromGroup(long groupId)
        {
            return this.groupRepository.GetUsersFromGroup(groupId);
        }

        public List<Group> GetGroupsForUser(long userId)
        {
            return this.groupRepository.GetGroupsForUser(userId);
        }
    }
}