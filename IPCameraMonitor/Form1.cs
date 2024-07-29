using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Accord.Video.FFMPEG;
using AForge.Video;

namespace IPCameraMonitor
{
    public partial class Form1 : Form
    {
        private List<MJPEGStream> mjpegStreams = new List<MJPEGStream>();
        private const string ConfigFilePath = "CameraConfig.xml";
        private const string RecordsFilePath = "CameraRecords.xml";
        private Dictionary<string, (ToolStripMenuItem btnConnect, ToolStripMenuItem btnDisconnect)> cameraButtons = new Dictionary<string, (ToolStripMenuItem, ToolStripMenuItem)>();
        private Dictionary<string, VideoFileWriter> videoWriters = new Dictionary<string, VideoFileWriter>();
        private List<CameraRecord> cameraRecords = new List<CameraRecord>();
        private Dictionary<string, TreeNode> cameraTreeNodes = new Dictionary<string, TreeNode>();

        public Form1()
        {
            InitializeComponent();
            LoadCameraConfigurations();
            LoadRecordsFromFile();
            this.FormClosing += Form1_FormClosing;
        }

        private async void btnAddCamera_Click(object sender, EventArgs e)
        {
            AddCameraPanelAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the FlowLayoutPanel
        }

        private async Task AddCameraPanelAsync(CameraConfig config = null)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int groupBoxWidth = (screenWidth - 400) / 4;

            GroupBox cameraGroupBox = new GroupBox
            {
                Name = config != null ? config.Nickname : "",
                Size = new Size(groupBoxWidth, 350),
                Padding = new Padding(10),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Navy,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(10)
            };

            Panel actionPanel = new Panel
            {
                Size = new Size(groupBoxWidth - 20, 40),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                Dock = DockStyle.Top,
            };

            GroupBox configPanel = new GroupBox
            {
                Name = "configPanel",
                Size = new Size(groupBoxWidth - 40, 200),
                Text = "Camera Configuration",
                Padding = new Padding(10),
                Dock = DockStyle.Top
            };

            configPanel.Location = new Point(
                (cameraGroupBox.ClientSize.Width - configPanel.Width) / 2,
                actionPanel.Bottom + 10
            );

            Panel cameraPanel = new Panel
            {
                Size = new Size(groupBoxWidth - 20, 260),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Dock = DockStyle.Fill,
            };

            PictureBox pictureBox = new PictureBox
            {
                Name = "pictureBox",
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black,
            };
            pictureBox.DoubleClick += (s, e) => OpenFullScreen(pictureBox);

            Label lblIPAddress = new Label { Text = "IP Address", Location = new Point(10, 20), AutoSize = true };
            TextBox txtIPAddress = new TextBox { Name = "txtIPAddress", Size = new Size(250, 20), Location = new Point(100, 20) };

            Label lblUsername = new Label { Text = "Username", Location = new Point(10, 50), AutoSize = true };
            TextBox txtUsername = new TextBox { Name = "txtUsername", Size = new Size(250, 20), Location = new Point(100, 50) };

            Label lblPassword = new Label { Text = "Password", Location = new Point(10, 80), AutoSize = true };
            TextBox txtPassword = new TextBox { Name = "txtPassword", Size = new Size(250, 20), Location = new Point(100, 80), UseSystemPasswordChar = true };

            Label lblStreamType = new Label { Text = "Stream Type", Location = new Point(10, 110), AutoSize = true };
            ComboBox comboBoxStreamType = new ComboBox { Name = "comboBoxStreamType", Location = new Point(100, 110), Size = new Size(250, 21) };
            comboBoxStreamType.Items.AddRange(new object[] { "MJPEG" });
            comboBoxStreamType.SelectedIndex = 0;

            Label lblNickname = new Label { Text = "Nickname", Location = new Point(10, 140), AutoSize = true };
            TextBox txtNickname = new TextBox { Name = "txtNickname", Size = new Size(250, 20), Location = new Point(100, 140) };

            Label lblGroups = new Label { Text = "Groups", Location = new Point(10, 170), AutoSize = true };
            TextBox txtGroups = new TextBox { Name = "txtGroups", Size = new Size(250, 20), Location = new Point(100, 170) };

            Button btnActions = new Button { Text = "Actions", Size = new Size(70, 25), Location = new Point(10, 5) };
            ContextMenuStrip actionMenu = new ContextMenuStrip();
            btnActions.ContextMenuStrip = actionMenu;

            ToolStripMenuItem connectMenuItem = new ToolStripMenuItem("Connect");
            ToolStripMenuItem disconnectMenuItem = new ToolStripMenuItem("Disconnect");
            ToolStripMenuItem recordStreamMenuItem = new ToolStripMenuItem("Record");
            ToolStripMenuItem stopRecordingMenuItem = new ToolStripMenuItem("Stop");
            ToolStripMenuItem removeMenuItem = new ToolStripMenuItem("Remove");

            actionMenu.Items.AddRange(new ToolStripItem[]
            {
                connectMenuItem,
                disconnectMenuItem,
                recordStreamMenuItem,
                stopRecordingMenuItem,
                removeMenuItem
            });

            connectMenuItem.Click += async (s, e) => await ConnectCameraAsync(txtIPAddress.Text, txtUsername.Text, txtPassword.Text, comboBoxStreamType.SelectedItem.ToString(), pictureBox, connectMenuItem, disconnectMenuItem, recordStreamMenuItem, stopRecordingMenuItem, removeMenuItem, cameraPanel, configPanel);
            disconnectMenuItem.Click += (s, e) => DisconnectCamera(txtIPAddress.Text, connectMenuItem, disconnectMenuItem, recordStreamMenuItem, stopRecordingMenuItem);
            recordStreamMenuItem.Click += (s, e) => SaveStream(txtIPAddress.Text, comboBoxStreamType.SelectedItem.ToString(), txtUsername.Text, txtPassword.Text, recordStreamMenuItem, stopRecordingMenuItem);
            stopRecordingMenuItem.Click += (s, e) => StopRecording(txtIPAddress.Text, recordStreamMenuItem, stopRecordingMenuItem);
            removeMenuItem.Click += (s, e) => RemoveCameraPanel(cameraGroupBox);

            btnActions.Click += (s, e) => actionMenu.Show(btnActions, new Point(0, btnActions.Height));

            if (config != null)
            {
                cameraButtons.Add(config.IPAddress, (connectMenuItem, disconnectMenuItem));
            }

            SetPlaceholder(txtIPAddress, "IP Address");
            SetPlaceholder(txtUsername, "Username");
            SetPlaceholder(txtPassword, "Password");
            SetPlaceholder(txtNickname, "Nickname");
            SetPlaceholder(txtGroups, "Groups (comma-separated)");

            if (config != null)
            {
                txtIPAddress.Text = config.IPAddress;
                txtUsername.Text = config.Username;
                txtPassword.Text = config.Password;
                comboBoxStreamType.SelectedItem = config.StreamType;
                txtNickname.Text = config.Nickname;
                txtGroups.Text = string.Join(", ", config.Groups);
                await ConnectCameraAsync(config.IPAddress, config.Username, config.Password, config.StreamType, pictureBox, connectMenuItem, disconnectMenuItem, recordStreamMenuItem, stopRecordingMenuItem, removeMenuItem, cameraPanel, configPanel);
            }

            configPanel.Controls.Add(lblIPAddress);
            configPanel.Controls.Add(txtIPAddress);
            configPanel.Controls.Add(lblUsername);
            configPanel.Controls.Add(txtUsername);
            configPanel.Controls.Add(lblPassword);
            configPanel.Controls.Add(txtPassword);
            configPanel.Controls.Add(lblStreamType);
            configPanel.Controls.Add(comboBoxStreamType);
            configPanel.Controls.Add(lblNickname);
            configPanel.Controls.Add(txtNickname);
            configPanel.Controls.Add(lblGroups);
            configPanel.Controls.Add(txtGroups);

            actionPanel.Controls.Add(btnActions);
            cameraPanel.Controls.Add(pictureBox);
            cameraGroupBox.Controls.Add(actionPanel);
            cameraGroupBox.Controls.Add(configPanel);
            cameraGroupBox.Controls.Add(cameraPanel);

            flowLayoutPanel.Controls.Add(cameraGroupBox);
        }

        private async Task ConnectCameraAsync(string ipAddress, string username, string password, string streamType, PictureBox pictureBox, ToolStripMenuItem btnConnect, ToolStripMenuItem btnDisconnect, ToolStripMenuItem btnSaveStream, ToolStripMenuItem btnStopRecording, ToolStripMenuItem btnRemove, Panel cameraPanel, GroupBox configPanel)
        {
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnSaveStream.Enabled = true;

            if (streamType == "MJPEG")
            {
                string url = $"http://{username}:{password}@{ipAddress}/mjpg/video.mjpg";
                var mjpegStream = new MJPEGStream(url);

                mjpegStream.NewFrame += (s, e) =>
                {
                    using (Bitmap originalBitmap = (Bitmap)e.Frame.Clone())
                    {
                        Bitmap resizedBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
                        using (Graphics g = Graphics.FromImage(resizedBitmap))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(originalBitmap, 0, 0, pictureBox.Width, pictureBox.Height);
                        }

                        pictureBox.Invoke(new Action(() =>
                        {
                            pictureBox.Image?.Dispose();
                            pictureBox.Image = resizedBitmap;
                        }));
                    }
                };

                mjpegStreams.Add(mjpegStream);
                await Task.Run(() => mjpegStream.Start());
            }

            HideConfigControls(configPanel, cameraPanel, pictureBox);
        }

        private void HideConfigControls(GroupBox configPanel, Panel cameraPanel, PictureBox pictureBox)
        {
            configPanel.Visible = false;
            pictureBox.Location = new Point(10, 10);
            cameraPanel.Dock = DockStyle.Fill;
        }

        private void RemoveCameraPanel(GroupBox cameraGroupBox)
        {
            flowLayoutPanel.Controls.Remove(cameraGroupBox);
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Tag = placeholder;
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                    textBox.UseSystemPasswordChar = placeholder == "Password";
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                    textBox.UseSystemPasswordChar = false;
                }
            };
        }

        private void OpenFullScreen(PictureBox pictureBox)
        {
            var parent = pictureBox.Parent;
            var originalLocation = pictureBox.Location;

            pictureBox.Tag = originalLocation;
            pictureBox.Parent.Controls.Remove(pictureBox);

            var fullScreenForm = new FullScreenForm(pictureBox);
            fullScreenForm.FormClosed += (s, e) =>
            {
                parent.Controls.Add(pictureBox);
                pictureBox.Location = originalLocation;
            };

            fullScreenForm.ShowDialog();
        }

        private void DisconnectAllCameras()
        {
            foreach (var stream in mjpegStreams)
            {
                stream.SignalToStop();
            }
            mjpegStreams.Clear();

            foreach (var videoWriter in videoWriters.Values)
            {
                CloseVideoWriter(videoWriter);
            }
            videoWriters.Clear();
        }

        private void DisconnectCamera(string ipAddress, ToolStripMenuItem btnConnect, ToolStripMenuItem btnDisconnect, ToolStripMenuItem btnSaveStream, ToolStripMenuItem btnStopRecording)
        {
            StopRecording(ipAddress, btnSaveStream, btnStopRecording);

            if (cameraButtons.ContainsKey(ipAddress))
            {
                var buttons = cameraButtons[ipAddress];
                buttons.btnConnect.Enabled = true;
                buttons.btnDisconnect.Enabled = false;
            }
        }

        private void SaveCameraConfigurations()
        {
            List<CameraConfig> existingConfigs = new List<CameraConfig>();

            // Load existing configurations
            if (File.Exists(ConfigFilePath))
            {
                XmlSerializer serializer1 = new XmlSerializer(typeof(List<CameraConfig>));
                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    existingConfigs = (List<CameraConfig>)serializer1.Deserialize(reader);
                }
            }

            // Collect new configurations from UI
            List<CameraConfig> newConfigs = new List<CameraConfig>();
            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    foreach (Control groupBoxControl in groupBox.Controls)
                    {
                        if (groupBoxControl is GroupBox panel && panel.Name == "configPanel")
                        {
                            var txtIPAddress = panel.Controls["txtIPAddress"] as TextBox;
                            var txtUsername = panel.Controls["txtUsername"] as TextBox;
                            var txtPassword = panel.Controls["txtPassword"] as TextBox;
                            var comboBoxStreamType = panel.Controls["comboBoxStreamType"] as ComboBox;
                            var txtNickname = panel.Controls["txtNickname"] as TextBox;
                            var txtGroups = panel.Controls["txtGroups"] as TextBox;
                            try
                            {
                                CameraConfig config = new CameraConfig
                                {
                                    IPAddress = txtIPAddress.Text,
                                    Username = txtUsername.Text,
                                    Password = txtPassword.Text,
                                    StreamType = comboBoxStreamType.SelectedItem?.ToString(),
                                    Nickname = txtNickname.Text,
                                    Groups = txtGroups.Text.Split(',').Select(g => g.Trim()).ToList()
                                };

                                newConfigs.Add(config);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error saving configuration: " + ex.Message);
                            }
                        }
                    }
                }
            }

            // Merge configurations
            foreach (var newConfig in newConfigs)
            {
                var existingConfig = existingConfigs.FirstOrDefault(c => c.IPAddress == newConfig.IPAddress);
                if (existingConfig != null)
                {
                    // Update existing configuration
                    existingConfig.Username = newConfig.Username;
                    existingConfig.Password = newConfig.Password;
                    existingConfig.StreamType = newConfig.StreamType;
                    existingConfig.Nickname = newConfig.Nickname;
                    existingConfig.Groups = newConfig.Groups;
                }
                else
                {
                    // Add new configuration
                    existingConfigs.Add(newConfig);
                }
            }

            // Save merged configurations back to the XML file
            XmlSerializer serializer = new XmlSerializer(typeof(List<CameraConfig>));
            using (StreamWriter writer = new StreamWriter(ConfigFilePath))
            {
                serializer.Serialize(writer, existingConfigs);
            }
        }


        private void TreeViewConfiguredCameras_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is CameraConfig config)
            {
                AddCameraPanelAsync(config);
            }
        }

        private void LoadCameraConfigurations()
        {
            if (File.Exists(ConfigFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CameraConfig>));
                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    List<CameraConfig> configs = (List<CameraConfig>)serializer.Deserialize(reader);
                    Dictionary<string, TreeNode> groupNodes = new Dictionary<string, TreeNode>();

                    foreach (var config in configs)
                    {
                        foreach (var group in config.Groups)
                        {
                            if (!groupNodes.ContainsKey(group))
                            {
                                var groupNode = new TreeNode(group);
                                treeViewConfiguredCameras.Nodes.Add(groupNode);
                                groupNodes[group] = groupNode;
                            }

                            var cameraNode = new TreeNode($"{config.Nickname} ({config.IPAddress})") { Tag = config };
                            groupNodes[group].Nodes.Add(cameraNode);
                        }
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCameraConfigurations();
            DisconnectAllCameras();
        }

        private void ShowAllCamerasFullScreen(object sender, EventArgs e)
        {
            var cameraPictureBoxes = flowLayoutPanel.Controls
                .OfType<GroupBox>()
                .SelectMany(groupBox => groupBox.Controls.OfType<Panel>())
                .SelectMany(panel => panel.Controls.OfType<PictureBox>())
                .ToList();

            if (cameraPictureBoxes.Any())
            {
                var fullScreenForm = new MultiCameraFullScreenForm(cameraPictureBoxes);
                fullScreenForm.ShowDialog();
            }
        }

        private void btnSaveRecords_Click(object sender, EventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CameraRecord>));
            using (StreamWriter writer = new StreamWriter(RecordsFilePath))
            {
                serializer.Serialize(writer, cameraRecords);
            }
        }

        private void LoadRecordsFromFile()
        {
            if (File.Exists(RecordsFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CameraRecord>));
                using (StreamReader reader = new StreamReader(RecordsFilePath))
                {
                    cameraRecords = (List<CameraRecord>)serializer.Deserialize(reader);
                }
            }
        }

        private void btnViewRecords_Click(object sender, EventArgs e)
        {
            var recordsForm = new RecordsForm(cameraRecords);
            recordsForm.ShowDialog();
        }

        private async void SaveStream(string ipAddress, string streamType, string username, string password, ToolStripMenuItem btnSaveStream, ToolStripMenuItem btnStopRecording)
        {
            if (streamType == "MJPEG")
            {
                string url = $"http://{username}:{password}@{ipAddress}/mjpg/video.mjpg";
                string outputPath = Path.Combine("D:\\Records\\" + ipAddress.Replace('.', '_').Replace(':', '_'), $"{Guid.NewGuid()}_output.avi");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                var mjpegStream = new MJPEGStream(url);
                var videoWriter = new VideoFileWriter();
                videoWriters[ipAddress] = videoWriter;

                try
                {
                    bool isFirstFrame = true;
                    mjpegStream.NewFrame += (s, e) =>
                    {
                        using (Bitmap frame = (Bitmap)e.Frame.Clone())
                        {
                            if (isFirstFrame)
                            {
                                videoWriter.Open(outputPath, frame.Width, frame.Height, 30, VideoCodec.MPEG4);
                                isFirstFrame = false;
                            }

                            using (Bitmap convertedFrame = ConvertToFormat(frame, PixelFormat.Format24bppRgb))
                            {
                                videoWriter.WriteVideoFrame(convertedFrame);
                            }
                        }
                    };

                    await Task.Run(() => mjpegStream.Start());
                    mjpegStreams.Add(mjpegStream);

                    cameraRecords.Add(new CameraRecord
                    {
                        IPAddress = ipAddress,
                        FilePath = outputPath,
                        RecordedAt = DateTime.Now
                    });

                    btnSaveStream.Enabled = false;
                    btnStopRecording.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving stream: {ex.Message}");
                    CloseVideoWriter(videoWriter);
                }
            }
        }

        private Bitmap ConvertToFormat(Bitmap original, PixelFormat format)
        {
            Bitmap converted = new Bitmap(original.Width, original.Height, format);
            using (Graphics g = Graphics.FromImage(converted))
            {
                g.DrawImage(original, new Rectangle(0, 0, converted.Width, converted.Height));
            }
            return converted;
        }

        private void CloseVideoWriter(VideoFileWriter videoWriter)
        {
            if (videoWriter != null && videoWriter.IsOpen)
            {
                videoWriter.Close();
                videoWriter.Dispose();
            }
        }

        private void StopRecording(string ipAddress, ToolStripMenuItem btnSaveStream, ToolStripMenuItem btnStopRecording)
        {
            if (videoWriters.ContainsKey(ipAddress))
            {
                var videoWriter = videoWriters[ipAddress];
                CloseVideoWriter(videoWriter);
                videoWriters.Remove(ipAddress);

                btnSaveStream.Enabled = true;
                btnStopRecording.Enabled = false;
            }
        }
    }

}
