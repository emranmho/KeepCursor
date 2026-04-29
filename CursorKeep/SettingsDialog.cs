namespace CursorKeep
{
    public class SettingsDialog : Form
    {
        private readonly TextBox _txtToken;
        private readonly CheckBox _chkStartup;

        public string BotToken => _txtToken.Text.Trim();
        public bool RunOnStartup => _chkStartup.Checked;

        public SettingsDialog(string currentToken, bool startupEnabled)
        {
            Text = "Settings";
            ClientSize = new Size(400, 165);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblToken = new Label
            {
                Text = "Telegram Bot Token:",
                Location = new Point(16, 16),
                AutoSize = true
            };

            _txtToken = new TextBox
            {
                Location = new Point(16, 38),
                Size = new Size(368, 28),
                Text = currentToken,
                PlaceholderText = "Paste your Telegram bot token here"
            };

            _chkStartup = new CheckBox
            {
                Text = "Run on Windows startup",
                Location = new Point(16, 80),
                AutoSize = true,
                Checked = startupEnabled
            };

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(224, 118),
                Size = new Size(75, 30)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(309, 118),
                Size = new Size(75, 30)
            };

            Controls.AddRange([lblToken, _txtToken, _chkStartup, btnOk, btnCancel]);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
