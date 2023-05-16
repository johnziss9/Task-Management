using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; } = new byte[0];

        [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; } = new byte[0];
    }
}