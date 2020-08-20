using System;
using System.Collections.Generic;
using System.Text;
using Adm.Boot.Data.EntityFrameworkCore.Uow;
using AutoMapper;

namespace Adm.Boot.Application {

    [UnitOfWork]
    public class AppServiceBase {
        public IAdmUnitOfWork CurrentUnitOfWork { get; set; }

        public IMapper ObjectMapper { get; set; }
    }
}
