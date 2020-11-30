using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Infrastructure.Ioc {

    public class IocFactory : IIocFactory {
        private readonly IServiceProvider _provider;

        public IocFactory(IServiceProvider provider) {
            _provider = provider;
        }

        public T Resolve<T>() {
            return _provider.GetService<T>();
        }

        public T Resolve<T>(Type type) {
            return (T)_provider.GetService(type);
        }

        public bool IsRegistered(Type type) {
            return _provider.GetServices(type).Any();
        }

        public bool IsRegistered<TType>() {
            return _provider.GetServices<TType>().Any();
        }

        public T ResolveScope<T>() {
            return _provider.CreateScope().ServiceProvider.GetService<T>();
        }

        public T ResolveScope<T>(Type type) {
            return (T)_provider.CreateScope().ServiceProvider.GetService(type);
        }
    }
}
