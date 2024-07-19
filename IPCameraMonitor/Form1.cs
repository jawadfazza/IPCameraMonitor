using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using AForge.Video;
using FFmpeg.AutoGen;

namespace IPCameraMonitor
{
    public partial class Form1 : Form
    {
        private List<MJPEGStream> mjpegStreams = new List<MJPEGStream>();
        private List<RTSPStreamHandler> rtspStreamHandlers = new List<RTSPStreamHandler>();
        private const int MaxCameras = 10; // Set a reasonable limit based on system capacity
        private const string ConfigFilePath = "CameraConfig.xml";
        private Dictionary<string, (Button btnConnect, Button btnDisconnect)> cameraButtons = new Dictionary<string, (Button, Button)>();

        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized; // Open Form1 in full screen
            FFmpegHelper.Initialize();
            LoadCameraConfigurations();

        }

        private void btnAddCamera_Click(object sender, EventArgs e)
        {

            AddCameraPanel();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the FlowLayoutPanel
           // flowLayoutPanel.AutoScroll = true;
            //flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            //flowLayoutPanel.WrapContents = false;
            //flowLayoutPanel.Dock = DockStyle.Fill;
        }
        private void AddCameraPanel(CameraConfig config = null)
        {
            // Get the system resolution
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Calculate the width for each GroupBox so that 4 can fit in one row
            int groupBoxWidth = (screenWidth - 100) / 4;

            GroupBox cameraGroupBox = new GroupBox
            {
                Size = new Size(groupBoxWidth, 400), // Adjusted to fit 4 in a row within the flowLayoutPanel
                Padding = new Padding(10),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Navy,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(10) // Added margin to separate group boxes
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
                Size = new Size(groupBoxWidth - 40, 150),
                Text = "Camera Configuration",
                Padding = new Padding(10),
                Dock = DockStyle.Top
            };

            // Center the configPanel
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

            //var rtspStreamHandler = new RTSPStreamHandler("D:\\Mostakel\\New folder\\"+ config.IPAddress + "_output.mp4", pictureBox );
            //rtspStreamHandler.Start();


            Label lblIPAddress = new Label { Text = "IP Address", Location = new Point(10, 20), AutoSize = true };
            TextBox txtIPAddress = new TextBox { Name = "txtIPAddress", Size = new Size(250, 20), Location = new Point(100, 20) };

            Label lblUsername = new Label { Text = "Username", Location = new Point(10, 50), AutoSize = true };
            TextBox txtUsername = new TextBox { Name = "txtUsername", Size = new Size(250, 20), Location = new Point(100, 50) };

            Label lblPassword = new Label { Text = "Password", Location = new Point(10, 80), AutoSize = true };
            TextBox txtPassword = new TextBox { Name = "txtPassword", Size = new Size(250, 20), Location = new Point(100, 80), UseSystemPasswordChar = true };

            Label lblStreamType = new Label { Text = "Stream Type", Location = new Point(10, 110), AutoSize = true };
            ComboBox comboBoxStreamType = new ComboBox { Name = "comboBoxStreamType", Location = new Point(100, 110), Size = new Size(250, 21) };
            comboBoxStreamType.Items.AddRange(new object[] { "MJPEG", "RTSP" });
            comboBoxStreamType.SelectedIndex = 0;

            Button btnConnect = new Button { Text = "Connect", Size = new Size(100, 25), Location = new Point(10, 5) };
            Button btnDisconnect = new Button { Text = "Disconnect", Size = new Size(100, 25), Location = new Point(120, 5) };

            Button btnDelete = new Button { Text = "Delete", Size = new Size(100, 25), Location = new Point(230, 5) };
            btnDelete.Click += (s, e) => DeleteCameraPanel(cameraGroupBox);

            btnConnect.Click += (s, e) => ConnectCamera(txtIPAddress.Text, txtUsername.Text, txtPassword.Text, comboBoxStreamType.SelectedItem.ToString(), pictureBox, btnConnect, btnDisconnect, btnDelete, cameraPanel, configPanel);
            btnDisconnect.Click += (s, e) => DisconnectCamera(txtIPAddress.Text, btnConnect,btnDisconnect);

            if (config != null)
            {
                cameraButtons.Add(config.IPAddress, (btnConnect, btnDisconnect));
            }
                SetPlaceholder(txtIPAddress, "IP Address");
                SetPlaceholder(txtUsername, "Username");
                SetPlaceholder(txtPassword, "Password");
            

            if (config != null)
            {
                txtIPAddress.Text = config.IPAddress;
                txtUsername.Text = config.Username;
                txtPassword.Text = config.Password;
                comboBoxStreamType.SelectedItem = config.StreamType;
                ConnectCamera(config.IPAddress, config.Username, config.Password, config.StreamType, pictureBox, btnConnect, btnDisconnect, btnDelete, cameraPanel, configPanel);
            }

            configPanel.Controls.Add(lblIPAddress);
            configPanel.Controls.Add(txtIPAddress);
            configPanel.Controls.Add(lblUsername);
            configPanel.Controls.Add(txtUsername);
            configPanel.Controls.Add(lblPassword);
            configPanel.Controls.Add(txtPassword);
            configPanel.Controls.Add(lblStreamType);
            configPanel.Controls.Add(comboBoxStreamType);

            actionPanel.Controls.Add(btnConnect);
            actionPanel.Controls.Add(btnDisconnect);
            actionPanel.Controls.Add(btnDelete);

            cameraPanel.Controls.Add(pictureBox);

            cameraGroupBox.Controls.Add(actionPanel);
            cameraGroupBox.Controls.Add(configPanel);
            cameraGroupBox.Controls.Add(cameraPanel);

            flowLayoutPanel.Controls.Add(cameraGroupBox);
        }


        private void ConnectCamera(string ipAddress, string username, string password, string streamType, PictureBox pictureBox, Button btnConnect, Button btnDisconnect, Button btnDelete, Panel cameraPanel, GroupBox configPanel)
        {
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnConnect.Text = "Connecting...";
            if (streamType == "MJPEG")
            {
                string url = $"http://{username}:{password}@{ipAddress}/mjpg/video.mjpg";
                var mjpegStream = new MJPEGStream(url);
                mjpegStream.NewFrame += (s, e) =>
                {
                    Bitmap originalBitmap = (Bitmap)e.Frame.Clone();
                    Bitmap resizedBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

                    using (Graphics g = Graphics.FromImage(resizedBitmap))
                    {
                        g.DrawImage(originalBitmap, 0, 0, pictureBox.Width, pictureBox.Height);
                    }

                    pictureBox.Image = resizedBitmap;
                    originalBitmap.Dispose();
                };
                mjpegStreams.Add(mjpegStream);
                mjpegStream.Start();
            }
            else if (streamType == "RTSP")
            {
                string url = $"rtsp://{username}:{password}@{ipAddress}/stream";
                var rtspStreamHandler = new RTSPStreamHandler(url, pictureBox);
                rtspStreamHandlers.Add(rtspStreamHandler);
                rtspStreamHandler.Start();
            }

            // Hide configuration controls and resize PictureBox
            HideConfigControls(configPanel, cameraPanel, pictureBox);
           
        }

        private void HideConfigControls(GroupBox configPanel,Panel cameraPanel, PictureBox pictureBox)
        {
            configPanel.Visible = false;
            pictureBox.Location = new Point(10, 10);
            cameraPanel.Dock=DockStyle.Fill;
        }

        private void DeleteCameraPanel(GroupBox cameraGroupBox)
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
                stream.WaitForStop();
            }
            mjpegStreams.Clear();

            foreach (var handler in rtspStreamHandlers)
            {
                handler.Stop();
            }
            rtspStreamHandlers.Clear();
        }

        private void DisconnectCamera(string ipAddress, Button btnConnect, Button btnDisconnect)
        {
            // Stop the MJPEG stream associated with the provided IP address
            var mjpegStream = mjpegStreams.FirstOrDefault(stream => stream.Source.ToString().Contains(ipAddress));
            if (mjpegStream != null)
            {
                mjpegStream.SignalToStop();
                mjpegStream.WaitForStop();
                mjpegStreams.Remove(mjpegStream);
            }

            // Stop the RTSP stream handler associated with the provided IP address
            //var rtspStreamHandler = rtspStreamHandlers.FirstOrDefault(handler => handler.so.Contains(ipAddress));
            //if (rtspStreamHandler != null)
            //{
            //    rtspStreamHandler.Stop();
            //    rtspStreamHandlers.Remove(rtspStreamHandler);
            //}


            if (cameraButtons.ContainsKey(ipAddress))
            {
                var buttons = cameraButtons[ipAddress];
                buttons.btnConnect.Enabled = true;
                buttons.btnDisconnect.Enabled = false;
            }
        }


        private void SaveCameraConfigurations()
        {
            List<CameraConfig> configs = new List<CameraConfig>();

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

                           
                                try
                                {
                                    CameraConfig config = new CameraConfig
                                    {
                                        IPAddress = txtIPAddress.Text,
                                        Username = txtUsername.Text,
                                        Password = txtPassword.Text,
                                        StreamType = comboBoxStreamType.SelectedItem?.ToString()
                                    };

                                    configs.Add(config);
                                }
                                catch (Exception ex)
                                {
                                    // Log or handle exceptions if necessary
                                    MessageBox.Show("Error saving configuration: " + ex.Message);
                                }
                            }
                        
                    }
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<CameraConfig>));
            using (StreamWriter writer = new StreamWriter(ConfigFilePath))
            {
                serializer.Serialize(writer, configs);
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
                    foreach (var config in configs)
                    {
                        AddCameraPanel(config);
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCameraConfigurations();
            DisconnectAllCameras();
        }

       
    }
}
