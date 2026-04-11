using System.Windows.Forms.DataVisualization.Charting;

namespace Prueba_1_proyecto_vacaciones
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // ── Panel superior ─────────────────────────────────────────────
            pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 55;
            pnlTop.BackColor = Color.FromArgb(30, 30, 50);
            pnlTop.Padding = new Padding(10, 0, 10, 0);

            lblTitle = new Label();
            lblTitle.Text = "DATA FUSION ARENA";
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(12, 14);

            btnLoad = new Button();
            btnLoad.Text = "Cargar Datos";
            btnLoad.Location = new Point(280, 12);
            btnLoad.Size = new Size(130, 32);
            btnLoad.BackColor = Color.FromArgb(0, 120, 215);
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Click += btnLoad_Click;

            btnProcess = new Button();
            btnProcess.Text = "Procesar";
            btnProcess.Location = new Point(420, 12);
            btnProcess.Size = new Size(120, 32);
            btnProcess.BackColor = Color.FromArgb(16, 137, 62);
            btnProcess.ForeColor = Color.White;
            btnProcess.FlatStyle = FlatStyle.Flat;
            btnProcess.Click += btnProcess_Click;

            btnConsole = new Button();
            btnConsole.Text = "Ver Consola";
            btnConsole.Location = new Point(550, 12);
            btnConsole.Size = new Size(120, 32);
            btnConsole.BackColor = Color.FromArgb(104, 33, 122);
            btnConsole.ForeColor = Color.White;
            btnConsole.FlatStyle = FlatStyle.Flat;
            btnConsole.Click += btnConsole_Click;

            lblStatus = new Label();
            lblStatus.Text = "Listo.";
            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.AutoSize = true;
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.Location = new Point(900, 18);

            pnlTop.Controls.Add(lblStatus);
            pnlTop.Controls.Add(btnConsole);
            pnlTop.Controls.Add(btnProcess);
            pnlTop.Controls.Add(btnLoad);
            pnlTop.Controls.Add(lblTitle);

            // ── TabControl ─────────────────────────────────────────────────
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 9F);

            // Tab 1 – DataGridView
            tabData = new TabPage("Todos los Datos");

            pnlDataFilter = new Panel();
            pnlDataFilter.Dock = DockStyle.Top;
            pnlDataFilter.Height = 38;
            pnlDataFilter.BackColor = Color.FromArgb(245, 245, 250);
            pnlDataFilter.BorderStyle = BorderStyle.FixedSingle;

            lblFilter = new Label();
            lblFilter.Text = "Filtrar por fuente:";
            lblFilter.Location = new Point(8, 11);
            lblFilter.AutoSize = true;

            cmbSourceFilter = new ComboBox();
            cmbSourceFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSourceFilter.Location = new Point(130, 7);
            cmbSourceFilter.Size = new Size(120, 23);
            cmbSourceFilter.Items.AddRange(new object[] { "Todas", "CSV", "JSON", "XML", "TXT", "DB" });
            cmbSourceFilter.SelectedIndex = 0;
            cmbSourceFilter.SelectedIndexChanged += cmbSourceFilter_SelectedIndexChanged;

            lblRowCount = new Label();
            lblRowCount.Text = "0 registros";
            lblRowCount.Location = new Point(262, 11);
            lblRowCount.AutoSize = true;

            pnlDataFilter.Controls.Add(lblRowCount);
            pnlDataFilter.Controls.Add(cmbSourceFilter);
            pnlDataFilter.Controls.Add(lblFilter);

            dgvData = new DataGridView();
            dgvData.Dock = DockStyle.Fill;
            dgvData.ReadOnly = true;
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.None;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 50);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabData.Controls.Add(dgvData);
            tabData.Controls.Add(pnlDataFilter);

            // Tab 2 – Grafica de Barras (Laptops)
            tabBar = new TabPage("Barras - Laptops");
            chartBar = new Chart();
            chartBar.Dock = DockStyle.Fill;
            chartBar.BackColor = Color.White;
            chartBar.ChartAreas.Add(new ChartArea("Default"));
            tabBar.Controls.Add(chartBar);

            // Tab 3 – Grafica de Pastel (Videojuegos)
            tabPie = new TabPage("Pastel - Videojuegos");
            chartPie = new Chart();
            chartPie.Dock = DockStyle.Fill;
            chartPie.BackColor = Color.White;
            chartPie.ChartAreas.Add(new ChartArea("Default"));
            tabPie.Controls.Add(chartPie);

            // Tab 4 – Grafica de Anillo (Inventario)
            tabDoughnut = new TabPage("Anillo - Inventario");
            chartDoughnut = new Chart();
            chartDoughnut.Dock = DockStyle.Fill;
            chartDoughnut.BackColor = Color.White;
            chartDoughnut.ChartAreas.Add(new ChartArea("Default"));
            tabDoughnut.Controls.Add(chartDoughnut);

            // Tab 5 – Grafica de Lineas (Temperatura)
            tabLine = new TabPage("Lineas - Temperatura");
            chartLine = new Chart();
            chartLine.Dock = DockStyle.Fill;
            chartLine.BackColor = Color.White;
            chartLine.ChartAreas.Add(new ChartArea("Default"));
            tabLine.Controls.Add(chartLine);

            // Tab 6 – Consola ASCII
            tabConsole = new TabPage("Consola ASCII");
            rtbConsole = new RichTextBox();
            rtbConsole.Dock = DockStyle.Fill;
            rtbConsole.Font = new Font("Consolas", 9F);
            rtbConsole.ReadOnly = true;
            rtbConsole.BackColor = Color.FromArgb(20, 20, 30);
            rtbConsole.ForeColor = Color.LightGreen;
            rtbConsole.BorderStyle = BorderStyle.None;
            rtbConsole.WordWrap = false;
            tabConsole.Controls.Add(rtbConsole);

            tabControl.TabPages.Add(tabData);
            tabControl.TabPages.Add(tabBar);
            tabControl.TabPages.Add(tabPie);
            tabControl.TabPages.Add(tabDoughnut);
            tabControl.TabPages.Add(tabLine);
            tabControl.TabPages.Add(tabConsole);

            // ── Panel de importacion de archivos ───────────────────────────
            pnlFiles = new Panel();
            pnlFiles.Dock = DockStyle.Top;
            pnlFiles.Height = 45;
            pnlFiles.BackColor = Color.FromArgb(45, 45, 68);

            lblImport = new Label();
            lblImport.Text = "Importar archivo:";
            lblImport.ForeColor = Color.LightGray;
            lblImport.Font = new Font("Segoe UI", 9F);
            lblImport.AutoSize = true;
            lblImport.Location = new Point(12, 13);

            btnImportCsv = new Button();
            btnImportCsv.Text = "CSV";
            btnImportCsv.Location = new Point(140, 8);
            btnImportCsv.Size = new Size(75, 28);
            btnImportCsv.BackColor = Color.FromArgb(0, 153, 76);
            btnImportCsv.ForeColor = Color.White;
            btnImportCsv.FlatStyle = FlatStyle.Flat;
            btnImportCsv.Click += btnImportCsv_Click;

            btnImportJson = new Button();
            btnImportJson.Text = "JSON";
            btnImportJson.Location = new Point(225, 8);
            btnImportJson.Size = new Size(75, 28);
            btnImportJson.BackColor = Color.FromArgb(200, 100, 0);
            btnImportJson.ForeColor = Color.White;
            btnImportJson.FlatStyle = FlatStyle.Flat;
            btnImportJson.Click += btnImportJson_Click;

            btnImportXml = new Button();
            btnImportXml.Text = "XML";
            btnImportXml.Location = new Point(310, 8);
            btnImportXml.Size = new Size(75, 28);
            btnImportXml.BackColor = Color.FromArgb(0, 100, 180);
            btnImportXml.ForeColor = Color.White;
            btnImportXml.FlatStyle = FlatStyle.Flat;
            btnImportXml.Click += btnImportXml_Click;

            btnImportTxt = new Button();
            btnImportTxt.Text = "TXT";
            btnImportTxt.Location = new Point(395, 8);
            btnImportTxt.Size = new Size(75, 28);
            btnImportTxt.BackColor = Color.FromArgb(130, 60, 160);
            btnImportTxt.ForeColor = Color.White;
            btnImportTxt.FlatStyle = FlatStyle.Flat;
            btnImportTxt.Click += btnImportTxt_Click;

            btnClearData = new Button();
            btnClearData.Text = "Limpiar Todo";
            btnClearData.Location = new Point(490, 8);
            btnClearData.Size = new Size(110, 28);
            btnClearData.BackColor = Color.FromArgb(180, 30, 30);
            btnClearData.ForeColor = Color.White;
            btnClearData.FlatStyle = FlatStyle.Flat;
            btnClearData.Click += btnClearData_Click;

            pnlFiles.Controls.Add(btnClearData);
            pnlFiles.Controls.Add(btnImportTxt);
            pnlFiles.Controls.Add(btnImportXml);
            pnlFiles.Controls.Add(btnImportJson);
            pnlFiles.Controls.Add(btnImportCsv);
            pnlFiles.Controls.Add(lblImport);

            // ── Form ───────────────────────────────────────────────────────
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1280, 780);
            this.Text = "Data Fusion Arena";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Fill primero, luego los Top de abajo hacia arriba
            this.Controls.Add(tabControl);
            this.Controls.Add(pnlFiles);
            this.Controls.Add(pnlTop);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ── Declaracion de controles ───────────────────────────────────────
        private Panel pnlTop;
        private Panel pnlFiles;
        private Label lblTitle;
        private Label lblImport;
        private Button btnLoad;
        private Button btnProcess;
        private Button btnConsole;
        private Button btnImportCsv;
        private Button btnImportJson;
        private Button btnImportXml;
        private Button btnImportTxt;
        private Button btnClearData;
        private Label lblStatus;

        private TabControl tabControl;
        private TabPage tabData;
        private TabPage tabBar;
        private TabPage tabPie;
        private TabPage tabDoughnut;
        private TabPage tabLine;
        private TabPage tabConsole;

        private Panel pnlDataFilter;
        private Label lblFilter;
        private ComboBox cmbSourceFilter;
        private Label lblRowCount;
        private DataGridView dgvData;
        private Chart chartBar;
        private Chart chartPie;
        private Chart chartDoughnut;
        private Chart chartLine;
        private RichTextBox rtbConsole;
    }
}
