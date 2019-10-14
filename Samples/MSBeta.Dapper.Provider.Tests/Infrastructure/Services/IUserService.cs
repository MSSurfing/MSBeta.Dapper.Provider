using MSBeta.Dapper.Provider.Tests.Data.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSBeta.Dapper.Provider.Tests.Services
{
    public interface IUserService : IDisposable
    {
        IList<User> Search(string name = null, int pageIndex = 0, int pageSize = 10);
        User GetUser(Guid Id);
        void Insert(User entity);

        Task<IList<User>> SearchAsync(string name = null, int pageIndex = 0, int pageSize = 10);
    }
}
