using System;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace Stickly
{
    public partial class Form1 : Form
    {
        private const string saveFile = "stickly.json";
        private const int borderWidth = 3; // Width of the custom border

        public Form1()
        {
            InitializeComponent();
            LoadFormData();
            noteTextBox.TextChanged += new EventHandler(OnTextChanged);
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            noteTextBox.Dock = DockStyle.Fill;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 63), borderWidth))
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }

        private void SaveFormData()
        {
            try
            {
                var oldFormData = new FormData();
                if (File.Exists(saveFile))
                {
                    oldFormData = JsonSerializer.Deserialize<FormData>(File.ReadAllText(saveFile));
                }

                var formData = new FormData
                {
                    Text = noteTextBox.Text,
                    LocationX = this.Location.X,
                    LocationY = this.Location.Y,
                    Width = this.Size.Width,
                    Height = this.Size.Height,
                    CustomTitleBar = oldFormData.CustomTitleBar
                };

                var json = JsonSerializer.Serialize(formData);
                File.WriteAllText(saveFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the data");
            }
        }

        private void LoadFormData()
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    var json = File.ReadAllText(saveFile);
                    var formData = JsonSerializer.Deserialize<FormData>(json);

                    if (formData != null)
                    {
                        noteTextBox.Text = formData.Text;

                        ResizeForm(formData.Width, formData.Height);
                        MoveForm(formData.LocationX, formData.LocationY);

                        // Check if the last location is fully visible on the screen, if not move it to the top right of the screen
                        if (!IsFormFullyVisibleOnScreen())
                        {
                            MoveFormToDefaultPosition();
                        }

                        if(formData.AlwaysOnTop)
                        {
                            this.TopMost = true;
                        }

                        if (formData.CustomTitleBar)
                        {
                            customTitleBar.Visible = true;
                            closeButton.Visible = true;
                            FormBorderStyle = FormBorderStyle.None;
                            this.Padding = new Padding(borderWidth); // Add padding for the custom border
                            this.Paint += new PaintEventHandler(OnPaint); // Handle the Paint event
                            ResizeForm(formData.Width, formData.Height); // We need to resize the form again or the form will shrink by the height of the titlebar every time it reopens
                        }
                    }
                }
                else
                {
                    // If there is no config file, move the form to the top right of the screen
                    MoveFormToDefaultPosition();
                    this.TopMost = true; // Set the form to always be on top
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error when loading the savefile");
            }
        }

        private void ResizeForm(int width, int height)
        {
            // Getting the bounds should not be needed according to VS (and copilot) but it is or the form location will not update, even when calling refresh or update methods. Not sure why this is, please let me know if you do.
            _ = Screen.FromControl(this).Bounds;
            this.Size = new Size(width, height);
        }
        private void MoveForm(int x, int y)
        {
            // Getting the bounds should not be needed according to VS (and copilot) but it is or the form location will not update, even when calling refresh or update methods. Not sure why this is, please let me know if you do.
            _ = Screen.FromControl(this).Bounds;
            this.Location = new Point(x, y);
        }

        private void MoveFormToDefaultPosition()
        {
            var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            // Top right corner of the screen with some margin to not block the close button of other windows
            MoveForm(primaryScreenBounds.Right - this.Width - 20, 40);
        }

        private bool IsFormFullyVisibleOnScreen()
        {
            var screenBounds = Screen.FromControl(this).Bounds;
            var formBounds = new Rectangle(this.Location, this.Size);
            return screenBounds.Contains(formBounds);
        }


        private void OnTextChanged(object sender, EventArgs e)
        {
            SaveFormData();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveFormData();
        }

        private void CustomTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastCursor = Cursor.Position;
            lastForm = this.Location;
        }

        private void CustomTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Location = new Point(
                    lastForm.X + Cursor.Position.X - lastCursor.X,
                    lastForm.Y + Cursor.Position.Y - lastCursor.Y);
            }
        }

        private void CustomTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        // Override WndProc to handle resizing
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HTCLIENT)
                {
                    Point pt = PointToClient(new Point(m.LParam.ToInt32()));
                    if (pt.X < borderWidth)
                    {
                        if (pt.Y < borderWidth)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (pt.Y > ClientSize.Height - borderWidth)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else
                            m.Result = (IntPtr)HTLEFT;
                    }
                    else if (pt.X > ClientSize.Width - borderWidth)
                    {
                        if (pt.Y < borderWidth)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (pt.Y > ClientSize.Height - borderWidth)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else
                            m.Result = (IntPtr)HTRIGHT;
                    }
                    else if (pt.Y < borderWidth)
                        m.Result = (IntPtr)HTTOP;
                    else if (pt.Y > ClientSize.Height - borderWidth)
                        m.Result = (IntPtr)HTBOTTOM;
                }
                Invalidate(); // Invalidate the form to trigger a repaint
                return;
            }
            base.WndProc(ref m);
        }


    }
    public class FormData
    {
        public bool AlwaysOnTop { get; set; } = true;
        public bool CustomTitleBar { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string Text { get; set; }

    }

}