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

        pictureBox.Dock = DockStyle.Fill;
        this.Controls.Add(pictureBox);

        this.DoubleClick += (s, e) => this.Close();
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        };
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        this.Controls.Remove(pictureBox);
        pictureBox.Dock = DockStyle.None;
        pictureBox.Parent.Controls.Add(pictureBox);
        pictureBox.Location = (Point)pictureBox.Tag; // Restore original location
    }
}
