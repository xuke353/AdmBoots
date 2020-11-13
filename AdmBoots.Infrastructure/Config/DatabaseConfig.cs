using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AdmBoots.Infrastructure.Config {

    public class DatabaseConfig {
        public static string ConnectionString => InitConn();
        public static DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
        public static string Version { get; set; } = "0.0.0";

        private static string InitConn() {
            var dbTypeStr = AdmBootsApp.Configuration["Database:Dialect"];
            var connectString = AdmBootsApp.Configuration.GetConnectionString("Default");
            switch (dbTypeStr) {
                case "MySql":
                    DatabaseType = DatabaseType.MySql;
                    Version = AdmBootsApp.Configuration["Database:Version"];
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

    public enum DatabaseType {
        MySql,
        SqlServer,
        Sqlite,
        Oracle,
        PostgreSQL
    }
}
