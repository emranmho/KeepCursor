namespace CursorKeep;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        btnStart = new Button();
        btnStop = new Button();
        notifyIcon = new NotifyIcon(components);
        contextMenuStripAutoMove = new ContextMenuStrip(components);
        startToolStripMenuItem = new ToolStripMenuItem();
        stopToolStripMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        contextMenuStripAutoMove.SuspendLayout();
        SuspendLayout();
        // 
        // btnStart
        // 
        btnStart.BackColor = SystemColors.Control;
        btnStart.ForeColor = Color.Black;
        btnStart.Location = new Point(56, 31);
        btnStart.Margin = new Padding(4, 4, 4, 4);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(118, 36);
        btnStart.TabIndex = 0;
        btnStart.Text = "Start";
        btnStart.UseVisualStyleBackColor = false;
        btnStart.Click += btnStart_Click;
        // 
        // btnStop
        // 
        btnStop.BackColor = SystemColors.Control;
        btnStop.Location = new Point(221, 31);
        btnStop.Margin = new Padding(4, 4, 4, 4);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(118, 36);
        btnStop.TabIndex = 1;
        btnStop.Text = "Stop";
        btnStop.UseVisualStyleBackColor = false;
        btnStop.Click += btnStop_Click;
        // 
        // notifyIcon
        // 
        notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
        notifyIcon.ContextMenuStrip = contextMenuStripAutoMove;
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "CursorKeep";
        notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
        // 
        // contextMenuStripAutoMove
        // 
        contextMenuStripAutoMove.ImageScalingSize = new Size(20, 20);
        contextMenuStripAutoMove.Items.AddRange(new ToolStripItem[] { startToolStripMenuItem, stopToolStripMenuItem, exitToolStripMenuItem });
        contextMenuStripAutoMove.Name = "contextMenuStripAutoMove";
        contextMenuStripAutoMove.Size = new Size(122, 100);
        contextMenuStripAutoMove.Opening += contextMenuStripAutoMove_Opening;
        // 
        // startToolStripMenuItem
        // 
        startToolStripMenuItem.Name = "startToolStripMenuItem";
        startToolStripMenuItem.Size = new Size(121, 32);
        startToolStripMenuItem.Text = "Start";
        startToolStripMenuItem.Click += startToolStripMenuItem_Click;
        // 
        // stopToolStripMenuItem
        // 
        stopToolStripMenuItem.Name = "stopToolStripMenuItem";
        stopToolStripMenuItem.Size = new Size(121, 32);
        stopToolStripMenuItem.Text = "Stop";
        stopToolStripMenuItem.Click += stopToolStripMenuItem_Click;
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(121, 32);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        ClientSize = new Size(390, 94);
        Controls.Add(btnStop);
        Controls.Add(btnStart);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4, 4, 4, 4);
        MaximizeBox = false;
        Name = "Form1";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "App";
        Resize += From_Resize;
        contextMenuStripAutoMove.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Button btnStart;
    private Button btnStop;
    private NotifyIcon notifyIcon;
    private ToolStripMenuItem startToolStripMenuItem;
    private ToolStripMenuItem stopToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ContextMenuStrip contextMenuStripAutoMove;
}
