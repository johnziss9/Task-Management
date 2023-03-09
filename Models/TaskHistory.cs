namespace Task_Management.Models
{
    public class TaskHistory
    {
        public User User { get; set; }
        public DateTime DateModified { get; set; }
        public string Description { get; set; }
    }
}