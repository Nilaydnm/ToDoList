using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IToDoRepository : IGenericRepository<ToDo>
    {
        // Kullanıcı ID'sine göre ToDo listelerini getir
        // bu  da diğer işlemlerden farklı olduğundan ayrı bir metot olarak tanımladık
        Task<List<ToDo>> GetToDosByUserIdAsync(int userId);
        Task UpdateAsync(ToDo todo);
        Task DeleteAsync(ToDo todo);
        Task<List<ToDo>> GetByUserIdAsync(int userId);
        Task<bool> GroupExistsAsync(int groupId);
        Task UpdateWithoutValidationAsync(ToDo todo);
        Task SaveChangesAsync();
    }
}
