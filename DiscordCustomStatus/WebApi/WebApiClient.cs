using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace DiscordCustomStatus.WebApi
{
    public static class WebApiClient
    {
        private const string ApiToken = "discord-custom-status";

        public static WebApplication StartWebApi()
        {
            var builder = WebApplication.CreateBuilder();
            var port = GetFreePort();
            builder.WebHost.ConfigureKestrel(o =>
            {
                o.ListenLocalhost(port);
            });

            builder.Services.AddSingleton<WebApiConfig>(new WebApiConfig
            {
                Port = port
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path.StartsWithSegments("/swagger"))
                {
                    await next();
                    return;
                }

                if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var token) ||
                    token != ApiToken)
                {
                    ctx.Response.StatusCode = 401;
                    await ctx.Response.WriteAsync("Unauthorized");
                    return;
                }

                await next();
            });

            Task.Run(() => app.Run());

            return app;
        }

        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 5000);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
