
# SocketService

一个Socket中间件，使用http主动发送消息给前端。

## Feature

- 基本上无痛接入的你的现有后台代码。
- 不用理会后台语言，只要你的语言可以优雅的发送http请求即可
- 足够简单
- 前端的上线和离线可被监控

## 使用

检查运行环境，检查输出内容是否包含版本`2.2.300`。

```bash
dotnet --list-sdks
```

下载源码

```bash
git clone https://github.com/sc1994/SocketService.git
cd SocketService
```

修改配置文件`appsettings.json`/`appsettings.Development.json`

```json
{
    ...
    "ConnectionStrings": {
        "Redis": "localhost:6379,password=1qaz2wsx3edc", // redis 链接配置
        "AllowOrigins": "*" // 跨域白名单
    },
    "EventCallBack": {
        "OnConnectedAsync": "http://localhost:5555/api/callback/connected", // 连接事件回调
        "OnDisconnectedAsync": "http://localhost:5555/api/callback/disconnected" // 断开事件回调
    }
    ...
}
```

运行

```bash
dotnet run 
```

## 测试
访问`http://localhost:5555`
![image]()
