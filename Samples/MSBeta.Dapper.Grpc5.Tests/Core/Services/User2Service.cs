using Autofac.Engine;
using DapperExtensions;
using MSBeta.Dapper.Provider.Tests.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSBeta.Dapper.Provider.Tests.Services
{
    public class User2Service : IUserService
    {
        public IList<User> Search(string name = null, int pageIndex = 0, int pageSize = 10)
        {
            using (var _userRepository = EngineContext.Resolve<IRepository<User>>())
            {
                var query = _userRepository.FilterAnd;

                if (!string.IsNullOrEmpty(name))
                    query.Where(e => e.Name, Operator.Eq, name);

                var sort = query.Sort(e => e.Name);

                return _userRepository.GetPaged(query, sort, pageIndex, pageSize).ToList();
            }
        }

        public User GetUser(Guid Id)
        {
            using (var _userRepository = EngineContext.Resolve<IRepository<User>>())
                return _userRepository.GetById(Id);
        }

        public void Insert(User entity)
        {
            if (entity == null)
                throw new ArgumentException(nameof(entity));
            using (var _userRepository = EngineContext.Resolve<IRepository<User>>())
                _userRepository.Insert(entity);
        }

        public void Dispose()
        {

        }
    }
}
