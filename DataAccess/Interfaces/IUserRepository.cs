using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<User>> GetByUserIdAsync(int userId,bool isDeleted = false);

        // kullanıcı adını kullanarak kullanıcıyı getir
        // bu diğer işlemlerden farklı olduğundan ayrı bir metot olarak tanımladık
        Task<User> GetByUsernameAsync(string username, bool isDeleted = false);
    }
}
