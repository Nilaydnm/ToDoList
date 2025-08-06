using DataAccess;
using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class EfGenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _table;

        public EfGenericRepository(AppDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id,bool isDeleted = false)
        {

           IQueryable<T> query = _table.Where(c => c.Id == id);

            //if (!isDeleted)
            //    query.Where(c => !c.IsDeleted);

            return query.FirstOrDefault();
        }

        public async Task<List<T>> GetAllAsync(bool isDeleted = false)
        {
            return await _table.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, bool isDeleted = false)
        {
            return await _table.Where(filter).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _table.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _table.Update(entity);
        }

        public void Remove(T entity)
        {
            entity.IsDeleted = true; // Soft delete
            _table.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
