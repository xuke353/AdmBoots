using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Infrastructure.Ioc {

    public class IocManager {

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IIocFactory SetProvider(IIocFactory provider) {
            if (Current == null) {
                Current = provider;
            }
            return Current;
        }

        public static IIocFactory Current { get; private set; }
    }
}
