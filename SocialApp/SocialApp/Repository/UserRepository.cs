namespace SocialApp.Repository
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using SocialApp.Entities;
    public class UserRepository : IUserRepository
    {
        private const string ConnectionString = "Data Source=vm;Initial Catalog=team_babes;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private readonly SqlConnection connection;

        public UserRepository() : this(ConnectionString) { }

        public UserRepository(string connectionString)
        {
           this.connection = new SqlConnection(connectionString);
        }

        public List<User> GetAll()
        {
            return this.ExecuteQuery("SELECT * FROM Users", MapUser);
        }

        public List<User> GetUserFollowers(long userId)
        {
            const string query = "SELECT * FROM Users WHERE Id IN (SELECT FollowerId FROM UserFollowers WHERE UserId = @UserId)";
            return this.ExecuteQuery(query, MapUser, new SqlParameter("@UserId", userId));
        }

        public List<User> GetUserFollowing(long userId)
        {
            const string query = "SELECT * FROM Users WHERE Id IN (SELECT UserId FROM UserFollowers WHERE FollowerId = @UserId)";
            return this.ExecuteQuery(query, MapUser, new SqlParameter("@UserId", userId));
        }

        public void Follow(long followerId, long followeeId)
        {
            const string query = "INSERT INTO UserFollowers (UserId, FollowerId) VALUES (@UserId, @FollowerId)";
            this.ExecuteNonQuery(query, new SqlParameter("@UserId", followeeId), new SqlParameter("@FollowerId", followerId));
        }

        public void Unfollow(long followerId, long followeeId)
        {
            const string query = "DELETE FROM UserFollowers WHERE UserId = @UserId AND FollowerId = @FollowerId";
            this.ExecuteNonQuery(query, new SqlParameter("@UserId", followeeId), new SqlParameter("@FollowerId", followerId));
        }

        public User GetByEmail(string email)
        {
            const string query = "SELECT * FROM Users WHERE Email = @Email";
            return this.ExecuteSingle(query, MapUser, new SqlParameter("@Email", email));
        }

        public User GetById(long id)
        {
            const string query = "SELECT * FROM Users WHERE Id = @Id";
            return this.ExecuteSingle(query, MapUser, new SqlParameter("@Id", id));
        }

        public void Save(User user)
        {
            const string query = "INSERT INTO Users (Username, Email, PasswordHash, Image) VALUES (@Username, @Email, @PasswordHash, @Image)";
            this.ExecuteNonQuery(query, new SqlParameter("@Username", user.Username), new SqlParameter("@Email", user.Email), new SqlParameter("@PasswordHash", user.PasswordHash), new SqlParameter("@Image", user.Image));
        }

        public void UpdateById(long id, string username, string email, string passwordHash, string image)
        {
            const string query = "UPDATE Users SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, Image = @Image WHERE Id = @Id";
            this.ExecuteNonQuery(query, new SqlParameter("@Id", id), new SqlParameter("@Username", username), new SqlParameter("@Email", email), new SqlParameter("@PasswordHash", passwordHash), new SqlParameter("@Image", image));
        }

        public void DeleteById(long id)
        {
            const string query = "DELETE FROM Users WHERE Id = @Id";
            this.ExecuteNonQuery(query, new SqlParameter("@Id", id));
        }

        private List<User> ExecuteQuery(string query, Func<SqlDataReader, User> mapper, params SqlParameter[] parameters)
        {
            try
            {
                this.connection.Open();
                using var command = new SqlCommand(query, this.connection);
                command.Parameters.AddRange(parameters);

                var results = new List<User>();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(mapper(reader));
                }

                return results;
            }
            finally
            {
                this.connection.Close();
            }
        }

        private User ExecuteSingle(string query, Func<SqlDataReader, User> mapper, params SqlParameter[] parameters)
        {
            try
            {
                this.connection.Open();
                using var command = new SqlCommand(query, this.connection);
                command.Parameters.AddRange(parameters);

                using var reader = command.ExecuteReader();
                return reader.Read() ? mapper(reader) : null;
            }
            finally
            {
                this.connection.Close();
            }
        }


        private void ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                this.connection.Open();
                using var command = new SqlCommand(query, this.connection);
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
            finally
            {
               this.connection.Close();
            }
        }

        private static User MapUser(SqlDataReader reader)
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