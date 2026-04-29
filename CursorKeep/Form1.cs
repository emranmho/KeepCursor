using CursorKeep.Commands;
using CursorKeep.Controllers;
using CursorKeep.Services;

namespace CursorKeep
{
    public partial class Form1 : Form
    {
        private readonly AppController _controller;
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

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            ApplyIcon(_iconStopped, "CursorKeep – Stopped");
        }

        private static Icon LoadIcon(string filename)
        {
            var stream = typeof(Form1).Assembly
                .GetManifestResourceStream($"CursorKeep.Icons.{filename}")!;
            return new Icon(stream);
        }

        private void ApplyIcon(Icon icon, string tooltip)
        {
            Icon = icon;
            notifyIcon.Icon = icon;
            notifyIcon.Text = tooltip;
            notifyIcon.Visible = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _controller.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            ApplyIcon(_iconRunning, "CursorKeep – Running");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _controller.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            ApplyIcon(_iconStopped, "CursorKeep – Stopped");
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

        private void startToolStripMenuItem_Click(object sender, EventArgs e) =>
            btnStart_Click(btnStart, EventArgs.Empty);

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) =>
            btnStop_Click(btnStop, EventArgs.Empty);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
