namespace Task_Management.Data
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string TasksCollectionName { get; set; }
    }
}