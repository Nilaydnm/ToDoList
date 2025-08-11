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
        Task<User> GetByUsernameAsync(string username, bool isDeleted = false);
    }
}
