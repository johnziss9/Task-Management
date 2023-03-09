using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Task_Management.Models
{
    public class Task
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int Priority { get; set; }

        public List<byte[]> Attachments { get; set; }

        public List<string> Tags { get; set; }

        public bool Status { get; set; }

        public List<TaskHistory> History { get; set; }

        public List<ObjectId> Dependencies { get; set; }
    }
}