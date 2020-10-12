using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace AdmBoots.Infrastructure.SignalR {
    /// <summary>
    /// 集线器
    /// </summary>

    public class ChatHub : Hub {
        private readonly IDistributedCache _cache;

        public ChatHub(IDistributedCache cache) {
            _cache = cache;
        }

        /// <summary>
        /// 给制定用户发消息
        /// 在需要发消息的地方，在cache根据userId获取connectionId
        /// var connectionIdList = _cache.GetObject<List<string>>(userId);
        /// _hubContext.Clients.Clients(connectionIdList).SendAsync("recieve-message", 发送的对象).Wait();
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync() {
            //判断用户登陆过期
            if (Context.User.Identity.IsAuthenticated) {
                var userName = Context.User.Identity.Name;
                var userId = Context.UserIdentifier;
                var connectionId = Context.ConnectionId;
                var connectionIdList = _cache.GetObject<List<string>>(userId);
                if (connectionIdList == null) {
                    _cache.SetObject(userId, new List<string> { connectionId });
                } else {
                    //可能会存在同一账号多窗口登陆得情况
                    if (!connectionIdList.Contains(connectionId)) {
                        connectionIdList.Add(connectionId);
                        _cache.SetObject(userId, connectionIdList);
                    }
                }

                string.Format("用户: {0},上线  ConnectionId: {1}", userName, Context.ConnectionId).WriteSuccessLine();
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (Context.User.Identity.IsAuthenticated) {
                var userId = Context.UserIdentifier;
                var connectionId = Context.ConnectionId;
                var connectionIdList = _cache.GetObject<List<string>>(userId);
                if (connectionIdList != null) {
                    if (connectionIdList.Contains(connectionId)) {
                        connectionIdList.Remove(connectionId);
                        _cache.SetObject(userId, connectionIdList);
                    }
                }
                string.Format("用户: {0},断开  ConnectionId: {1}", Context.User.Identity.Name, Context.ConnectionId).WriteSuccessLine();
            }
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 使用事例：
        /// 1.客户端调用服务端方法
        /// connection.invoke("SendMessage", "消息")
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string message) {
            //2.客户端接收服务端消息
            //connection.on("RecieveMessage", data => $('#msglist').append('<li>').text(data))
            await Clients.All.SendAsync("RecieveMessage", $"{Context.ConnectionId}: {message}");
        }
    }
}
