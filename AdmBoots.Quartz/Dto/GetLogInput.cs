using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Quartz.Dto {

    public class GetLogInput : PageRequest {
        public string JobName { get; set; }
        public string GroupName { get; set; }
    }
}
