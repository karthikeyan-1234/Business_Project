namespace CommonLibrary.Caching
{
    public interface ICacheManager
    {
        Task<T?> TryGetAsync<T>(string key);
        Task<bool> TrySetAsync<T>(T obj, string key);
        void SetTimeOut(TimeSpan timeOut);
    }
}