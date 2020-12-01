using System;
using System.Linq;
using AdmBoots.Infrastructure.Helper;
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

        public bool IsRegistered<T>() {
            return _provider.GetServices<T>().Any();
        }

        public T ResolveFromScope<T>(IServiceScope scope) {
            Check.NotNull(scope, "scope");
            return scope.ServiceProvider.GetService<T>();
        }

        public T ResolveFromScope<T>(Type type, IServiceScope scope) {
            Check.NotNull(scope, "scope");
            return (T)scope.ServiceProvider.GetService(type);
        }

        /// <summary>
        /// 使用：using (var serviceScope = IocManager.Current.CreateScope()) {var xx = IocManager.Current.ResolveFromScope<T>(serviceScope)}
        /// </summary>
        /// <returns></returns>
        public IServiceScope CreateScope() {
            return _provider.GetService<IServiceScopeFactory>().CreateScope();
        }
    }
}
