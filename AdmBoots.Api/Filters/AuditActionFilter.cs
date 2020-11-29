using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdmBoots.Api.Filters {
    public class AuditActionFilter : IAsyncActionFilter {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            throw new NotImplementedException();
        }
    }
}
