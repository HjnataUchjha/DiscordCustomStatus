using DiscordCustomStatus.Tray;
using DiscordCustomStatus.WebApi;
using DiscordRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Application = System.Windows.Application;

namespace DiscordCustomStatus
{
    public partial class App : Application
    {
        private NotifyIcon _trayIcon;
        private WebApplication _webApp;
        private DiscordRpcClient _client;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _webApp = WebApiClient.StartWebApi();
            var port = _webApp.Services.GetRequiredService<WebApiConfig>().Port;

            _trayIcon = TrayIconClient.StartTray();
            _trayIcon.ContextMenuStrip.AddCurrentConfigLabel();
            _trayIcon.ContextMenuStrip.RunDiscordRpcClient(_client);
            _trayIcon.ContextMenuStrip.AddSettingsButton(port);
            _trayIcon.ContextMenuStrip.AddShutdownBotton(() => Shutdown());
            //_trayIcon.ContextMenuStrip.AddSwaggerBotton(port);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _webApp?.DisposeAsync();
        }
    }
}
