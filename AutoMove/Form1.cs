using CursorKeep.Commands;
using CursorKeep.Services;
using CursorKeep.Controllers;

namespace CursorKeep
{
    public partial class Form1 : Form
    {
        private AppController _controller;
        private readonly HotkeyManager _hotkeyManager;

        public Form1()
        {
            InitializeComponent();
            InitializeApp();

            // Initialize HotkeyManager
            _hotkeyManager = new HotkeyManager(this.Handle);
            _hotkeyManager.StartHotkeyPressed += (s, e) => btnStart_Click(btnStart, EventArgs.Empty);
            _hotkeyManager.StopHotkeyPressed += (s, e) => btnStop_Click(btnStop, EventArgs.Empty);
            _hotkeyManager.ExitHotkeyPressed += (s, e) => Application.Exit();
        }


        private void InitializeApp()
        {
            // Initialize the service with a 10-second interval
            var mouseMoverService = new MouseMoverService(10000);

            // Create commands for start and stop actions
            var startCommand = new StartCommand(mouseMoverService);
            var stopCommand = new StopCommand(mouseMoverService);

            // Initialize controller with the commands
            _controller = new AppController(startCommand, stopCommand);

            // Initialize UI state
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _controller.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _controller.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _controller.Stop();
            _hotkeyManager.Dispose();
            base.OnFormClosed(e);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void From_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void contextMenuStripAutoMove_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_controller.IsMoving)
            {
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
            }
            else
            {
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStart_Click(btnStart, EventArgs.Empty);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStop_Click(btnStop, EventArgs.Empty);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
