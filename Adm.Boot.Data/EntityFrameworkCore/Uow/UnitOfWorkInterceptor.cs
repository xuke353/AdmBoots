using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Adm.Boot.Infrastructure.Helper;
using Castle.DynamicProxy;

namespace Adm.Boot.Data.EntityFrameworkCore.Uow {

    public class UnitOfWorkInterceptor : IInterceptor {
        private readonly IAdmUnitOfWork _admUnitOfWork;

        public UnitOfWorkInterceptor(IAdmUnitOfWork admUnitOfWork) {
            _admUnitOfWork = admUnitOfWork;
        }

        public void Intercept(IInvocation invocation) {
            MethodInfo method;
            try {
                method = invocation.MethodInvocationTarget;
            } catch {
                method = invocation.GetConcreteMethod();
            }

            var unitOfWorkAttr = GetUnitOfWorkAttributeOrNull(method);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled) {
                //No need to a uow
                invocation.Proceed();
                return;
            }

            //No current uow, run a new one
            PerformUow(invocation);
        }

        private void PerformUow(IInvocation invocation) {
            if (invocation.Method.IsAsync()) {
                PerformAsyncUow(invocation);
            } else {
                PerformSyncUow(invocation);
            }
        }

        private void PerformSyncUow(IInvocation invocation) {
            Console.WriteLine("同步方法事务开启");
            using (var uow = _admUnitOfWork.Begin()) {
                invocation.Proceed();
                _admUnitOfWork.Complete(uow);
            }
        }

        private void PerformAsyncUow(IInvocation invocation) {
            Console.WriteLine("异步方法事务开启");
            var uow = _admUnitOfWork.Begin();

            try {
                invocation.Proceed();
            } catch {
                uow.Dispose();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task)) {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                    (Task)invocation.ReturnValue,
                    async () => await _admUnitOfWork.CompleteAsync(uow),
                    exception => uow.Dispose()
                );
            } else //Task<TResult>
              {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await _admUnitOfWork.CompleteAsync(uow),
                    exception => uow.Dispose()
                );
            }
        }

        private UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo) {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }

            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }
            return null;
        }
    }
}
