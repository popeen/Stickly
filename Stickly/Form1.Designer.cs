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
    private bool isDragging = false;
    private Point lastCursor;
    private Point lastForm;

}
