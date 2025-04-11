using Microsoft.Data.SqlClient;
using SocialApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private const string DefaultConnectionString = "Data Source=vm;Initial Catalog=team_babes;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        public UserRepository()
        {
            _connectionString = DefaultConnectionString;
        }

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<User> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users", connection);
            connection.Open();
            return ReadUsers(command.ExecuteReader());
        }

        public List<User> GetUserFollowers(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Id IN (SELECT FollowerId FROM UserFollowers WHERE UserId = @Id)", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            return ReadUsers(command.ExecuteReader());
        }

        public List<User> GetUserFollowing(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Id IN (SELECT UserId FROM UserFollowers WHERE FollowerId = @Id)", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            return ReadUsers(command.ExecuteReader());
        }

        public void Follow(long userId, long whoToFollowId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("INSERT INTO UserFollowers (UserId, FollowerId) VALUES (@UserId, @FollowerId)", connection);
            command.Parameters.AddWithValue("@UserId", whoToFollowId);
            command.Parameters.AddWithValue("@FollowerId", userId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Unfollow(long userId, long whoToUnfollowId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("DELETE FROM UserFollowers WHERE UserId = @UserId AND FollowerId = @FollowerId", connection);
            command.Parameters.AddWithValue("@UserId", whoToUnfollowId);
            command.Parameters.AddWithValue("@FollowerId", userId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        public User GetById(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        public void Save(User entity)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("INSERT INTO Users (Username, Email, PasswordHash, Image) VALUES (@Username, @Email, @PasswordHash, @Image)", connection);
            command.Parameters.AddWithValue("@Username", entity.Username);
            command.Parameters.AddWithValue("@Email", entity.Email);
            command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            command.Parameters.AddWithValue("@Image", entity.Image ?? (object)DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateById(long id, string username, string email, string passwordHash, string image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("UPDATE Users SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, Image = @Image WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@Image", image ?? (object)DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DeleteById(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }

        private static List<User> ReadUsers(SqlDataReader reader)
        {
            var users = new List<User>();
            while (reader.Read())
            {
                users.Add(ReadUser(reader));
            }
            return users;
        }

        private static User ReadUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? string.Empty : reader.GetString(reader.GetOrdinal("Image"))
            };
        }
    }
}
