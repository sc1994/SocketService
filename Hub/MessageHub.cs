using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SocketService.Hub
{
    public interface IMessageHub
    {

    }

    public class MessageHub : Hub<IMessageHub>
    {
        private readonly IHttpContextAccessor _http; // http上下文
        private readonly IConfiguration _config; // 配置

        public MessageHub(IHttpContextAccessor http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        /// <summary>
        /// 上线处理，触发回调事件
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var status = true;
            string message = null;
            try
            {
                if (_http.HttpContext.Request.Query.TryGetValue("groupid", out var groupId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                    message = groupId;
                    Log.Information("OnConnectedAsync({groupId})", groupId);
                }
                else
                {
                    status = false;
                    message = "创建连接时未传入groupId";
                }
            }
            catch (Exception ex) // 记录异常
            {
                status = false;
                message = ex.ToString();
            }
            finally
            {
                var url = _config.GetSection("EventCallBack")?["OnConnected"];
                if (url != null)
                {
                    await url.PostJsonAsync(new { status, message }); // 消息回发
                }
            }
        }

        /// <summary>
        /// 离线处理，触发回调事件
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var status = true;
            string message = null;
            try
            {
                if (_http.HttpContext.Request.Query.TryGetValue("groupid", out var groupId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
                    message = groupId;
                    Log.Information("OnDisconnectedAsync({groupId})", groupId);
                }
                else
                {
                    status = false;
                    message = "断开连接时未传入groupId";
                }
            }
            catch (Exception ex) // 记录异常
            {
                status = false;
                message = ex.ToString();
            }
            finally
            {
                var url = _config.GetSection("EventCallBack")?["OnDisconnected"];
                if (url != null)
                {
                    await url.PostJsonAsync(new { status, message, exception = exception?.ToString() }); // 消息回发
                }
            }
        }
    }
}