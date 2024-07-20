using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class MultiCameraFullScreenForm : Form
{
    private TableLayoutPanel tableLayoutPanel;
    private Dictionary<PictureBox, Control> originalParents;

    public MultiCameraFullScreenForm(List<PictureBox> cameraPictureBoxes)
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.Black;
        this.DoubleClick += (s, e) => this.Close();
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        };

        originalParents = new Dictionary<PictureBox, Control>();
        InitializeTableLayoutPanel(cameraPictureBoxes);
    }

    private void InitializeTableLayoutPanel(List<PictureBox> cameraPictureBoxes)
    {
        int cameraCount = cameraPictureBoxes.Count;
        int rows = (int)Math.Ceiling(Math.Sqrt(cameraCount));
        int columns = (int)Math.Ceiling((double)cameraCount / rows);

        tableLayoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            RowCount = rows,
            ColumnCount = columns,
        };

        // Adjust row and column styles to equally distribute space
        for (int i = 0; i < rows; i++)
        {
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));
        }

        for (int i = 0; i < columns; i++)
        {
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));
        }

        foreach (var pictureBox in cameraPictureBoxes)
        {
            originalParents[pictureBox] = pictureBox.Parent; // Store original parent

            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        for (int i = 0; i < cameraCount; i++)
        {
            int row = i / columns;
            int column = i % columns;
            tableLayoutPanel.Controls.Add(cameraPictureBoxes[i], column, row);
        }

        this.Controls.Add(tableLayoutPanel);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        foreach (var pictureBox in originalParents.Keys)
        {
            var originalParent = originalParents[pictureBox];
            originalParent.Controls.Add(pictureBox);
            pictureBox.Dock = DockStyle.Fill;
        }
    }
}
