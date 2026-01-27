using DiscordCustomStatus.Tray;
using DiscordCustomStatus.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Application = System.Windows.Application;

namespace DiscordCustomStatus
{
    public partial class App : Application
    {
        private NotifyIcon _trayIcon;
        private WebApplication _webApp;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _webApp = WebApiClient.StartWebApi();

            _trayIcon = TrayIconClient.StartTray();
            _trayIcon.ContextMenuStrip.AddShutdownBotton(() => Shutdown());
            if (_webApp.Environment.IsDevelopment())
            {
                _trayIcon.ContextMenuStrip.AddSwaggerBotton();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _webApp?.DisposeAsync();
        }
    }
}
