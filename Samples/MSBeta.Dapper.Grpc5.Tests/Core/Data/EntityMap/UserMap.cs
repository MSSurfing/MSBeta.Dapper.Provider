using DapperExtensions.Mapper;
using MSBeta.Dapper.Provider.Tests.Data.Entity;

namespace MSBeta.Dapper.Provider.Tests.Data.EntityMap
{
    public class UserMap : ClassMapper<User>
    {
        public UserMap()
        {
            Table("User");
            Map(x => x.Id).Key(KeyType.Guid);
            AutoMap();
        }
    }
}
