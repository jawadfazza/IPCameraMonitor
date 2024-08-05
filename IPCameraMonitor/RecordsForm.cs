using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IPCameraMonitor
{
    public class RecordsForm : Form
    {
        private TreeView cameraTreeView;
        private DataGridView dataGridView;
        private List<CameraRecord> records;
        private Dictionary<string, List<CameraRecord>> cameraRecords;
        private TextBox filterTextBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public RecordsForm(List<CameraRecord> records)
        {
            this.records = records;
            cameraRecords = records.GroupBy(r => r.IPAddress)
                                   .ToDictionary(g => g.Key, g => g.ToList());
            InitializeComponents();
            this.Text = "Camera Records";
            this.WindowState = FormWindowState.Maximized;
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        private void InitializeComponents()
        {
            InitializeFilterTextBox();
            InitializeTreeView();
            InitializeDataGridView();
            InitializeStatusStrip();

            LayoutComponents();
        }

        private void InitializeFilterTextBox()
        {
            filterTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "Enter filter text..."
            };
            filterTextBox.GotFocus += (sender, e) =>
            {
                if (filterTextBox.Text == "Enter filter text...")
                {
                    filterTextBox.Text = "";
                    filterTextBox.ForeColor = Color.Black;
                    filterTextBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                }
            };
            filterTextBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(filterTextBox.Text))
                {
                    filterTextBox.ForeColor = Color.Gray;
                    filterTextBox.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                    filterTextBox.Text = "Enter filter text...";
                }
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
        }

        private void InitializeTreeView()
        {
            cameraTreeView = new TreeView
            {
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Width = 200
            };
            cameraTreeView.AfterSelect += CameraTreeView_AfterSelect;
        }

        private void InitializeDataGridView()
        {
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);
        }

        private void LayoutComponents()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 200
            };

            GroupBox treeViewGroupBox = new GroupBox
            {
                Text = "Cameras",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            treeViewGroupBox.Controls.Add(cameraTreeView);
            treeViewGroupBox.Controls.Add(filterTextBox);

            GroupBox dataGridViewGroupBox = new GroupBox
            {
                Text = "Records",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            dataGridViewGroupBox.Controls.Add(dataGridView);

            splitContainer.Panel1.Controls.Add(treeViewGroupBox);
            splitContainer.Panel2.Controls.Add(dataGridViewGroupBox);

            this.Controls.Add(splitContainer);
            this.Controls.Add(statusStrip);

            this.Load += RecordsForm_Load;
        }

        private void RecordsForm_Load(object sender, EventArgs e)
        {
            LoadCameraTree();
            UpdateStatusLabel();
        }

        private void LoadCameraTree()
        {
            foreach (var camera in cameraRecords.Keys)
            {
                TreeNode cameraNode = new TreeNode(camera);
                cameraTreeView.Nodes.Add(cameraNode);
            }
        }

        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            string filterText = filterTextBox.Text.ToLower();
            var filteredRecords = records.Where(r =>
                r.FilePath.ToLower().Contains(filterText) ||
                r.RecordedAt.ToString().ToLower().Contains(filterText)
            ).ToList();

            dataGridView.DataSource = new BindingSource { DataSource = filteredRecords };
            UpdateStatusLabel(filteredRecords.Count);
        }

        private void CameraTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedCamera = e.Node.Text;
            if (cameraRecords.ContainsKey(selectedCamera))
            {
                var selectedRecords = cameraRecords[selectedCamera];
                dataGridView.DataSource = new BindingSource { DataSource = selectedRecords };

                // Enable sorting for all columns
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                }
                UpdateStatusLabel(selectedRecords.Count);
            }
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRecord = dataGridView.Rows[e.RowIndex].DataBoundItem as CameraRecord;
                if (selectedRecord != null)
                {
                    if (File.Exists(selectedRecord.FilePath))
                    {
                        System.Diagnostics.Process.Start(selectedRecord.FilePath);
                    }
                    else
                    {
                        MessageBox.Show("File not found: " + selectedRecord.FilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateStatusLabel(int count = 0)
        {
            if (count == 0)
            {
                count = records.Count;
            }
            statusLabel.Text = $"Total Records: {count}";
        }
    }
}
