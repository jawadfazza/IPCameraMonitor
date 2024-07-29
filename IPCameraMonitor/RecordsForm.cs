using System;
using System.Collections.Generic;
using System.Drawing;
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
        }

        private void InitializeComponents()
        {
            filterTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                //PlaceholderText = "Enter filter text..."
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;

            cameraTreeView = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Width = 200
            };
            cameraTreeView.AfterSelect += CameraTreeView_AfterSelect;

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Width = 700
            };
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;

            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);

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
                Padding = new Padding(10),
                Width = 700
            };
            dataGridViewGroupBox.Controls.Add(dataGridView);

            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 10,

            };

            splitContainer.Panel1.Controls.Add(treeViewGroupBox);
            splitContainer.Panel2.Controls.Add(dataGridViewGroupBox);
            splitContainer.Panel2.Controls.Add(statusStrip);

            this.Controls.Add(splitContainer);
            this.Load += RecordsForm_Load;

            // Set Form properties
            this.Text = "Camera Records";
            this.WindowState = FormWindowState.Maximized;
            this.Font = new Font("Arial", 10, FontStyle.Regular);
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
                    System.Diagnostics.Process.Start(selectedRecord.FilePath);
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
