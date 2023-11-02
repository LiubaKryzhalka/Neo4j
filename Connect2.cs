using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo4j
{
    public class Connect2
    {
        static GraphClient Client
        {
            get
            {
                GraphClient client = new GraphClient(new Uri("http://localhost:7687/"), "neo4j", "3026network");
                client.ConnectAsync().Wait();
                return client;
            }
        }
        private User currentUser;
        public void Authtentificate(string username, string pass)
        {
            var user = Client.Cypher
                .Match("(u:User { username: $un})")
                .WithParam("un", username)
                .Where("u.password= $pass")
                .WithParam("pass", pass)
                .Return(u => u.As<User>())
                .ResultsAsync.Result;
            currentUser = user.ElementAt(0);
        }
        public void CreateUser(string userName, string firstName, string surname, string password)
        {
            var newUser = new User
                (
                userName,
                firstName,
                surname,
                password
                );
            Client.Cypher
                .Create("(u:User $newUser)")
                .WithParam("newUser", newUser)
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void CreateRelationshipUserSubscribers(string SubscribersName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})", "(f:User{username: $fn})")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", SubscribersName)
                .Create("(f)-[:Subscribers]->(u)")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public void DeleteRelationshipUserSubscribers(string SubscribersName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})-[r:Subscribers]->(f:User{username: $fn})")
                .WithParam("fn", currentUser.UserName)
                .WithParam("un", SubscribersName)
                .Delete("r")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public void DeleteUser(string userName)
        {
            DeleteAllRelationshipWithUser(userName);
            Client.Cypher
                .Match("(u:User {username: $deleteUser})")
                .WithParam("deleteUser", userName)
                .Delete("u")
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void DeleteAllRelationshipWithUser(string userName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})-[r]-(f:User)")
                .WithParam("un", currentUser.UserName)
                .Delete("r")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public IEnumerable<Object> SearchRelationshipOfUser(string searchedUser)
        {
            var userWithSubscribers = Client.Cypher
                .Match("(u:User {username: $un})-[r]-> (f: User {username: $fn})")
                .WithParam("fn", currentUser.UserName)
                .WithParam("un", searchedUser)
                .Return((u, f) => new
                {
                    User = f.As<User>(),
                    Subscribers = u.As<User>()
                })
                .ResultsAsync.Result;
            return userWithSubscribers;
        }
        public double ShortestPathToSearchedUser(string searchedUserName)
        {
            var userWithSubscribed = Client.Cypher
                .Match("sp = shortestPath((:User {username: $un})-[*]-(:User {username: $fn}))")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", searchedUserName)
                .Return(sp => sp.Length())
                .ResultsAsync.Result;
            return userWithSubscribed.First();
        }
    }
}
