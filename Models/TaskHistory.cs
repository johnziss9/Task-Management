using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class TaskHistory
    {
        [BsonElement("user")]
        public User? User { get; set; }

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }
    }
}