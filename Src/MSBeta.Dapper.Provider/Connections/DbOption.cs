using System;
using System.Collections.Generic;
using System.Text;

namespace DapperExtensions.Connections
{
    public enum DbOption
    {
        UseSqlServer = 10,
        UseMySql = 20,

        [Obsolete("Not yet supported", true)]
        UseInMemory = 30,
        [Obsolete("Not yet supported", true)]
        UseSqlite = 50,
        [Obsolete("Not yet supported", true)]
        UseSqlCe = 60,
        [Obsolete("Not yet supported", true)]
        UsePostgreSql = 70
    }
}
