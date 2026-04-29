using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace CursorKeep.Services
{
    public class MouseMoverService
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private readonly Timer _timer;
        private readonly Random _random;

        public bool IsRunning => _timer.Enabled;

        public MouseMoverService(int interval)
        {
            _timer = new Timer { Interval = interval };
            _timer.Tick += Timer_Tick;
            _random = new Random();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int x = Cursor.Position.X + _random.Next(-100, 101);
            int y = Cursor.Position.Y + _random.Next(-100, 101);
            SetCursorPos(x, y);
            SendKeys.Send("+");
        }

        public void Start()
        {
            if (!_timer.Enabled)
                _timer.Start();
        }

        public void Stop()
        {
            if (_timer.Enabled)
                _timer.Stop();
        }
    }
}
