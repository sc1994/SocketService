using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SocketService.Hub;
using SocketService.Models;

namespace SocketService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext; // 集线器上下文
        public SendController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// 发送消息到组
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ToGroup(SendInfo info)
        {
            Log.Information("Send({groupId},{typeEnum},{message})", info.GroupId, info.Method, info.Message);
            try
            {
                await _hubContext.Clients.Group(info.GroupId).SendAsync(info.Method, info.Message);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
