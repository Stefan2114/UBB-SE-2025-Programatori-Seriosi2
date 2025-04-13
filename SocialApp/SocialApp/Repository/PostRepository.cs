﻿using Microsoft.Data.SqlClient;
using SocialApp.Entities;
using SocialApp.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.Repository
{
    public class PostRepository : IPostRepository
    {

        private string loginString = "Data Source=vm;" +
    "Initial Catalog=team_babes;" +
    "Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private SqlConnection connection;

        public PostRepository()
        {
            this.connection = new SqlConnection(loginString);
        }

        public List<Post> GetAll()
        {
            connection.Open();
            List<Post> posts = new List<Post>();

            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Posts", connection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Post post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? 0 : reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("PostTag")) ? PostTag.Misc : (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag")),
                };
                posts.Add(post);
            }

            reader.Close();
            connection.Close();
            return posts;
        }

        public List<Post> GetHomeFeed(long userId)
        {
            connection.Open();
            List<Post> posts = new List<Post>();
            SqlCommand selectCommand = new SqlCommand(
                "SELECT * FROM Posts WHERE UserId IN (SELECT UserId FROM UserFollowers WHERE FollowerId = @UserId AND (PostVisibility = 2 OR PostVisibility = 3)) OR UserId = @UserId OR PostVisibility = 3 ORDER BY CreatedDate DESC",
                connection
            );
            if (userId == -1)
            {
                selectCommand = new SqlCommand(
                    "SELECT * FROM Posts WHERE PostVisibility = 3 ORDER BY CreatedDate DESC",
                    connection
                );
            }
            selectCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Post post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? 0 : reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("PostTag")) ? PostTag.Misc : (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag"))
                };
                posts.Add(post);
            }
            reader.Close();
            connection.Close();
            return posts;
        }

        public List<Post> GetGroupsFeed(long userId)
        {
            connection.Open();
            List<Post> posts = new List<Post>();
            SqlCommand selectCommand = new SqlCommand(
                "SELECT * FROM Posts WHERE GroupId IN (SELECT GroupId FROM GroupUsers WHERE UserId = @UserId) ORDER BY CreatedDate DESC",
                connection
            );
            selectCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                Post post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag"))
                };
                posts.Add(post);
            }

            reader.Close();
            connection.Close();
            return posts;
        }

        public List<Post> GetByUser(long userId)
        {
            connection.Open();
            List<Post> posts = new List<Post>();
            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Posts WHERE UserId = @UserId ORDER BY CreatedDate DESC", connection);
            selectCommand.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Post post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? 0 : reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("PostTag")) ? PostTag.Misc : (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag"))
                };
                posts.Add(post);
            }
            reader.Close();
            connection.Close();
            return posts;
        }

        public List<Post> GetByGroup(long groupId)
        {
            connection.Open();
            List<Post> posts = new List<Post>();
            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Posts WHERE GroupId = @GroupId ORDER BY CreatedDate DESC", connection);
            selectCommand.Parameters.AddWithValue("@GroupId", groupId);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Post post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? 0 : reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("PostTag")) ? PostTag.Misc : (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag"))
                };
                posts.Add(post);
            }
            reader.Close();
            connection.Close();
            return posts;
        }

        public Post GetById(long id)
        {
            connection.Open();
            Post post = null;

            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Posts WHERE Id = @Id ORDER BY CreatedDate DESC", connection);
            selectCommand.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = selectCommand.ExecuteReader();
            if (reader.Read())
            {
                post = new Post
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? 0 : reader.GetInt64(reader.GetOrdinal("GroupId")),
                    Visibility = (PostVisibility)reader.GetInt32(reader.GetOrdinal("PostVisibility")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("PostTag")) ? PostTag.Misc : (PostTag)reader.GetInt32(reader.GetOrdinal("PostTag"))
                };
            }

            reader.Close();
            connection.Close();
            return post;
        }

        public void Save(Post entity)
        {
            connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Posts (Title, Content, CreatedDate, UserId, PostVisibility, GroupId, PostTag) VALUES (@Title, @Description, @CreatedDate, @UserId, @PostVisibility, @GroupId, @PostTag)",
                connection
            );
            insertCommand.Parameters.AddWithValue("@Title", entity.Title);
            insertCommand.Parameters.AddWithValue("@Description", entity.Content);
            insertCommand.Parameters.AddWithValue("@CreatedDate", entity.CreatedDate);
            insertCommand.Parameters.AddWithValue("@UserId", entity.UserId);
            insertCommand.Parameters.AddWithValue("@PostVisibility", (int)entity.Visibility);
            insertCommand.Parameters.AddWithValue("@GroupId", entity.GroupId);
            insertCommand.Parameters.AddWithValue("@PostTag", (int)entity.Tag);

            insertCommand.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateById(long id, string title, string content, PostVisibility visibility, PostTag tag)
        {
            connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Posts SET Title = @Title, Description = @Description, PostVisibility = @PostVisibility, PostTag = @PostTag WHERE Id = @Id",
                connection
            );

            updateCommand.Parameters.AddWithValue("@Id", id);
            updateCommand.Parameters.AddWithValue("@Title", title);
            updateCommand.Parameters.AddWithValue("@Description", content);
            updateCommand.Parameters.AddWithValue("@PostVisibility", (int)visibility);
            updateCommand.Parameters.AddWithValue("@PostTag", (int)tag);

            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

        public void DeleteById(long id)
        {
            connection.Open();

            SqlCommand deleteCommand = new SqlCommand("DELETE FROM Posts WHERE Id = @Id", connection);
            deleteCommand.Parameters.AddWithValue("@Id", id);
            deleteCommand.ExecuteNonQuery();

            connection.Close();
        }
    }
}
