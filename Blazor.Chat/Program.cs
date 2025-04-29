using Blazor.Chat.Components;
using Blazor.Chat.Services;
using Blazor.Chat.Utility;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.ResponseCompression;

namespace Blazor.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            // Add services to the container.

            Func<IServiceProvider, IFreeSql> fsqlFactory = r =>
            {
                IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.Sqlite, @"Data Source=MiaoMiaoJiang.db")
                    .UseAdoConnectionPool(true)
                    .UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}"))
                    .UseAutoSyncStructure(true) //自动同步实体结构到数据库，只有CRUD时才会生成表
                    .Build();
                return fsql;
            };
            builder.Services.AddSingleton(fsqlFactory);

            builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);
            builder.Services.AddAuthorizationCore();    
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthenticationStateDeserialization();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/html" });
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });


            builder.Services.AddScoped<MiaoMiaoJiangAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(service => service.GetRequiredService<MiaoMiaoJiangAuthenticationStateProvider>());
            builder.Services.AddScoped<IMiaoMiaoJiangAuthenticationStateProvider>(service => service.GetRequiredService<MiaoMiaoJiangAuthenticationStateProvider>());
            builder.Services.AddScoped<IChatService, ChatService>();
            // 添加 HttpClient 工厂
            builder.Services.AddHttpClient("OllamaClient", client =>
            {
                client.Timeout = TimeSpan.FromHours(24);
            });

            builder.Services.AddScoped<ISettingsService, SettingsService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseAntiforgery();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,  // 允许服务未知文件类型
                OnPrepareResponse = ctx =>
                {
                    if (ctx.File.Name.EndsWith(".mp3"))
                    {
                        ctx.Context.Response.Headers.Append("Accept-Ranges", "bytes");
                        ctx.Context.Response.Headers.Append("Content-Type", "audio/mpeg");
                    }
                    ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                    ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                    ctx.Context.Response.Headers.Append("Expires", "0");
                }
            });


            app.MapStaticAssets();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
            app.MapGet("/api/audio", () =>
            {
                var filePath = Path.Combine(app.Environment.WebRootPath, "audios", "1.mp3");
                if (!System.IO.File.Exists(filePath))
                {
                    return Results.NotFound();
                }
                return Results.File(filePath, "audio/mpeg", "1.mp3", enableRangeProcessing: true);
            });

            // 允许所有来源的跨域请求（CORS），以便前端能够访问 API
            app.UseCors("AllowAll");
            app.Run();
        }
    }
}
