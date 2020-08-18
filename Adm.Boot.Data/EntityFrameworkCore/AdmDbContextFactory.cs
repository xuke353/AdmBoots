
using Adm.Boot.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Data.EntityFrameworkCore {
    /* This class is needed to run EF Core PMC commands. Not used anywhere else */

    public class AdmDbContextFactory //: IDesignTimeDbContextFactory<AdmDbContext>
        {

        //public AdmDbContext CreateDbContext(string[] args)
        //{
        //    var builder = new DbContextOptionsBuilder<AdmDbContext>();
        //    if (DatabaseConfig.DatabaseType == DatabaseType.MySql)
        //        builder.UseMySql(DatabaseConfig.ConnectionString);

        //    //if (DatabaseConfig.DatabaseType == DatabaseType.SqlServer)
        //    //    builder.UseSqlServer(DatabaseConfig.ConnectionString);

        //    return new AdmDbContext(builder.Options);
        //}
    }
}
