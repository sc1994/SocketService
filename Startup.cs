using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SocketService.Hub;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace SocketService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json", true, true) // 启用 不同环境的配置文件
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build(); // 构建自定义的配置

            Log.Logger = new LoggerConfiguration() // 注册日志组件
                            .WriteTo
                            .File("logs/.log", rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>(); // 注入http请求的上下文

            services.AddSignalR(options =>
            {
                options.HandshakeTimeout = TimeSpan.FromSeconds(30); // 握手30s超时
                options.EnableDetailedErrors = true;
            })
            .AddRedis(options =>
            {
                options.ConnectionFactory = async factory =>
                {
                    var connection = await ConnectionMultiplexer.ConnectAsync(Configuration.GetConnectionString("Redis"));
                    connection.InternalError += (sender, e) =>
                    {
                        Log.Error("RedisInternalError({sender},{e})", sender, e);
                    };
                    connection.ErrorMessage += (sender, e) =>
                    {
                        Log.Error("RedisErrorMessage({sender},{e})", sender, e);
                    };
                    return connection;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UsePathBase("/api"); // api使用后缀

            if (env.IsDevelopment()) // 开发环境
            {
                app.UseDeveloperExceptionPage(); // 启用错误详情页

                app.UseSwagger(); // 启用swagger
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "API_V1");
                });
            }

            app.UseCors(x => // 跨域配置
                        x.AllowAnyMethod()
                         .AllowAnyHeader()
                         .WithOrigins(Configuration.GetConnectionString("AllowOrigins"))
                         .AllowCredentials());

            // app.UseHttpsRedirection(); 需要https时
            app.UseMvc();

            app.UseDefaultFiles(); // 测试文件
            app.UseStaticFiles();

            app.UseSignalR(builder =>
                {
                    builder.MapHub<MessageHub>("/message"); // 绑定socket 二级message
                });
        }
    }
}
