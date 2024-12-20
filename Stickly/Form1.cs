using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Stickly
{
    public partial class Form1 : Form
    {
        private const string saveFile = "stickly.json";
        private const int borderWidth = 3;
        private FormData saveData = new FormData();
        private FileSystemWatcher? fileWatcher;

        public Form1()
        {
            InitializeComponent();
            LoadFormData();
            noteTextBox.TextChanged += new EventHandler(OnTextChanged);
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);
            noteTextBox.Dock = DockStyle.Fill;

            if (saveData.ListenForSavefileChanges)
            {
                InitializeFileWatcher();
            }

        }

        private void LoadFormData()
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    var json = File.ReadAllText(saveFile);
                    saveData = JsonSerializer.Deserialize<FormData>(json) ?? new FormData();

                    ResizeForm(saveData.Width, saveData.Height);
                    MoveForm(saveData.LocationX, saveData.LocationY);

                    // Check if the last location is fully visible on the screen, if not move it to the top right of the screen
                    if (!IsFormFullyVisibleOnScreen())
                    {
                        MoveFormToDefaultPosition();
                    }

                }
                else
                {
                    // If there is no config file, move the form to the top right of the screen
                    MoveFormToDefaultPosition();
                }

                noteTextBox.Text = saveData.Text;

                this.TopMost = saveData.AlwaysOnTop;

                if (saveData.CustomTitleBar)
                {
                    customTitleBar.Visible = true;
                    closeButton.Visible = true;
                    settingsButton.Visible = true;
                    pinButton.Visible = true;
                    FormBorderStyle = FormBorderStyle.None;
                    this.Padding = new Padding(borderWidth); // Add padding for the custom border
                    this.Paint += new PaintEventHandler(DrawBoarderOnPaint); // Handle the Paint event
                }

                UpdateStyling();

                // We put this at the end since the size of the form can change and if it does it can lead to form being smaller and smaller on every restart unless we do this
                ResizeForm(saveData.Width, saveData.Height);

            }
            catch
            {
                MessageBox.Show("There was an error when loading the savefile");
            }
        }

        private void SaveFormData()
        {
            try
            {

                // Temporarily disable the FileSystemWatcher to avoid triggering it when we update the file
                if (fileWatcher != null)
                {
                    fileWatcher.EnableRaisingEvents = false;
                }

                saveData.Text = noteTextBox.Text;
                saveData.LocationX = this.Location.X;
                saveData.LocationY = this.Location.Y;
                saveData.Width = this.Size.Width;
                saveData.Height = this.Size.Height;

                var json = JsonSerializer.Serialize(saveData);
                File.WriteAllText(saveFile, json);

                // Re-enable the FileSystemWatcher
                if (fileWatcher != null)
                {
                    fileWatcher.EnableRaisingEvents = true;
                }

            }
            catch
            {
                MessageBox.Show("Could not save the data");
            }
        }

        private void InitializeFileWatcher()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory; // Get the application's base directory
            string fullPath = Path.Combine(directory, saveFile); // Combine the base directory with the save file name

            if (Directory.Exists(directory) && File.Exists(fullPath))
            {
                fileWatcher = new FileSystemWatcher
                {
                    Path = directory,
                    Filter = Path.GetFileName(saveFile),
                    NotifyFilter = NotifyFilters.LastWrite
                };

                fileWatcher.Changed += OnSaveFileChanged;
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        private void OnSaveFileChanged(object sender, FileSystemEventArgs e)
        {
            // Reload the form data on a separate thread to avoid cross-thread operation exceptions
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                LoadFormData();
            });
        }
        private void OnTextChanged(object? sender, EventArgs e)
        {
            SaveFormData();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveFormData();
        }

        public void UpdateStyling()
        {
            this.BackColor = ColorTranslator.FromHtml(saveData.Colors.Background);
            noteTextBox.BackColor = ColorTranslator.FromHtml(saveData.Colors.Background);
            noteTextBox.ForeColor = ColorTranslator.FromHtml(saveData.Colors.Text);
            customTitleBar.BackColor = ColorTranslator.FromHtml(saveData.Colors.TitleBarBackground);
            customTitleBar.ForeColor = ColorTranslator.FromHtml(saveData.Colors.TitleBarText);

            closeButton.BackColor = ColorTranslator.FromHtml(saveData.Colors.CloseButtonBackground);
            closeButton.ForeColor = ColorTranslator.FromHtml(saveData.Colors.CloseButtonText);

            settingsButton.BackColor = ColorTranslator.FromHtml(saveData.Colors.SettingsButtonBackground);
            settingsButton.ForeColor = ColorTranslator.FromHtml(saveData.Colors.SettingsButtonText);

            if (saveData.AlwaysOnTop)
            {
                pinButton.BackColor = ColorTranslator.FromHtml(saveData.Colors.PinEnabledButtonBackground);
                pinButton.ForeColor = ColorTranslator.FromHtml(saveData.Colors.PinEnabledButtonText);
            }
            else
            {
                pinButton.BackColor = ColorTranslator.FromHtml(saveData.Colors.PinDisabledButtonBackground);
                pinButton.ForeColor = ColorTranslator.FromHtml(saveData.Colors.PinDisabledButtonText);
            }

        }

        private void DrawBoarderOnPaint(object? sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen pen = new Pen(ColorTranslator.FromHtml(saveData.Colors.Border), borderWidth))
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
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
            var primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen != null)
            {
                var primaryScreenBounds = primaryScreen.Bounds;
                // Top right corner of the screen with some margin to not block the close button of other windows
                MoveForm(primaryScreenBounds.Right - this.Width - 20, 40);
            }
            else
            {
                // This shouldn't happen but if it does, default to top left corner instead of crashing
                MoveForm(0, 0); // Default to top-left corner
            }
        }

        private bool IsFormFullyVisibleOnScreen()
        {
            var screenBounds = Screen.FromControl(this).Bounds;
            var formBounds = new Rectangle(this.Location, this.Size);
            return screenBounds.Contains(formBounds);
        }

        private string GetCurrentVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            // If version is null, return a dummy version number
            return version != null ? version.ToString() : "1.0.0.0";
        }

        private static async Task<Release> GetLatestVersionAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.github.com/repos/popeen/stickly/releases";
                client.DefaultRequestHeaders.Add("User-Agent", "stickly-updater");
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var releases = JsonSerializer.Deserialize<List<Release>>(jsonResponse);

                return new Release
                {
                    Version = releases[0].tag_name,
                    ChangeLog = releases[0].html_url,
                    DownloadLink = releases[0].html_url
                };
            }
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            saveData.AlwaysOnTop = !saveData.AlwaysOnTop;
            this.TopMost = saveData.AlwaysOnTop;
            SaveFormData();
            UpdateStyling();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            settingsContextMenu.Show(settingsButton, new Point(0, settingsButton.Height));
        }

        private async void CheckForUpdates_Click(object sender, EventArgs e)
        {
            Release latestVersion = await GetLatestVersionAsync();
            string version = GetCurrentVersion();

            if (latestVersion == null || string.IsNullOrEmpty(latestVersion.Version) || string.IsNullOrEmpty(version))
            {
                MessageBox.Show("Unable to check for updates. Version information is missing.");
                return;
            }

            if (new Version(latestVersion.Version).CompareTo(new Version(version)) > 0)
            {
                DialogResult result = MessageBox.Show($"There is a new version available.\n\nCurrent version: {version}\nLatest version: {latestVersion.Version}\n\nDo you want to download the new version?", "Update Available", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = latestVersion.DownloadLink,
                        UseShellExecute = true
                    });
                }
            }
            else
            {
                MessageBox.Show("You are already using the latest version");
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastCursor = Cursor.Position;
            lastForm = this.Location;
        }

        private void CustomTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
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

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
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
        public bool CustomTitleBar { get; set; } = true;
        public bool ListenForSavefileChanges { get; set; } = true;
        public Colors Colors { get; set; } = new Colors();
        public int Width { get; set; } = 252;
        public int Height { get; set; } = 220;
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string Text { get; set; } = String.Empty;

    }

    public class Colors
    {
        public string Background { get; set; } = "#212121";
        public string Text { get; set; } = "#FFFFFF";
        public string Border { get; set; } = "#3C3C3F";
        public string TitleBarBackground { get; set; } = "#2D2D30";
        public string TitleBarText { get; set; } = "#FFFFFF";
        public string CloseButtonBackground { get; set; } = "#3C3C3F";
        public string CloseButtonText { get; set; } = "#FF636E";
        public string SettingsButtonBackground { get; set; } = "#3C3C3F";
        public string SettingsButtonText { get; set; } = "#EEEEEE";
        public string PinEnabledButtonBackground { get; set; } = "#3C3C3F";
        public string PinEnabledButtonText { get; set; } = "#55AB55";
        public string PinDisabledButtonBackground { get; set; } = "#3C3C3F";
        public string PinDisabledButtonText { get; set; } = "#FF636E";

    }

    public class Release
    {
        public string Version { get; set; }
        public string ChangeLog { get; set; }
        public string DownloadLink { get; set; }

        public string tag_name { get; set; }
        public string html_url { get; set; }
    }

}