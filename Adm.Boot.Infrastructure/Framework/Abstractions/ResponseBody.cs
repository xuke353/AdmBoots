using System;
using System.Collections.Generic;
using System.Text;

namespace Adm.Boot.Infrastructure.Framework.Abstractions {
    /// <summary>
    /// 约定的 HTTP Response 结构
    /// </summary>
    /// <typeparam name="TPayload">数据类型</typeparam>
    public class ResponseBody<TPayload> {

        /// <summary>
        /// 数据
        /// </summary>
        public TPayload Payload { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; }

        public ResponseBody(TPayload payload, string message = null) {
            Payload = payload;
            Message = message;
        }

        public ResponseBody(string message) {
            Message = message;
        }
    }

    /// <summary>
    /// 快速创建 <see cref="ResponseBody{TPayload}"/> 实例
    /// </summary>
    public static class ResponseBody {

        public static ResponseBody<T> From<T>(T payload, string message = null) {
            return new ResponseBody<T>(payload, message);
        }

        public static ResponseBody<object> From(string message) {
            return new ResponseBody<object>(message);
        }
    }
}
