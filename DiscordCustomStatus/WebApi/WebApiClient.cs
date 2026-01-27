using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Net;

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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            Task.Run(() =>
            {
                app.Run($"http://localhost:{port}");
            });

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
