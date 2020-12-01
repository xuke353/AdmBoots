using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Infrastructure.Ioc {

    public interface IIocFactory {

        T Resolve<T>();

        T Resolve<T>(Type type);

        bool IsRegistered(Type type);

        bool IsRegistered<T>();

        T ResolveFromScope<T>(IServiceScope scope);

        T ResolveFromScope<T>(Type type, IServiceScope scope);

        IServiceScope CreateScope();
    }
}
