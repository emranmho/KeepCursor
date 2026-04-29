namespace CursorKeep
{
    public class SettingsDialog : Form
    {
        private readonly TextBox _txtToken;

        public string BotToken => _txtToken.Text.Trim();

        public SettingsDialog(string currentToken)
        {
            Text = "Telegram Settings";
            ClientSize = new Size(400, 130);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblToken = new Label
            {
                Text = "Bot Token:",
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

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(224, 82),
                Size = new Size(75, 30)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(309, 82),
                Size = new Size(75, 30)
            };

            Controls.AddRange([lblToken, _txtToken, btnOk, btnCancel]);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
