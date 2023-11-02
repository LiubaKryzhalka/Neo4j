using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongodb
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("first_name")]
        public string FirstName { get; set; }
        [BsonElement("surname")]
        public string Surname { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("interests")]
        public List<string> Interests { get; set; }
        [BsonElement("subscribers")]
        public List<string> Subscribers { get; set; }
        public override string ToString()
        {
            return $"{FirstName} {Surname} {UserName} ";
        }
    }
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("post")]
        public string PostText { get; set; }
        [BsonElement("date")]
        public DateTime CreationDate { get; set; }
        [BsonElement("comments")]
        public List<Comment> Comments { get; set; }
        [BsonElement("likes")]
        public List<string> Likes { get; set; }
        public override string ToString()
        {
            return $"\n\nusername: { UserName}"+"\n\n\n" + PostText + "\n\n\n" + $"likes: {Likes.Count}    comments: {Comments.Count}\n\n"
                + $"date: {CreationDate.ToShortDateString()}" + "\n\n";
        }
    }
    public class Comment
    {
        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("text")]
        public string CommentText { get; set; }
        [BsonElement("date")]
        public DateTime CreationDate { get; set; }
        public override string ToString()
        {
            return $"\ndate: {CreationDate.ToShortDateString()} username: {UserName}\n\n{CommentText}";
        }
    }
}
