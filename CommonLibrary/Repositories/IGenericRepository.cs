namespace CommonLibrary.Repositories
{
    public interface IGenericRepository<T,D> where T : class
    {
        Task<T> AddAsync(T item);
        ICollection<T> Find(Func<T, bool> predicate);
        Task<ICollection<T>> GetAllAsync();
        Task<T> GetById(object id);
        T Update(T item);
        void Delete(T item);
        Task SaveChangesAsync();
    }
}