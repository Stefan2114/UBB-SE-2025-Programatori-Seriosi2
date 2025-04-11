using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using SocialApp.Entities;
using SocialApp.Repository;

namespace SocialApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly IUserRepository userRepository;

        public GroupService(IGroupRepository groupRepository, IUserRepository userRepository)
        {
            this.groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Group ValidateAdd(string groupName, string groupDescription, string groupImage, long adminUserId)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));
            }

            if (userRepository.GetById(adminUserId) == null)
            {
                throw new ArgumentException("User does not exist", nameof(adminUserId));
            }

            var newGroup = new Group 
            { 
                Name = groupName.Trim(),
                Description = groupDescription?.Trim(),
                Image = groupImage,
                AdminId = adminUserId 
            };

            groupRepository.Save(newGroup);
            return newGroup;
        }

        public void ValidateDelete(long groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException("Group ID must be a positive number", nameof(groupId));
            }

            var group = groupRepository.GetById(groupId);
            if (group == null)
            {
                throw new ArgumentException($"Group with ID {groupId} does not exist", nameof(groupId));
            }

            groupRepository.DeleteById(groupId);
        }

        public void ValidateUpdate(long groupId, string groupName, string groupDescription, string groupImage, long adminUserId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException("Group ID must be a positive number", nameof(groupId));
            }

            if (adminUserId <= 0)
            {
                throw new ArgumentException("Admin user ID must be a positive number", nameof(adminUserId));
            }

            var group = groupRepository.GetById(groupId);
            if (group == null)
            {
                throw new ArgumentException($"Group with ID {groupId} does not exist", nameof(groupId));
            }

            var adminUser = userRepository.GetById(adminUserId);
            if (adminUser == null)
            {
                throw new ArgumentException($"User with ID {adminUserId} does not exist", nameof(adminUserId));
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be empty or whitespace", nameof(groupName));
            }

            groupRepository.UpdateById(groupId, groupName.Trim(), groupImage, groupDescription?.Trim(), adminUserId);
        }

        public List<Group> GetAll()
        {
            var groups = groupRepository.GetAll();
            return groups ?? new List<Group>();
        }

        public Group GetById(long groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException("Group ID must be a positive number", nameof(groupId));
            }

            var requestedGroup = groupRepository.GetById(groupId);
            if (requestedGroup == null)
            {
                throw new ArgumentException($"Group with ID {groupId} does not exist", nameof(groupId));
            }
            return requestedGroup;
        }

        public List<User> GetUsersFromGroup(long groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException("Group ID must be a positive number", nameof(groupId));
            }

            var group = groupRepository.GetById(groupId);
            if (group == null)
            {
                throw new ArgumentException($"Group with ID {groupId} does not exist", nameof(groupId));
            }

            var users = groupRepository.GetUsersFromGroup(groupId);
            return users ?? new List<User>();
        }

        public List<Group> GetGroupsForUser(long userId)
        {
            if (userRepository.GetById(userId) == null)
            {
                throw new ArgumentException("User does not exist", nameof(userId));
            }

            return groupRepository.GetGroupsForUser(userId);
        }
    }
}
