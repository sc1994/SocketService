namespace SocketService.Models
{
    public class CallBackInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        /// <value></value>
        public bool Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        /// <value></value>
        public string Message { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        /// <value></value>
        public string Exception { get; set; }
    }
}