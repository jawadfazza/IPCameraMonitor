namespace IPCameraMonitor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddCamera = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(800, 400);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // btnAddCamera
            // 
            this.btnAddCamera.Location = new System.Drawing.Point(12, 410);
            this.btnAddCamera.Name = "btnAddCamera";
            this.btnAddCamera.Size = new System.Drawing.Size(100, 23);
            this.btnAddCamera.TabIndex = 1;
            this.btnAddCamera.Text = "Add Camera";
            this.btnAddCamera.UseVisualStyleBackColor = true;
            this.btnAddCamera.Click += new System.EventHandler(this.btnAddCamera_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnAddCamera);
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "Form1";
            this.Text = "IP Camera Monitor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Button btnAddCamera;
    }
}
