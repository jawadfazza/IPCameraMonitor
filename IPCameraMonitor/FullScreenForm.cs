using System;
using System.Drawing;
using System.Windows.Forms;

public class FullScreenForm : Form
{
    private PictureBox pictureBox;

    public FullScreenForm(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.Black;

        pictureBox.Dock = DockStyle.Fill; // Dock the PictureBox to fill the form
        this.Controls.Add(pictureBox);

        this.DoubleClick += (s, e) => this.Close(); // Close the form on double-click
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); // Close the form on Escape key press
            }
        };
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        this.Controls.Remove(pictureBox); // Remove the PictureBox from the form
        pictureBox.Dock = DockStyle.Fill; // Reset the dock style
        pictureBox.Parent.Controls.Add(pictureBox); // Add the PictureBox back to its original parent
        pictureBox.Location = (Point)pictureBox.Tag; // Restore the original location
    }
}
