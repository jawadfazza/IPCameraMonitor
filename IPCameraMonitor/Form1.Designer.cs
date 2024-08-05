using System.Windows.Forms;

namespace IPCameraMonitor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.miStartRecording = new System.Windows.Forms.ToolStripMenuItem();
            this.miStopRecording = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewRecords = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFullScreenAll = new System.Windows.Forms.ToolStripButton();
            this.redCircle = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.btnMinimize = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeViewConfiguredCameras = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.statusStrip1.ForeColor = System.Drawing.Color.Black;
            this.statusStrip1.Location = new System.Drawing.Point(0, 729);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1371, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "Ready";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(85)))), ((int)(((byte)(170)))));
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.btnFullScreenAll,
            this.redCircle,
            this.btnClose,
            this.btnMinimize});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1371, 27);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCameraToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripDropDownButton2.ForeColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(51, 24);
            this.toolStripDropDownButton2.Text = "Files";
            // 
            // newCameraToolStripMenuItem
            // 
            this.newCameraToolStripMenuItem.Name = "newCameraToolStripMenuItem";
            this.newCameraToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newCameraToolStripMenuItem.Size = new System.Drawing.Size(216, 24);
            this.newCameraToolStripMenuItem.Text = "New Camera";
            this.newCameraToolStripMenuItem.Click += new System.EventHandler(this.btnAddCamera_Click);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(216, 24);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(216, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miStartRecording,
            this.miStopRecording,
            this.btnViewRecords});
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(75, 24);
            this.toolStripDropDownButton1.Text = "Records";
            // 
            // miStartRecording
            // 
            this.miStartRecording.Name = "miStartRecording";
            this.miStartRecording.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.miStartRecording.Size = new System.Drawing.Size(232, 24);
            this.miStartRecording.Text = "Start Recording";
            this.miStartRecording.Click += new System.EventHandler(this.miStartRecording_Click);
            // 
            // miStopRecording
            // 
            this.miStopRecording.Name = "miStopRecording";
            this.miStopRecording.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miStopRecording.Size = new System.Drawing.Size(232, 24);
            this.miStopRecording.Text = "Stop Recording";
            this.miStopRecording.Click += new System.EventHandler(this.miStopRecording_Click);
            // 
            // btnViewRecords
            // 
            this.btnViewRecords.Name = "btnViewRecords";
            this.btnViewRecords.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.btnViewRecords.Size = new System.Drawing.Size(232, 24);
            this.btnViewRecords.Text = "View Records";
            this.btnViewRecords.Click += new System.EventHandler(this.btnViewRecords_Click);
            // 
            // btnFullScreenAll
            // 
            this.btnFullScreenAll.ForeColor = System.Drawing.Color.Black;
            this.btnFullScreenAll.Image = global::IPCameraMonitor.Properties.Resources._5642608;
            this.btnFullScreenAll.Name = "btnFullScreenAll";
            this.btnFullScreenAll.Size = new System.Drawing.Size(100, 24);
            this.btnFullScreenAll.Text = "Full Screen";
            this.btnFullScreenAll.Click += new System.EventHandler(this.ShowAllCamerasFullScreen);
            // 
            // redCircle
            // 
            this.redCircle.BackColor = System.Drawing.Color.Transparent;
            this.redCircle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.redCircle.ForeColor = System.Drawing.SystemColors.WindowText;
            this.redCircle.Image = ((System.Drawing.Image)(resources.GetObject("redCircle.Image")));
            this.redCircle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redCircle.Name = "redCircle";
            this.redCircle.Size = new System.Drawing.Size(97, 24);
            this.redCircle.Text = "Recording";
            this.redCircle.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(23, 24);
            this.btnClose.Text = "toolStripButton1";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnMinimize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(23, 24);
            this.btnMinimize.Text = "toolStripButton2";
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.panel1.Controls.Add(this.treeViewConfiguredCameras);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 702);
            this.panel1.TabIndex = 5;
            // 
            // treeViewConfiguredCameras
            // 
            this.treeViewConfiguredCameras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.treeViewConfiguredCameras.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewConfiguredCameras.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.treeViewConfiguredCameras.ForeColor = System.Drawing.Color.Black;
            this.treeViewConfiguredCameras.ImageIndex = 0;
            this.treeViewConfiguredCameras.ImageList = this.imageList1;
            this.treeViewConfiguredCameras.Location = new System.Drawing.Point(0, 0);
            this.treeViewConfiguredCameras.Name = "treeViewConfiguredCameras";
            this.treeViewConfiguredCameras.SelectedImageIndex = 0;
            this.treeViewConfiguredCameras.Size = new System.Drawing.Size(300, 702);
            this.treeViewConfiguredCameras.TabIndex = 0;
            this.treeViewConfiguredCameras.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewConfiguredCameras_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "camera_Icon.png");
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(300, 27);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(1071, 702);
            this.flowLayoutPanel.TabIndex = 6;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1371, 751);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IP Camera Monitor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnFullScreenAll;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem miStartRecording;
        private System.Windows.Forms.ToolStripMenuItem btnViewRecords;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.TreeView treeViewConfiguredCameras;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miStopRecording;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripButton btnClose;
        private ToolStripButton btnMinimize;
        private ToolStripButton redCircle;
    }
}
