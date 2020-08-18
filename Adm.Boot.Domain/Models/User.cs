using Adm.Boot.Infrastructure.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Adm.Boot.Domain.Models
{
    [Table("User")]
    public class User : Entity<int>
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool IsMaster { get; set; }

        public int UnreadCount { get; set; }

        public string Email { get; set; }
    }
}
