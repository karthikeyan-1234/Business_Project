namespace Services
{
    public interface IInfoHub
    {
        Task SendMessage(string mesage);
    }
}