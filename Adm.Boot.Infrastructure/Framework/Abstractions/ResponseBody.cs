using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Infrastructure.Framework.Abstractions {
    /// <summary>
    /// 约定的 HTTP Response 结构
    /// </summary>
    /// <typeparam name="TData">数据类型</typeparam>
    public class ResponseBody<TData> {

        /// <summary>
        /// 数据
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; }

        public ResponseBody(TData data, bool status, string message = null) {
            Data = data;
            Message = message;
            Status = status;
        }

        public ResponseBody(bool status, string message) {
            Message = message;
            Status = status;
        }
    }

    /// <summary>
    /// 快速创建 <see cref="ResponseBody{TData}"/> 实例
    /// </summary>
    public static class ResponseBody {

        public static ResponseBody<T> From<T>(T payload, string message = null) {
            return new ResponseBody<T>(payload, true, message);
        }

        public static ResponseBody<object> From(string message) {
            return new ResponseBody<object>(true, message);
        }

        public static ResponseBody<T> Bad<T>(T payload, string message = null) {
            return new ResponseBody<T>(payload, false, message);
        }

        public static ResponseBody<object> Bad(string message) {
            return new ResponseBody<object>(false, message);
        }
    }
}
