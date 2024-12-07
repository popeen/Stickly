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
        SuspendLayout();

        noteTextBox.BackColor = Color.FromArgb(33, 33, 33);
        noteTextBox.BorderStyle = BorderStyle.None;
        noteTextBox.Font = new Font("Arial", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        noteTextBox.ForeColor = SystemColors.ControlLightLight;
        noteTextBox.Location = new Point(12, 12);

        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(33, 33, 33);
        ClientSize = new Size(360, 360);
        Controls.Add(noteTextBox);
        ForeColor = SystemColors.ControlLight;
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        Text = "Stickly";
        ResumeLayout(false);
    }

    private RichTextBox noteTextBox;
}
