using Adm.Boot.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Data.EntityFrameworkCore {
    public class AdmDbContext : DbContext {
        public virtual DbSet<User> Users { get; set; }
        public AdmDbContext(DbContextOptions<AdmDbContext> options)
         : base(options) {
        }
    }
}
