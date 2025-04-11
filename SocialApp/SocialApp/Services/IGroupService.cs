using SocialApp.Entities;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IGroupService
    {
        /// <summary>
        /// Gets all groups from the repository
        /// </summary>
        /// <returns>List of all groups</returns>
        List<Group> GetAll();

        /// <summary>
        /// Gets a group by its ID
        /// </summary>
        /// <param name="id">The ID of the group to retrieve</param>
        /// <returns>The requested group</returns>
        /// <exception cref="ArgumentException">Thrown when id is invalid or group doesn't exist</exception>
        Group GetById(long id);

        /// <summary>
        /// Gets all groups for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>List of groups the user is a member of</returns>
        /// <exception cref="ArgumentException">Thrown when userId is invalid or user doesn't exist</exception>
        List<Group> GetGroupsForUser(long userId);

        /// <summary>
        /// Gets all users in a specific group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <returns>List of users in the group</returns>
        /// <exception cref="ArgumentException">Thrown when groupId is invalid or group doesn't exist</exception>
        List<User> GetUsersFromGroup(long groupId);

        /// <summary>
        /// Validates and adds a new group
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="desc">Description of the group</param>
        /// <param name="image">Base64 encoded image of the group</param>
        /// <param name="adminId">ID of the admin user</param>
        /// <returns>The newly created group</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        Group ValidateAdd(string name, string desc, string image, long adminId);

        /// <summary>
        /// Validates and deletes a group
        /// </summary>
        /// <param name="groupId">ID of the group to delete</param>
        /// <exception cref="ArgumentException">Thrown when groupId is invalid or group doesn't exist</exception>
        void ValidateDelete(long groupId);

        /// <summary>
        /// Validates and updates a group
        /// </summary>
        /// <param name="id">ID of the group to update</param>
        /// <param name="name">New name of the group</param>
        /// <param name="desc">New description of the group</param>
        /// <param name="image">New base64 encoded image of the group</param>
        /// <param name="adminId">New admin user ID</param>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        void ValidateUpdate(long id, string name, string desc, string image, long adminId);
    }
}