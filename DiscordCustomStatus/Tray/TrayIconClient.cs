namespace DiscordCustomStatus.Tray
{
    public static class TrayIconClient
    {
        public static NotifyIcon StartTray()
        {
            var trayIcon = new NotifyIcon
            {
                Icon = new Icon("Imgs/Icon.ico"),
                Visible = true,
                Text = "Discord custom status"
            };

            var menu = new ContextMenuStrip();
            trayIcon.ContextMenuStrip = menu;

            return trayIcon;
        }

        public static void AddShutdownBotton(this ContextMenuStrip menu, Action shutdown)
        {
            menu.Items.Add("Выход", null, (_, _) =>
            {
                shutdown.Invoke();
            });
        }

        public static void AddSwaggerBotton(this ContextMenuStrip menu)
        {
            menu.Items.Add("Swagger", null, (_, _) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "http://localhost:5000/swagger",
                    UseShellExecute = true
                });
            });
        }
    }
}
