using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Stickly;

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

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        noteTextBox = new RichTextBox();
        customTitleBar = new Panel();
        closeButton = new Button();
        settingsButton = new Button();
        pinButton = new Button();
        settingsContextMenu = new ContextMenuStrip();
        SuspendLayout();
        // 
        // noteTextBox
        // 
        noteTextBox.BackColor = Color.FromArgb(33, 33, 33);
        noteTextBox.BorderStyle = BorderStyle.None;
        noteTextBox.Font = new Font("Arial", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        noteTextBox.ForeColor = SystemColors.ControlLightLight;
        noteTextBox.Location = new Point(8, 40);
        noteTextBox.Margin = new Padding(2);
        noteTextBox.Name = "noteTextBox";
        noteTextBox.Size = new Size(236, 170);
        noteTextBox.TabIndex = 0;
        noteTextBox.Text = "";
        // 
        // customTitleBar
        // 
        customTitleBar.BackColor = Color.FromArgb(45, 45, 48);
        customTitleBar.Dock = DockStyle.Top;
        customTitleBar.Height = 32;
        customTitleBar.Controls.Add(closeButton);
        customTitleBar.Controls.Add(settingsButton);
        customTitleBar.Controls.Add(pinButton);
        customTitleBar.MouseDown += new MouseEventHandler(CustomTitleBar_MouseDown);
        customTitleBar.MouseMove += new MouseEventHandler(CustomTitleBar_MouseMove);
        customTitleBar.MouseUp += new MouseEventHandler(CustomTitleBar_MouseUp);
        customTitleBar.Visible = false;
        // 
        // closeButton
        // 
        closeButton.Text = "X";
        closeButton.BackColor = Color.FromArgb(60, 60, 63);
        closeButton.ForeColor = Color.White;
        closeButton.FlatStyle = FlatStyle.Flat;
        closeButton.FlatAppearance.BorderSize = 0;
        closeButton.Size = new Size(24, 24);
        closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        closeButton.Location = new Point(customTitleBar.Width - closeButton.Width - 4, 4);
        closeButton.Click += new EventHandler(CloseButton_Click);
        closeButton.Visible = false;
        // 
        // settingsButton
        // 
        settingsButton.Text = ". . .";
        settingsButton.BackColor = Color.FromArgb(60, 60, 63);
        settingsButton.ForeColor = Color.White;
        settingsButton.FlatStyle = FlatStyle.Flat;
        settingsButton.FlatAppearance.BorderSize = 0;
        settingsButton.Size = new Size(30, 24);
        settingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        settingsButton.Location = new Point(customTitleBar.Width - closeButton.Width - settingsButton.Width - 8, 4);
        settingsButton.Click += new EventHandler(SettingsButton_Click);
        settingsButton.Visible = false;
        // 
        // settingsContextMenu
        // 
        settingsContextMenu.Items.AddRange(new ToolStripItem[] {
            new ToolStripMenuItem("Check for updates", null, CheckForUpdates_Click)
        });
        // 
        // pinButton
        // 
        pinButton.Text = "📌";
        pinButton.BackColor = Color.FromArgb(60, 60, 63);
        pinButton.ForeColor = Color.White;
        pinButton.FlatStyle = FlatStyle.Flat;
        pinButton.FlatAppearance.BorderSize = 0;
        pinButton.Size = new Size(24, 24);
        pinButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        pinButton.Location = new Point(customTitleBar.Width - closeButton.Width - settingsButton.Width - pinButton.Width - 12, 4);
        pinButton.Click += new EventHandler(PinButton_Click);
        pinButton.Visible = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(33, 33, 33);
        ClientSize = new Size(252, 216);
        Controls.Add(noteTextBox);
        Controls.Add(customTitleBar);
        ForeColor = SystemColors.ControlLight;
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        Margin = new Padding(2);
        Name = "Form1";
        Text = "Stickly";
        ResumeLayout(false);
    }

    private RichTextBox noteTextBox;
    private Panel customTitleBar;
    private Button closeButton;
    private Button settingsButton;
    private Button pinButton;
    private ContextMenuStrip settingsContextMenu;
    private bool isDragging = false;
    private Point lastCursor;
    private Point lastForm;

}
