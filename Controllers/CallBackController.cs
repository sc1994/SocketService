using Microsoft.AspNetCore.Mvc;
using Serilog;
using SocketService.Models;

namespace SocketService.Controllers
{
    /// <summary>
    /// 默认客户端的代码
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CallBackController : ControllerBase
    {
        /// <summary>
        /// 监控上线
        /// </summary>
        /// <param name="info"></param>
        [HttpPost("connected")]
        public void Connected(CallBackInfo info)
        {
            Log.Information("CallBackController-Connected({status},{message})", info.Status, info.Message);
        }

        /// <summary>
        /// 监控离线
        /// </summary>
        /// <param name="info"></param>
        [HttpPost("disconnected")]
        public void Disconnected(CallBackInfo info)
        {
            Log.Information("CallBackController-Disconnected({status},{message},{exception})", info.Status, info.Message, info.Exception);
        }
    }
}