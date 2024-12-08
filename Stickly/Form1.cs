using System;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace Stickly
{
    public partial class Form1 : Form
    {
        private const string saveFile = "stickly.json";

        public Form1()
        {
            InitializeComponent();
            this.TopMost = true; // Set the form to always be on top
            LoadFormData();
            noteTextBox.TextChanged += new EventHandler(OnTextChanged);
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            noteTextBox.Dock = DockStyle.Fill;
        }

        private void SaveFormData()
        {
            try
            {
                var formData = new FormData
                {
                    Text = noteTextBox.Text,
                    LocationX = this.Location.X,
                    LocationY = this.Location.Y,
                    Width = this.Size.Width,
                    Height = this.Size.Height
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
                    }
                }
                else
                {
                    // If there is no config file, move the form to the top right of the screen
                    MoveFormToDefaultPosition();
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

    }
    public class FormData
    {
        public string Text { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }

}