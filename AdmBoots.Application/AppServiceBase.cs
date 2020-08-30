using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Data.EntityFrameworkCore.Uow;
using AdmBoots.Infrastructure.Framework.Web;
using AutoMapper;

namespace AdmBoots.Application {

    [UnitOfWork]
    public class AppServiceBase {
        public IAdmUnitOfWork CurrentUnitOfWork { get; set; }

        public IMapper ObjectMapper { get; set; }

        public IAdmSession AdmSession { get; set; }
    }
}
