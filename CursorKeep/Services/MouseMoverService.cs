using System;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;


namespace CursorKeep.Services
{
    public class MouseMoverService
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private System.Windows.Forms.Timer _timer;
        private readonly Random _random;


        public MouseMoverService(int interval)
        {
            _timer = new Timer { Interval = interval };
            _timer.Tick += Timer_Tick;
            _timer.Tick += (s, e) => PressKeyboardKey();
            _random = new Random();
        }

        private void PressKeyboardKey()
        {
            SendKeys.Send("+");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveMouse();
        }

        private void MoveMouse()
        {
            int x = Cursor.Position.X + _random.Next(-100, 101);
            int y = Cursor.Position.Y + _random.Next(-100, 101); 
            SetCursorPos(x, y);
        }

        public void Start()
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
                Console.WriteLine("Mouse mover started.");
            }
        }

        public void Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                Console.WriteLine("Mouse mover stopped.");
            }
        }
    }
}
