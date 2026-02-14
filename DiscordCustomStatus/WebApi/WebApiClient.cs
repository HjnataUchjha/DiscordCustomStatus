using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace DiscordCustomStatus.WebApi
{
    public static class WebApiClient
    {
        public static WebApplication StartWebApi()
        {
            var builder = WebApplication.CreateBuilder();

            var port = GetFreePort();
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

            LastPort = port;

            Task.Run(() =>
            {
                app.Run($"http://localhost:{port}");
            });

            return app;
        }

        public static int LastPort { get; private set; }

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
