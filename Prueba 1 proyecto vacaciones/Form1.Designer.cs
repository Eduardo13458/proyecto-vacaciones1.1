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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlTop = new Panel();
            lblStatus = new Label();
            btnConsole = new Button();
            btnProcess = new Button();
            lblTitle = new Label();
            tabControl = new TabControl();
            tabData = new TabPage();
            dgvData = new DataGridView();
            pnlDataFilter = new Panel();
            lblRowCount = new Label();
            cmbSourceFilter = new ComboBox();
            lblFilter = new Label();
            tabConsole = new TabPage();
            rtbConsole = new RichTextBox();
            tabAutoChart = new TabPage();
            tlpAutoCharts = new TableLayoutPanel();
            pnlFiles = new Panel();
            btnChartConsole = new Button();
            btnGenerateChart = new Button();
            btnClearData = new Button();
            btnImportPostgre = new Button();
            btnImportMariaDb = new Button();
            btnImportSqlServer = new Button();
            btnImportTxt = new Button();
            btnImportXml = new Button();
            btnImportJson = new Button();
            btnImportCsv = new Button();
            lblImport = new Label();
            pnlExport = new Panel();
            btnExportPostgre = new Button();
            btnExportMariaDb = new Button();
            btnExportSql = new Button();
            lblExport = new Label();
            pnlTop.SuspendLayout();
            tabControl.SuspendLayout();
            tabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvData).BeginInit();
            pnlDataFilter.SuspendLayout();
            tabConsole.SuspendLayout();
            tabAutoChart.SuspendLayout();
            pnlFiles.SuspendLayout();
            pnlExport.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(30, 30, 50);
            pnlTop.Controls.Add(lblStatus);
            pnlTop.Controls.Add(btnConsole);
            pnlTop.Controls.Add(btnProcess);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(10, 0, 10, 0);
            pnlTop.Size = new Size(1377, 55);
            pnlTop.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Location = new Point(2077, 18);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(43, 20);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Listo.";
            // 
            // btnConsole
            // 
            btnConsole.BackColor = Color.FromArgb(104, 33, 122);
            btnConsole.FlatStyle = FlatStyle.Flat;
            btnConsole.ForeColor = Color.White;
            btnConsole.Location = new Point(410, 12);
            btnConsole.Name = "btnConsole";
            btnConsole.Size = new Size(120, 32);
            btnConsole.TabIndex = 1;
            btnConsole.Text = "Ver Consola";
            btnConsole.UseVisualStyleBackColor = false;
            btnConsole.Click += btnConsole_Click;
            // 
            // btnProcess
            // 
            btnProcess.BackColor = Color.FromArgb(16, 137, 62);
            btnProcess.FlatStyle = FlatStyle.Flat;
            btnProcess.ForeColor = Color.White;
            btnProcess.Location = new Point(280, 12);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(120, 32);
            btnProcess.TabIndex = 2;
            btnProcess.Text = "Procesar";
            btnProcess.UseVisualStyleBackColor = false;
            btnProcess.Click += btnProcess_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(12, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(259, 32);
            lblTitle.TabIndex = 4;
            lblTitle.Text = "DATA FUSION ARENA";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabData);
            tabControl.Controls.Add(tabConsole);
            tabControl.Controls.Add(tabAutoChart);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 9F);
            tabControl.Location = new Point(0, 142);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1377, 638);
            tabControl.TabIndex = 0;
            // 
            // tabData
            // 
            tabData.Controls.Add(dgvData);
            tabData.Controls.Add(pnlDataFilter);
            tabData.Location = new Point(4, 29);
            tabData.Name = "tabData";
            tabData.Size = new Size(1369, 605);
            tabData.TabIndex = 0;
            tabData.Text = "Todos los Datos";
            // 
            // dgvData
            // 
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(30, 30, 50);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvData.ColumnHeadersHeight = 29;
            dgvData.Dock = DockStyle.Fill;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.Location = new Point(0, 38);
            dgvData.Name = "dgvData";
            dgvData.ReadOnly = true;
            dgvData.RowHeadersWidth = 51;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.Size = new Size(1369, 567);
            dgvData.TabIndex = 0;
            // 
            // pnlDataFilter
            // 
            pnlDataFilter.BackColor = Color.FromArgb(245, 245, 250);
            pnlDataFilter.BorderStyle = BorderStyle.FixedSingle;
            pnlDataFilter.Controls.Add(lblRowCount);
            pnlDataFilter.Controls.Add(cmbSourceFilter);
            pnlDataFilter.Controls.Add(lblFilter);
            pnlDataFilter.Dock = DockStyle.Top;
            pnlDataFilter.Location = new Point(0, 0);
            pnlDataFilter.Name = "pnlDataFilter";
            pnlDataFilter.Size = new Size(1369, 38);
            pnlDataFilter.TabIndex = 1;
            // 
            // lblRowCount
            // 
            lblRowCount.AutoSize = true;
            lblRowCount.Location = new Point(262, 11);
            lblRowCount.Name = "lblRowCount";
            lblRowCount.Size = new Size(78, 20);
            lblRowCount.TabIndex = 0;
            lblRowCount.Text = "0 registros";
            // 
            // cmbSourceFilter
            // 
            cmbSourceFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSourceFilter.Items.AddRange(new object[] { "Todas", "CSV", "JSON", "XML", "TXT", "DB" });
            cmbSourceFilter.Location = new Point(130, 7);
            cmbSourceFilter.Name = "cmbSourceFilter";
            cmbSourceFilter.Size = new Size(120, 28);
            cmbSourceFilter.TabIndex = 1;
            cmbSourceFilter.SelectedIndexChanged += cmbSourceFilter_SelectedIndexChanged;
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(8, 11);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(123, 20);
            lblFilter.TabIndex = 2;
            lblFilter.Text = "Filtrar por fuente:";
            // 
            // tabConsole
            // 
            tabConsole.Controls.Add(rtbConsole);
            tabConsole.Location = new Point(4, 29);
            tabConsole.Name = "tabConsole";
            tabConsole.Size = new Size(1272, 605);
            tabConsole.TabIndex = 1;
            tabConsole.Text = "Consola ASCII";
            // 
            // rtbConsole
            // 
            rtbConsole.BackColor = Color.FromArgb(20, 20, 30);
            rtbConsole.BorderStyle = BorderStyle.None;
            rtbConsole.Dock = DockStyle.Fill;
            rtbConsole.Font = new Font("Consolas", 9F);
            rtbConsole.ForeColor = Color.LightGreen;
            rtbConsole.Location = new Point(0, 0);
            rtbConsole.Name = "rtbConsole";
            rtbConsole.ReadOnly = true;
            rtbConsole.Size = new Size(1272, 605);
            rtbConsole.TabIndex = 0;
            rtbConsole.Text = "";
            rtbConsole.WordWrap = false;
            // 
            // tabAutoChart
            // 
            tabAutoChart.Controls.Add(tlpAutoCharts);
            tabAutoChart.Location = new Point(4, 29);
            tabAutoChart.Name = "tabAutoChart";
            tabAutoChart.Size = new Size(1272, 605);
            tabAutoChart.TabIndex = 2;
            tabAutoChart.Text = "⚡ Grafica Generada";
            // 
            // tlpAutoCharts
            // 
            tlpAutoCharts.ColumnCount = 2;
            tlpAutoCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAutoCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAutoCharts.Dock = DockStyle.Fill;
            tlpAutoCharts.Location = new Point(0, 0);
            tlpAutoCharts.Name = "tlpAutoCharts";
            tlpAutoCharts.RowCount = 2;
            tlpAutoCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpAutoCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpAutoCharts.Size = new Size(1272, 605);
            tlpAutoCharts.TabIndex = 0;
            // 
            // chartAutoBar
            // 
            chartAutoBar = new Chart();
            chartAutoBar.Dock = DockStyle.Fill;
            chartAutoBar.Name = "chartAutoBar";
            chartAutoBar.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default"));
            tlpAutoCharts.Controls.Add(chartAutoBar, 0, 0);
            // 
            // chartAutoPie
            // 
            chartAutoPie = new Chart();
            chartAutoPie.Dock = DockStyle.Fill;
            chartAutoPie.Name = "chartAutoPie";
            chartAutoPie.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default"));
            tlpAutoCharts.Controls.Add(chartAutoPie, 1, 0);
            // 
            // chartAutoDoughnut
            // 
            chartAutoDoughnut = new Chart();
            chartAutoDoughnut.Dock = DockStyle.Fill;
            chartAutoDoughnut.Name = "chartAutoDoughnut";
            chartAutoDoughnut.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default"));
            tlpAutoCharts.Controls.Add(chartAutoDoughnut, 0, 1);
            // 
            // chartAutoLine
            // 
            chartAutoLine = new Chart();
            chartAutoLine.Dock = DockStyle.Fill;
            chartAutoLine.Name = "chartAutoLine";
            chartAutoLine.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default"));
            tlpAutoCharts.Controls.Add(chartAutoLine, 1, 1);
            // 
            // pnlFiles
            // 
            pnlFiles.BackColor = Color.FromArgb(45, 45, 68);
            pnlFiles.Controls.Add(btnChartConsole);
            pnlFiles.Controls.Add(btnGenerateChart);
            pnlFiles.Controls.Add(btnClearData);
            pnlFiles.Controls.Add(btnImportPostgre);
            pnlFiles.Controls.Add(btnImportMariaDb);
            pnlFiles.Controls.Add(btnImportSqlServer);
            pnlFiles.Controls.Add(btnImportTxt);
            pnlFiles.Controls.Add(btnImportXml);
            pnlFiles.Controls.Add(btnImportJson);
            pnlFiles.Controls.Add(btnImportCsv);
            pnlFiles.Controls.Add(lblImport);
            pnlFiles.Dock = DockStyle.Top;
            pnlFiles.Location = new Point(0, 55);
            pnlFiles.Name = "pnlFiles";
            pnlFiles.Size = new Size(1377, 45);
            pnlFiles.TabIndex = 2;
            // 
            // btnChartConsole
            // 
            btnChartConsole.BackColor = Color.FromArgb(104, 33, 122);
            btnChartConsole.FlatStyle = FlatStyle.Flat;
            btnChartConsole.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnChartConsole.ForeColor = Color.White;
            btnChartConsole.Location = new Point(1139, 8);
            btnChartConsole.Name = "btnChartConsole";
            btnChartConsole.Size = new Size(145, 28);
            btnChartConsole.TabIndex = 0;
            btnChartConsole.Text = "📊 Grafica Consola";
            btnChartConsole.UseVisualStyleBackColor = false;
            btnChartConsole.Click += btnChartConsole_Click;
            // 
            // btnGenerateChart
            // 
            btnGenerateChart.BackColor = Color.FromArgb(0, 120, 215);
            btnGenerateChart.FlatStyle = FlatStyle.Flat;
            btnGenerateChart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGenerateChart.ForeColor = Color.White;
            btnGenerateChart.Location = new Point(971, 8);
            btnGenerateChart.Name = "btnGenerateChart";
            btnGenerateChart.Size = new Size(155, 28);
            btnGenerateChart.TabIndex = 1;
            btnGenerateChart.Text = "⚡ Generar Grafica";
            btnGenerateChart.UseVisualStyleBackColor = false;
            btnGenerateChart.Click += btnGenerateChart_Click;
            // 
            // btnClearData
            // 
            btnClearData.BackColor = Color.FromArgb(180, 30, 30);
            btnClearData.FlatStyle = FlatStyle.Flat;
            btnClearData.ForeColor = Color.White;
            btnClearData.Location = new Point(875, 8);
            btnClearData.Name = "btnClearData";
            btnClearData.Size = new Size(85, 28);
            btnClearData.TabIndex = 2;
            btnClearData.Text = "Limpiar";
            btnClearData.UseVisualStyleBackColor = false;
            btnClearData.Click += btnClearData_Click;
            // 
            // btnImportPostgre
            // 
            btnImportPostgre.BackColor = Color.FromArgb(51, 103, 145);
            btnImportPostgre.FlatStyle = FlatStyle.Flat;
            btnImportPostgre.ForeColor = Color.White;
            btnImportPostgre.Location = new Point(710, 8);
            btnImportPostgre.Name = "btnImportPostgre";
            btnImportPostgre.Size = new Size(152, 28);
            btnImportPostgre.TabIndex = 10;
            btnImportPostgre.Text = "📥 PostgreSQL";
            btnImportPostgre.UseVisualStyleBackColor = false;
            btnImportPostgre.Click += btnImportPostgre_Click;
            // 
            // btnImportMariaDb
            // 
            btnImportMariaDb.BackColor = Color.FromArgb(0, 108, 133);
            btnImportMariaDb.FlatStyle = FlatStyle.Flat;
            btnImportMariaDb.ForeColor = Color.White;
            btnImportMariaDb.Location = new Point(595, 8);
            btnImportMariaDb.Name = "btnImportMariaDb";
            btnImportMariaDb.Size = new Size(105, 28);
            btnImportMariaDb.TabIndex = 9;
            btnImportMariaDb.Text = "📥 MariaDB";
            btnImportMariaDb.UseVisualStyleBackColor = false;
            btnImportMariaDb.Click += btnImportMariaDb_Click;
            // 
            // btnImportSqlServer
            // 
            btnImportSqlServer.BackColor = Color.FromArgb(204, 48, 43);
            btnImportSqlServer.FlatStyle = FlatStyle.Flat;
            btnImportSqlServer.ForeColor = Color.White;
            btnImportSqlServer.Location = new Point(480, 8);
            btnImportSqlServer.Name = "btnImportSqlServer";
            btnImportSqlServer.Size = new Size(105, 28);
            btnImportSqlServer.TabIndex = 8;
            btnImportSqlServer.Text = "📥 SQL Srv";
            btnImportSqlServer.UseVisualStyleBackColor = false;
            btnImportSqlServer.Click += btnImportSqlServer_Click;
            // 
            // btnImportTxt
            // 
            btnImportTxt.BackColor = Color.FromArgb(130, 60, 160);
            btnImportTxt.FlatStyle = FlatStyle.Flat;
            btnImportTxt.ForeColor = Color.White;
            btnImportTxt.Location = new Point(395, 8);
            btnImportTxt.Name = "btnImportTxt";
            btnImportTxt.Size = new Size(75, 28);
            btnImportTxt.TabIndex = 3;
            btnImportTxt.Text = "TXT";
            btnImportTxt.UseVisualStyleBackColor = false;
            btnImportTxt.Click += btnImportTxt_Click;
            // 
            // btnImportXml
            // 
            btnImportXml.BackColor = Color.FromArgb(0, 100, 180);
            btnImportXml.FlatStyle = FlatStyle.Flat;
            btnImportXml.ForeColor = Color.White;
            btnImportXml.Location = new Point(310, 8);
            btnImportXml.Name = "btnImportXml";
            btnImportXml.Size = new Size(75, 28);
            btnImportXml.TabIndex = 4;
            btnImportXml.Text = "XML";
            btnImportXml.UseVisualStyleBackColor = false;
            btnImportXml.Click += btnImportXml_Click;
            // 
            // btnImportJson
            // 
            btnImportJson.BackColor = Color.FromArgb(200, 100, 0);
            btnImportJson.FlatStyle = FlatStyle.Flat;
            btnImportJson.ForeColor = Color.White;
            btnImportJson.Location = new Point(225, 8);
            btnImportJson.Name = "btnImportJson";
            btnImportJson.Size = new Size(75, 28);
            btnImportJson.TabIndex = 5;
            btnImportJson.Text = "JSON";
            btnImportJson.UseVisualStyleBackColor = false;
            btnImportJson.Click += btnImportJson_Click;
            // 
            // btnImportCsv
            // 
            btnImportCsv.BackColor = Color.FromArgb(0, 153, 76);
            btnImportCsv.FlatStyle = FlatStyle.Flat;
            btnImportCsv.ForeColor = Color.White;
            btnImportCsv.Location = new Point(140, 8);
            btnImportCsv.Name = "btnImportCsv";
            btnImportCsv.Size = new Size(75, 28);
            btnImportCsv.TabIndex = 6;
            btnImportCsv.Text = "CSV";
            btnImportCsv.UseVisualStyleBackColor = false;
            btnImportCsv.Click += btnImportCsv_Click;
            // 
            // lblImport
            // 
            lblImport.AutoSize = true;
            lblImport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblImport.ForeColor = Color.FromArgb(130, 200, 255);
            lblImport.Location = new Point(12, 13);
            lblImport.Name = "lblImport";
            lblImport.Size = new Size(122, 21);
            lblImport.TabIndex = 7;
            lblImport.Text = "📥 IMPORTAR:";
            // 
            // pnlExport
            // 
            pnlExport.BackColor = Color.FromArgb(50, 35, 30);
            pnlExport.Controls.Add(btnExportPostgre);
            pnlExport.Controls.Add(btnExportMariaDb);
            pnlExport.Controls.Add(btnExportSql);
            pnlExport.Controls.Add(lblExport);
            pnlExport.Dock = DockStyle.Top;
            pnlExport.Location = new Point(0, 100);
            pnlExport.Name = "pnlExport";
            pnlExport.Size = new Size(1377, 42);
            pnlExport.TabIndex = 1;
            // 
            // btnExportPostgre
            // 
            btnExportPostgre.BackColor = Color.FromArgb(51, 103, 145);
            btnExportPostgre.FlatStyle = FlatStyle.Flat;
            btnExportPostgre.ForeColor = Color.White;
            btnExportPostgre.Location = new Point(415, 7);
            btnExportPostgre.Name = "btnExportPostgre";
            btnExportPostgre.Size = new Size(125, 28);
            btnExportPostgre.TabIndex = 0;
            btnExportPostgre.Text = "📤 PostgreSQL";
            btnExportPostgre.UseVisualStyleBackColor = false;
            btnExportPostgre.Click += btnExportPostgre_Click;
            // 
            // btnExportMariaDb
            // 
            btnExportMariaDb.BackColor = Color.FromArgb(0, 108, 133);
            btnExportMariaDb.FlatStyle = FlatStyle.Flat;
            btnExportMariaDb.ForeColor = Color.White;
            btnExportMariaDb.Location = new Point(297, 7);
            btnExportMariaDb.Name = "btnExportMariaDb";
            btnExportMariaDb.Size = new Size(105, 28);
            btnExportMariaDb.TabIndex = 1;
            btnExportMariaDb.Text = "📤 MariaDB";
            btnExportMariaDb.UseVisualStyleBackColor = false;
            btnExportMariaDb.Click += btnExportMariaDb_Click;
            // 
            // btnExportSql
            // 
            btnExportSql.BackColor = Color.FromArgb(204, 48, 43);
            btnExportSql.FlatStyle = FlatStyle.Flat;
            btnExportSql.ForeColor = Color.White;
            btnExportSql.Location = new Point(180, 7);
            btnExportSql.Name = "btnExportSql";
            btnExportSql.Size = new Size(105, 28);
            btnExportSql.TabIndex = 2;
            btnExportSql.Text = "📤 SQL Server";
            btnExportSql.UseVisualStyleBackColor = false;
            btnExportSql.Click += btnExportSql_Click;
            // 
            // lblExport
            // 
            lblExport.AutoSize = true;
            lblExport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblExport.ForeColor = Color.FromArgb(255, 200, 130);
            lblExport.Location = new Point(12, 12);
            lblExport.Name = "lblExport";
            lblExport.Size = new Size(160, 21);
            lblExport.TabIndex = 3;
            lblExport.Text = "📤 EXPORTAR a BD:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1377, 780);
            Controls.Add(tabControl);
            Controls.Add(pnlExport);
            Controls.Add(pnlFiles);
            Controls.Add(pnlTop);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Data Fusion Arena";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            tabControl.ResumeLayout(false);
            tabData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvData).EndInit();
            pnlDataFilter.ResumeLayout(false);
            pnlDataFilter.PerformLayout();
            tabConsole.ResumeLayout(false);
            tabAutoChart.ResumeLayout(false);
            pnlFiles.ResumeLayout(false);
            pnlFiles.PerformLayout();
            pnlExport.ResumeLayout(false);
            pnlExport.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // ── Declaracion de controles ───────────────────────────────────────
        private Panel pnlTop;
        private Panel pnlFiles;
        private Label lblTitle;
        private Label lblImport;
        private Button btnProcess;
        private Button btnConsole;
        private Button btnImportCsv;
        private Button btnImportJson;
        private Button btnImportXml;
        private Button btnImportTxt;
        private Button btnClearData;
        private Button btnGenerateChart;
        private Button btnImportSqlServer;
        private Button btnImportMariaDb;
        private Button btnImportPostgre;
        private Button btnChartConsole;
        private Label lblStatus;

        private TabControl tabControl;
        private TabPage tabData;
        private TabPage tabConsole;
        private TabPage tabAutoChart;

        private Panel pnlDataFilter;
        private Label lblFilter;
        private ComboBox cmbSourceFilter;
        private Label lblRowCount;
        private DataGridView dgvData;
        private Chart chartAutoBar;
        private Chart chartAutoPie;
        private Chart chartAutoDoughnut;
        private Chart chartAutoLine;
        private TableLayoutPanel tlpAutoCharts;
        private RichTextBox rtbConsole;

        private Panel pnlExport;
        private Label lblExport;
        private Button btnExportSql;
        private Button btnExportMariaDb;
        private Button btnExportPostgre;
    }
}
