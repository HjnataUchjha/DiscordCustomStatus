using DiscordRPC;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace DiscordCustomStatus
{
    public partial class MainWindow : Window
    {
        private DiscordRpcClient _client;
        private bool _isDirty = false;

        public MainWindow()
        {
            InitializeComponent();

            ApiKeyBox.Text = App.Config.ApiKey ?? string.Empty;
            GameDetailsBox.Text = App.Config.GameDetails ?? string.Empty;
            StateBox.Text = App.Config.State ?? string.Empty;
            ImageKeyBox.Text = App.Config.ImageKey ?? string.Empty;
            ImageTextBox.Text = App.Config.ImageText ?? string.Empty;

            ApiKeyBox.TextChanged += OnSettingChanged;
            GameDetailsBox.TextChanged += OnSettingChanged;
            StateBox.TextChanged += OnSettingChanged;
            ImageKeyBox.TextChanged += OnSettingChanged;
            ImageTextBox.TextChanged += OnSettingChanged;

            _isDirty = false;
            SaveConfigButton.IsEnabled = false;

            ConnectionStatusText.Text = "Готово к запуску";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            App.Config.ApiKey = ApiKeyBox.Text;
            App.Config.GameDetails = GameDetailsBox.Text;
            App.Config.State = StateBox.Text;
            App.Config.ImageKey = ImageKeyBox.Text;
            App.Config.ImageText = ImageTextBox.Text;

            ApiKeyBox.IsEnabled = false;
            GameDetailsBox.IsEnabled = false;
            StateBox.IsEnabled = false;
            ImageKeyBox.IsEnabled = false;
            ImageTextBox.IsEnabled = false;

            StartButton.Visibility = Visibility.Collapsed;
            StopButton.Visibility = Visibility.Visible;

            ConnectionStatusText.Text = "Подключение к Discord...";
            ConnectionStatusText.Foreground = Brushes.Orange;

            InitDiscord();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _client?.Dispose();
            _client = null;

            ConnectionStatusText.Text = "Остановлено";
            ConnectionStatusText.Foreground = Brushes.Gray;

            ApiKeyBox.IsEnabled = true;
            GameDetailsBox.IsEnabled = true;
            StateBox.IsEnabled = true;
            ImageKeyBox.IsEnabled = true;
            ImageTextBox.IsEnabled = true;

            StartButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Collapsed;
        }

        private void OnSettingChanged(object sender, TextChangedEventArgs e)
        {
            _isDirty = true;
            SaveConfigButton.IsEnabled = true;
            UnsavedText.Text = "Форма была изменена";
            UnsavedText.Foreground = Brushes.Orange;
        }

        private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void SaveConfig()
        {
            App.Config.ApiKey = ApiKeyBox.Text;
            App.Config.GameDetails = GameDetailsBox.Text;
            App.Config.State = StateBox.Text;
            App.Config.ImageKey = ImageKeyBox.Text;
            App.Config.ImageText = ImageTextBox.Text;

            try
            {
                App.SaveConfig();

                _isDirty = false;
                SaveConfigButton.IsEnabled = false;
                UnsavedText.Text = "Конфиг сохранён";
                UnsavedText.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить конфиг: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitDiscord()
        {
            _client = new DiscordRpcClient(App.Config.ApiKey);
            _client.Initialize();

            _client.OnReady += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ConnectionStatusText.Text = "Подключено к Discord";
                    ConnectionStatusText.Foreground = Brushes.Green;
                });

                _client.SetPresence(new RichPresence
                {
                    Details = App.Config.GameDetails,
                    State = App.Config.State,
                    Assets = new Assets
                    {
                        LargeImageKey = App.Config.ImageKey,
                        LargeImageText = App.Config.ImageText,
                        SmallImageKey = App.Config.ImageKey,
                        SmallImageText = App.Config.ImageText
                    }
                });
            };

            _client.OnError += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ConnectionStatusText.Text = $"Ошибка Discord RPC: {e.Message}";
                    ConnectionStatusText.Foreground = Brushes.Red;
                });
            };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isDirty)
            {
                var res = MessageBox.Show("Форма была изменена. Сохранить перед выходом?", "Сохранить изменения", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    SaveConfig();
                }
            }

            _client?.Dispose();
            base.OnClosing(e);
        }
    }
}