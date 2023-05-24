using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class TaskHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("user")]
        public User? User { get; set; }

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("taskId")]
        public string? TaskId { get; set; }
    }
}