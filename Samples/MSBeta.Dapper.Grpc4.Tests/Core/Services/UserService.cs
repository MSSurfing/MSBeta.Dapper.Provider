using DapperExtensions;
using MSBeta.Dapper.Provider.Tests.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MSBeta.Dapper.Provider.Tests.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }

        protected virtual IRepository<User> GetRepository()
        {
            var connectionString = "Data Source=139.159.137.99;Initial Catalog=Surfing.Test109;Persist Security Info=True;User ID=sa2;Password=z;Connect Timeout=360";
            var connection = new SqlConnection(connectionString);

            return new DapperRepository<User>(connection);
        }

        public IList<User> Search(string name = null, int pageIndex = 0, int pageSize = 10)
        {
            using (var _userRepository = GetRepository())
            {
                var query = _userRepository.FilterAnd;

                if (!string.IsNullOrEmpty(name))
                    query.Where(e => e.Name, Operator.Eq, name);

                var sort = query.Sort(e => e.Name);

                return _userRepository.GetPaged(query, sort, pageIndex, pageSize).ToList();
            }
        }

        public void Dispose()
        {

        }
    }
}
