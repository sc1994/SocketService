namespace SocketService.Models
{
    /// <summary>
    /// send info
    /// </summary>
    public class SendInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}