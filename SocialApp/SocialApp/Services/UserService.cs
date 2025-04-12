namespace SocialApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SocialApp.Entities;
    using SocialApp.Repository;

    public class UserService : IUserService
    {
        private IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">IUserRepository instance of the repo.</param>
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Validates and adds a new user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="image">The profile image of the user.</param>
        public void AddUser(string username, string email, string password, string image)
        {
            if (username == null || username.Length == 0)
            {
                throw new Exception("Username cannot be empty");
            }

            if (email == null || email.Length == 0)
            {
                throw new Exception("Email cannot be empty");
            }

            if (password == null || password.Length == 0)
            {
                throw new Exception("Password cannot be empty");
            }

            this.userRepository.Save(new User() { Username = username, Email = email, PasswordHash = password, Image = image });
        }

        /// <summary>
        /// Validates and deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        public void DeleteUser(long id)
        {
            if (this.userRepository.GetById(id) == null)
            {
                throw new Exception("User does not exist");
            }

            this.userRepository.DeleteById(id);
        }

        /// <summary>
        /// Validates and updates a user's information.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="username">The new username of the user.</param>
        /// <param name="email">The new email of the user.</param>
        /// <param name="password">The new password of the user.</param>
        /// <param name="image">The new profile image of the user (optional).</param>
        public void UpdateUser(long id, string username, string email, string password, string? image)
        {
            if (this.userRepository.GetById(id) == null)
            {
                throw new Exception("User does not exist");
            }

            this.userRepository.UpdateById(id, username, email, password, image);
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public List<User> GetAll()
        {
            return this.userRepository.GetAll();
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        public User GetById(long id)
        {
            return this.userRepository.GetById(id);
        }

        /// <summary>
        /// Retrieves the followers of a user.
        /// </summary>
        /// <param name="id">The ID of the user whose followers to retrieve.</param>
        /// <returns>A list of users who follow the specified user.</returns>
        public List<User> GetUserFollowers(long id)
        {
            return this.userRepository.GetUserFollowers(id);
        }

        /// <summary>
        /// Retrieves the users that a user is following.
        /// </summary>
        /// <param name="id">The ID of the user whose following list to retrieve.</param>
        /// <returns>A list of users that the specified user is following.</returns>
        public List<User> GetUserFollowing(long id)
        {
            return this.userRepository.GetUserFollowing(id);
        }

        /// <summary>
        /// Allows a user to follow another user.
        /// </summary>
        /// <param name="userId">The ID of the user who wants to follow.</param>
        /// <param name="whoToFollowId">The ID of the user to be followed.</param>
        public void FollowUser(long userId, long whoToFollowId)
        {
            if (this.userRepository.GetById(userId) == null)
            {
                throw new Exception("User does not exist");
            }

            if (this.userRepository.GetById(whoToFollowId) == null)
            {
                throw new Exception("User to follow does not exist");
            }

            this.userRepository.Follow(userId, whoToFollowId);
        }

        /// <summary>
        /// Allows a user to unfollow another user.
        /// </summary>
        /// <param name="userId">The ID of the user who wants to unfollow.</param>
        /// <param name="whoToUnfollowId">The ID of the user to be unfollowed.</param>
        public void UnfollowUser(long userId, long whoToUnfollowId)
        {
            if (this.userRepository.GetById(userId) == null)
            {
                throw new Exception("User does not exist");
            }

            if (this.userRepository.GetById(whoToUnfollowId) == null)
            {
                throw new Exception("User to unfollow does not exist");
            }

            this.userRepository.Unfollow(userId, whoToUnfollowId);
        }

        /// <summary>
        /// Searches for users that the current user is following based on a query.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="query">The search query to filter users by username.</param>
        /// <returns>A list of users matching the search query.</returns>
        public List<User> SearchUsers(long userId, string query)
        {
            // Get the list of users the current user is following
            var followingUsers = this.GetUserFollowing(userId);

            // Filter those users by the search query (matching the username)
            return followingUsers.Where(u => u.Username.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
