using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Ioc {

    public interface IIocFactory {

        T Resolve<T>();

        T Resolve<T>(Type type);

        bool IsRegistered(Type type);

        bool IsRegistered<TType>();

        T ResolveScope<T>();

        T ResolveScope<T>(Type type);
    }
}
