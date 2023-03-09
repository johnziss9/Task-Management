using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        List<Task> Tasks { get; set; }
    }
}