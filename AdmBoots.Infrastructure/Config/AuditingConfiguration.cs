using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Config {
    public class AuditingConfiguration {
        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public List<Type> IgnoredTypes { get; }

        public bool SaveReturnValues { get; set; }

        public AuditingConfiguration() {
            IsEnabled = true;
            IsEnabledForAnonymousUsers = false;
            IgnoredTypes = new List<Type>();
            SaveReturnValues = false;
        }
    }
}
