using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CursorKeep.Services
{
    public class HotkeyManager : IMessageFilter, IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private readonly IntPtr _handle;

        public event EventHandler StartHotkeyPressed;
        public event EventHandler StopHotkeyPressed;
        public event EventHandler ExitHotkeyPressed;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifier keys for RegisterHotKey
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;


        public HotkeyManager(IntPtr handle)
        {
            _handle = handle;
            Application.AddMessageFilter(this);
            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            // Register CTRL + SHIFT + S for Start (id 1)
            RegisterHotKey(_handle, 1, MOD_CTRL | MOD_SHIFT, (uint)Keys.S);

            // Register CTRL + SHIFT + T for Stop (id 2)
            RegisterHotKey(_handle, 2, MOD_CTRL | MOD_SHIFT, (uint)Keys.T);

            // Register CTRL + SHIFT + E for Exit (id 3)
            RegisterHotKey(_handle, 3, MOD_CTRL | MOD_SHIFT, (uint)Keys.E);
        }

        public void Dispose()
        {
            UnregisterHotKey(_handle, 1);
            UnregisterHotKey(_handle, 2);
            UnregisterHotKey(_handle, 3);
            Application.RemoveMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 1:
                        StartHotkeyPressed?.Invoke(this, EventArgs.Empty);
                        break;
                    case 2:
                        StopHotkeyPressed?.Invoke(this, EventArgs.Empty);
                        break;
                    case 3:
                        ExitHotkeyPressed?.Invoke(this, EventArgs.Empty);
                        break;
                }
                return true;
            }
            return false;
        }
    }
}
