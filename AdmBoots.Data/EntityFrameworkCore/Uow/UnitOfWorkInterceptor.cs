using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Extensions;
using AdmBoots.Infrastructure.Helper;
using Castle.DynamicProxy;

namespace AdmBoots.Data.EntityFrameworkCore.Uow {

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

            var unitOfWorkAttr = method.GetAttribute<UnitOfWorkAttribute>();
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

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="invocation"></param>
        private void PerformSyncUow(IInvocation invocation) {
            using (var uow = _admUnitOfWork.Begin()) {
                invocation.Proceed();
                _admUnitOfWork.Complete(uow);
            }
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="invocation"></param>
        private void PerformAsyncUow(IInvocation invocation) {
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
    }
}
