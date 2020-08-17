using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Data.EntityFrameworkCore
{
    public class AdmDbContext : DbContext
    {
        public AdmDbContext(DbContextOptions<AdmDbContext> options)
         : base(options)
        {
        }
    }
}
