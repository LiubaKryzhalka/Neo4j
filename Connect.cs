using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Mongodb
{
    public class Connect
    {
        static string ConnectionString()
        {
            return "mongodb://localhost:27017/test";
        }
        private User _user;
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<User> _users;
        private IMongoCollection<Post> _posts;
        public Connect()
        {
            _client = new MongoClient(ConnectionString());
            _database = _client.GetDatabase("test");
            _users = _database.GetCollection<User>("users");
            _posts = _database.GetCollection<Post>("posts");
        }
        public bool Log_in(string username, string password)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = Builders<User>.Filter.Eq("username", username) & filterBuilder.Eq("password", password);
            var found = _users.Find(filter).ToList();
            if (found.Count == 0)
            {
                return false;
            }
            _user = found[0];
            return true;
        }
        public List<Post> Posts()
        {
            var filter = Builders<Post>.Filter.In("username", _user.Subscribers);
            var result_posts = _posts.Find(filter).Sort("{date : -1}").ToList();
            return result_posts;
        }
        public List<Post> User_Posts(string username)
        {
            var filter = Builders<Post>.Filter.Eq("username", username);
            var result_posts = _posts.Find(filter).Sort("{date : -1}").ToList();
            return result_posts;
        }
        public List<User> GetSubscribers()
        {
            var filter = Builders<User>.Filter.In("username", _user.Subscribers);
            var subscribers = _users.Find(filter).ToList();
            return subscribers;
        }
        public void LikePost(Post post)
        {
            if (!post.Likes.Contains(_user.UserName))
            {
                post.Likes.Add(_user.UserName);
                Console.WriteLine("You liked this post");
                _posts.ReplaceOne(p => p.Id == post.Id, post);
            }
            else
            {
                post.Likes.Remove(_user.UserName);
                Console.WriteLine("You remove your like");
                _posts.ReplaceOne(p => p.Id == post.Id, post);
            }
        }
        public void WriteComment(Post post, string comment)
        {
            post.Comments.Add(new Comment { UserName = _user.UserName, CommentText = comment, CreationDate = DateTime.Now });
            _posts.ReplaceOne(p => p.Id == post.Id, post);
        }
        public bool UnSubscribe(string username)
        {
            bool result = _user.Subscribers.Remove(username);
            if (result)
            {
                _users.ReplaceOne(u => u.Id == _user.Id, _user);
            }
            return result;
        }
        public bool GetSubscribers(string username)
        {
            return _user.Subscribers.Contains(username);
        }
        public User FindUser(string username)
        {
            var filter = Builders<User>.Filter.Eq("username", username);
            var user_s = _users.Find(filter).ToList();
            if (user_s.Count == 1)
            {
                return user_s[0];
            }
            return null;
        }
        public bool IsSubscribers(User user)
        {
            return _user.Subscribers.Contains(user.UserName);
        }
        public void Subscribe(string username)
        {
            _user.Subscribers.Add(username);
            _users.ReplaceOne(u => u.Id == _user.Id, _user);
        }
        public void CreateUser(string userName, string firstName, string lastName, string password, List<string> subscribers)
        {
            var newUser = new User
            {
                UserName = userName,
                FirstName = firstName,
                Surname = lastName,
                Password = password,
                Subscribers = subscribers
            };

            _users.InsertOne(newUser);
        }

        public void DeleteUser(string userName)
        {
            _users.DeleteOne(p => p.UserName == userName);
        }
    }
}
