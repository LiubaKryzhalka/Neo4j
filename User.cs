using Newtonsoft.Json;

namespace Neo4j
{
    public class User
    {
        public User(string username, string first_name, string surname, string password)
        {
            UserName = username;
            FirstName = first_name;
            Surname = surname;
            Password = password;
        }
        public User() { }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "surname")]
        public string Surname { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {Surname} - username: {UserName}";
        }
    }
}
