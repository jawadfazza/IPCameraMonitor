using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using FFmpeg.AutoGen;

namespace IPCameraMonitor
{
    public partial class Form1 : Form
    {
        private List<MJPEGStream> mjpegStreams = new List<MJPEGStream>();
        private List<RTSPStreamHandler> rtspStreamHandlers = new List<RTSPStreamHandler>();
        private const int MaxCameras = 10; // Set a reasonable limit based on system capacity

        public Form1()
        {
            InitializeComponent();
            //FFmpegHelper.LoadFFmpegLibraries();
            FFmpegHelper.Initialize();
        }

        private void btnAddCamera_Click(object sender, EventArgs e)
        {
            if (mjpegStreams.Count + rtspStreamHandlers.Count < MaxCameras)
            {
                AddCameraPanel();
            }
            else
            {
                MessageBox.Show("Maximum number of cameras reached. Please remove a camera before adding another.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddCameraPanel()
        {
            Panel cameraPanel = new Panel
            {
                Size = new Size(380, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            TextBox txtIPAddress = new TextBox { Size = new Size(150, 20), Location = new Point(10, 10) };
            TextBox txtUsername = new TextBox { Size = new Size(150, 20), Location = new Point(10, 40) };
            TextBox txtPassword = new TextBox { Size = new Size(150, 20), Location = new Point(10, 70), UseSystemPasswordChar = true };
            ComboBox comboBoxStreamType = new ComboBox { Location = new Point(10, 100), Size = new Size(150, 21) };
            comboBoxStreamType.Items.AddRange(new object[] { "MJPEG", "RTSP" });
            comboBoxStreamType.SelectedIndex = 0;
            PictureBox pictureBox = new PictureBox { Location = new Point(170, 10), Size = new Size(200, 150), BorderStyle = BorderStyle.FixedSingle };
            pictureBox.DoubleClick += (s, e) => OpenFullScreen(pictureBox);

            Button btnConnect = new Button { Text = "Connect", Size = new Size(75, 23), Location = new Point(10, 130) };
            Button btnDisconnect = new Button { Text = "Disconnect", Size = new Size(75, 23), Location = new Point(90, 130) };

            btnConnect.Click += (s, e) => ConnectCamera(txtIPAddress.Text, txtUsername.Text, txtPassword.Text, comboBoxStreamType.SelectedItem.ToString(), pictureBox);
            btnDisconnect.Click += (s, e) => DisconnectCamera(txtIPAddress.Text);

            SetPlaceholder(txtIPAddress, "IP Address");
            SetPlaceholder(txtUsername, "Username");
            SetPlaceholder(txtPassword, "Password");

            cameraPanel.Controls.Add(txtIPAddress);
            cameraPanel.Controls.Add(txtUsername);
            cameraPanel.Controls.Add(txtPassword);
            cameraPanel.Controls.Add(comboBoxStreamType);
            cameraPanel.Controls.Add(pictureBox);
            cameraPanel.Controls.Add(btnConnect);
            cameraPanel.Controls.Add(btnDisconnect);

            flowLayoutPanel.Controls.Add(cameraPanel);
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

        private void ConnectCamera(string ipAddress, string username, string password, string streamType, PictureBox pictureBox)
        {
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
        }

        private void DisconnectCamera(string ipAddress)
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectCamera(null);
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
    }
}
