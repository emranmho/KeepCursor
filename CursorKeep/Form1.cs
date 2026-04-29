using CursorKeep.Commands;
using CursorKeep.Controllers;
using CursorKeep.Services;

namespace CursorKeep
{
    public partial class Form1 : Form
    {
        private readonly AppController _controller;
        private readonly ConfigService _configService;
        private readonly TelegramBotService _telegramService;
        private readonly StartupService _startupService;
        private readonly Icon _iconRunning;
        private readonly Icon _iconStopped;
        private const int MouseMoveIntervalMs = 10_000;

        public Form1()
        {
            InitializeComponent();

            _iconRunning = LoadIcon("running.ico");
            _iconStopped = LoadIcon("stopped.ico");

            var service = new MouseMoverService(MouseMoveIntervalMs);
            _controller = new AppController(new StartCommand(service), new StopCommand(service));

            _configService = new ConfigService();
            _startupService = new StartupService();
            _startupService.SyncPath(Application.ExecutablePath);

            _telegramService = new TelegramBotService(
                _configService,
                () => _controller.IsMoving,
                () => { if (IsHandleCreated && !IsDisposed) Invoke(SetRunning); },
                () => { if (IsHandleCreated && !IsDisposed) Invoke(SetStopped); },
                action => { if (IsHandleCreated && !IsDisposed) Invoke(action); });

            SetStopped();

            if (!string.IsNullOrWhiteSpace(_configService.BotToken))
            {
                bool ok = _telegramService.Start(_configService.BotToken);
                if (!ok)
                {
                    lblTelegramStatus.ForeColor = Color.Red;
                    lblTelegramStatus.Text = "● Telegram: click ⚙ to reconfigure";
                    return;
                }
            }

            UpdateTelegramStatus();
        }

        private static Icon LoadIcon(string filename)
        {
            var stream = typeof(Form1).Assembly
                .GetManifestResourceStream($"CursorKeep.Icons.{filename}")!;
            return new Icon(stream);
        }

        private void SetRunning()
        {
            _controller.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            ApplyIcon(_iconRunning, "CursorKeep – Running");
        }

        private void SetStopped()
        {
            _controller.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            ApplyIcon(_iconStopped, "CursorKeep – Stopped");
        }

        private void ApplyIcon(Icon icon, string tooltip)
        {
            Icon = icon;
            notifyIcon.Icon = icon;
            notifyIcon.Text = tooltip;
            notifyIcon.Visible = true;
        }

        private void btnStart_Click(object sender, EventArgs e) => SetRunning();

        private void btnStop_Click(object sender, EventArgs e) => SetStopped();

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using var dlg = new SettingsDialog(_configService.BotToken, _startupService.IsEnabled());
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var token = dlg.BotToken;

            if (!string.IsNullOrWhiteSpace(token))
            {
                btnSettings.Enabled = false;
                btnSettings.Text = "…";
                bool valid = await TelegramBotService.ValidateTokenAsync(token);
                btnSettings.Enabled = true;
                btnSettings.Text = "⚙";

                if (!valid)
                {
                    MessageBox.Show("Invalid bot token. Please check and try again.",
                        "Invalid Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _configService.SaveToken(token);

            if (!string.IsNullOrWhiteSpace(token))
                _telegramService.Start(token);
            else
                _telegramService.Stop();

            if (dlg.RunOnStartup)
                _startupService.Enable(Application.ExecutablePath);
            else
                _startupService.Disable();

            UpdateTelegramStatus();
        }

        private void UpdateTelegramStatus()
        {
            if (_telegramService.IsConnected)
            {
                lblTelegramStatus.ForeColor = Color.Green;
                lblTelegramStatus.Text = "● Telegram: Connected";
            }
            else
            {
                lblTelegramStatus.ForeColor = Color.Gray;
                lblTelegramStatus.Text = "● Telegram: Not configured";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _telegramService.Dispose();
            _controller.Stop();
            notifyIcon.Visible = false;
            base.OnFormClosed(e);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void From_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void contextMenuStripAutoMove_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            startToolStripMenuItem.Enabled = !_controller.IsMoving;
            stopToolStripMenuItem.Enabled = _controller.IsMoving;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e) => SetRunning();

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) => SetStopped();

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
