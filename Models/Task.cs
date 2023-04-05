using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class Task
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }

        [BsonElement("dateModified")]
        public DateTime DateModified { get; set; }

        [BsonElement("priority")]
        public int Priority { get; set; }

        [BsonElement("attachments")]
        public List<byte[]>? Attachments { get; set; }

        [BsonElement("tags")]
        public List<string>? Tags { get; set; }

        [BsonElement("status")]
        public bool Status { get; set; }

        [BsonElement("history")]
        public List<TaskHistory>? History { get; set; }

        [BsonElement("dependencies")]
        public List<ObjectId>? Dependencies { get; set; }
    }
}