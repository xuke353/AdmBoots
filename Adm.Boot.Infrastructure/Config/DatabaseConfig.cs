using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adm.Boot.Infrastructure.Config
{
    public class DatabaseConfig
    {
        public static string ConnectionString => InitConn();
        public static DatabaseType DatabaseType = DatabaseType.SqlServer;


        private static string InitConn()
        {
            var dbTypeStr = AdmApp.Configuration["Database:DbProviderName"];
            var connectString = AdmApp.Configuration["Database:ConnectionString"];
            switch (dbTypeStr)
            {
                case "MySql":
                    DatabaseType = DatabaseType.MySql;
                    break;
                case "SqlServer":
                    DatabaseType = DatabaseType.SqlServer;
                    break;
                case "Oracle":
                    DatabaseType = DatabaseType.Oracle;
                    break;
                case "Sqlite":
                    DatabaseType = DatabaseType.Sqlite;
                    connectString = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, connectString);
                    break;
            }
            return connectString;
        }
    }

    public enum DatabaseType
    {
        MySql,
        SqlServer,
        Sqlite,
        Oracle,
        PostgreSQL
    }
}
