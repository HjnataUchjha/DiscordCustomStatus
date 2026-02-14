using DiscordCustomStatus.AppConfig;
using DiscordRPC;

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

        public static void RunDiscordRpcClient(this NotifyIcon notifyIcon, DiscordRpcClient client)
        {
            var menu = notifyIcon.ContextMenuStrip;
            var runItem = new ToolStripMenuItem("Запустить");
            var stopItem = new ToolStripMenuItem("Остановить") { Visible = false };

            menu.Items.Add(runItem);
            menu.Items.Add(stopItem);

            void ShowRun()
            {
                if (menu.InvokeRequired)
                {
                    menu.Invoke(() => 
                    { 
                        runItem.Visible = true; 
                        stopItem.Visible = false; 
                    });
                }                    
                else
                {
                    runItem.Visible = true;
                    stopItem.Visible = false;
                }
            }

            void StopAndDispose()
            {
                try
                {
                    client?.Dispose();

                    var config = AppConfigHelper.Config.DcsConfigs.First(x => x.Id == AppConfigHelper.Config.CurrentDcsConfigId);
                    notifyIcon.ShowBalloonTip(
                        3000,
                        "Discord Custom Status",
                        $"<{config.Name}> остановлен",
                        ToolTipIcon.Info
                    );
                }
                catch { }
                client = null;
                ShowRun();
            }

            runItem.Click += (_, _) =>
            {
                var config = AppConfigHelper.Config.DcsConfigs.First(x => x.Id == AppConfigHelper.Config.CurrentDcsConfigId);
                client = new DiscordRpcClient(config.ApiKey);
                try
                {
                    client.Initialize();
                }
                catch
                {
                    StopAndDispose();
                    return;
                }

                client.OnReady += (sender, e) =>
                {
                    try
                    {
                        client.SetPresence(new RichPresence
                        {
                            Details = config.GameDetails,
                            State = config.State,
                            Assets = new Assets
                            {
                                LargeImageKey = config.ImageKey,
                                LargeImageText = config.ImageText,
                                SmallImageKey = config.ImageKey,
                                SmallImageText = config.ImageText
                            }
                        });
                        notifyIcon.ShowBalloonTip(
                            3000,
                            "Discord Custom Status",
                            $"<{config.Name}> запущен",
                            ToolTipIcon.Info
                        );
                    }
                    catch { }
                };

                client.OnError += (sender, e) =>
                {
                    StopAndDispose();
                };

                if (menu.InvokeRequired)
                {
                    menu.Invoke(() => 
                    { 
                        runItem.Visible = false; 
                        stopItem.Visible = true; 
                    });
                }
                else
                {
                    runItem.Visible = false;
                    stopItem.Visible = true;
                }
            };

            stopItem.Click += (_, _) =>
            {
                StopAndDispose();
            };
        }

        public static void AddShutdownBotton(this ContextMenuStrip menu, Action shutdown)
        {
            menu.Items.Add("Выход", null, (_, _) =>
            {
                shutdown.Invoke();
            });
        }

        public static void AddSwaggerBotton(this ContextMenuStrip menu, int port)
        {
            menu.Items.Add("Swagger", null, (_, _) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = $"http://localhost:{port}/swagger",
                    UseShellExecute = true
                });
            });
        }

        public static void AddSettingsButton(this ContextMenuStrip menu, int port)
        {
            menu.Items.Add("Настройки", null, (_, _) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = $"http://localhost:{port}/settings.html",
                    UseShellExecute = true
                });
            });
        }

        public static void AddCurrentConfigLabel(this ContextMenuStrip menu, int updateIntervalMs = 1000)
        {
            var item = new ToolStripMenuItem
            {
                Enabled = false
            };
            menu.Items.Add(item);

            void UpdateText()
            {
                try
                {
                    var config = AppConfigHelper.Config;
                    string name = null;
                    if (config.CurrentDcsConfigId != null)
                    {
                        var current = config.DcsConfigs.First(d => d.Id == config.CurrentDcsConfigId);
                        name = current.Name;
                    }

                    item.Text = string.IsNullOrEmpty(name) ? "<не выбрано>" : name;
                }
                catch
                {
                    item.Text = "<ошибка>";
                }
            }

            UpdateText();

            var timer = new System.Timers.Timer(updateIntervalMs);
            timer.Elapsed += (_, _) =>
            {
                try
                {
                    if (menu.InvokeRequired)
                        menu.Invoke((Action)UpdateText);
                    else
                        UpdateText();
                }
                catch { }
            };
            timer.AutoReset = true;
            timer.Start();
        }
    }
}
