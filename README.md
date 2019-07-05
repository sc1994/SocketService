
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

安装依赖

```bash
dotnet restore
cd wwwroot
npm i
cd ../
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
![image](https://raw.githubusercontent.com/sc1994/SocketService/master/Static/1562308000(1).jpg)
这里面的`2019-07-05T06:25:50.162Z` 是当前连接标识（偷懒直接用时间格式了...）  
访问`http://localhost:5555/swagger/index.html`
![image](https://raw.githubusercontent.com/sc1994/SocketService/master/Static/1562308172(1).jpg)
发送消息需要带上

```json
{
  "groupId": "string", // 发送给谁，如果有多客户端使用了同一个连接标识，那么这个消息将会是一个多播消息
  "method": "string", // 消息的方法名称（前端设置的监听方法名）具体可在代码中全文搜索此代码 this.connection.on("test", msg => {
  "message": "string" // 消息文本
}
```

编辑请求参数，给某个客户端发条消息试一下

```json
{
    "groupId": "2019-07-05T06:25:50.162Z",
    "method": "test",
    "message": "Hello"
}
```

点击Execute，顺利的话。在`http://localhost:5555`页面上就会打印出这条消息了。

## 日志

日志文件在项目根目录，logs文件夹下。 日志中的`2019-07-05T05:48:26.138Z`为连接标识（偷个懒）

```log
2019-07-05 13:48:26.357 +08:00 [INF] OnConnectedAsync(["2019-07-05T05:48:26.138Z"]) // 连接日志
2019-07-05 13:48:26.504 +08:00 [INF] CallBackController-Connected(true,2019-07-05T05:48:26.138Z) // 连接回调的测试日志
2019-07-05 13:48:48.661 +08:00 [INF] Send(2019-07-05T05:48:26.138Z,test,string) // 调用http给2019-07-05T05:48:26.138Z发送消息日志
2019-07-05 14:11:27.859 +08:00 [INF] OnDisconnectedAsync(["2019-07-05T05:48:26.138Z"]) // 离线日志
2019-07-05 14:11:27.866 +08:00 [INF] CallBackController-Disconnected(true,2019-07-05T05:48:26.138Z,null) // 离线回调测试日志
```

## 前端代码

参考 `https://github.com/sc1994/SocketService/blob/master/wwwroot/index.html`
