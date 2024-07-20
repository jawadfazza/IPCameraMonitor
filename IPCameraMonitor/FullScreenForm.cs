using System;
using System.Drawing;
using System.Windows.Forms;

public class FullScreenForm : Form
{
    private PictureBox pictureBox;
    private Form originalParent;
    private Point originalLocation;
    private Size originalSize;

    public FullScreenForm(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.Black;

        originalParent = pictureBox.Parent as Form;
        originalLocation = pictureBox.Location;
        originalSize = pictureBox.Size;

        pictureBox.Dock = DockStyle.Fill; // Dock the PictureBox to fill the form
        this.Controls.Add(pictureBox);

        this.DoubleClick += (s, e) => ToggleFullscreen(); // Toggle fullscreen on double-click
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); // Close the form on Escape key press
            }
        };

        // Add a smooth transition
        this.Load += (s, e) => SmoothTransition(true);
        this.FormClosed += (s, e) => SmoothTransition(false);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        this.Controls.Remove(pictureBox); // Remove the PictureBox from the form
        pictureBox.Dock = DockStyle.Fill; // Reset the dock style
        pictureBox.Parent.Controls.Add(pictureBox); // Add the PictureBox back to its original parent
        pictureBox.Location = (Point)pictureBox.Tag; // Restore the original location
    }

    private void ToggleFullscreen()
    {
        if (this.FormBorderStyle == FormBorderStyle.None)
        {
            // Exit fullscreen
            this.Close();
        }
        else
        {
            // Enter fullscreen
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            pictureBox.Dock = DockStyle.Fill;
        }
    }

    private void SmoothTransition(bool entering)
    {
        var timer = new Timer { Interval = 1 };
        var opacityIncrement = entering ? 0.05 : -0.05;

        timer.Tick += (s, e) =>
        {
            this.Opacity += opacityIncrement;
            if (this.Opacity <= 0 || this.Opacity >= 1)
            {
                timer.Stop();
                timer.Dispose();
            }
        };

        this.Opacity = entering ? 0 : 1;
        timer.Start();
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // FullScreenForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FullScreenForm";
            this.ResumeLayout(false);

    }
}
