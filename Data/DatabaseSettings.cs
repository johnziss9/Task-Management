namespace Task_Management.Data
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string TasksCollectionName { get; set; }
    }
}