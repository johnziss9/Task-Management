namespace Task_Management.Services.MessagingService
{
    public interface IMessageProducer
    {
        public void SendMessage<T>(T message);
    }
}