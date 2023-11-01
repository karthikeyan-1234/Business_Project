using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Repositories
{
    public class GenericRepository<T, D> : IGenericRepository<T,D> where T : class where D : DbContext
    {
        D? db;
        DbSet<T>? table;

        public GenericRepository(D db)
        {
            this.db = db;
            this.table = db.Set<T>();
        }

        public async Task<T> AddAsync(T item)
        {
            var obj = await table.AddAsync(item);
            return obj.Entity;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }

        public T Update(T item)
        {
            db.Entry(item).State = EntityState.Modified;
            return item;
        }

        public void Delete(T item) 
        {
            db.Entry(item).State = EntityState.Deleted;
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await table.FindAsync(id);
        }

        public ICollection<T> Find(Func<T, bool> predicate)
        {
            return table.Where(predicate).ToList();
        }
    }
}
