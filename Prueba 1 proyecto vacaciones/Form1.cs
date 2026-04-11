using System.Data;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using Prueba_1_proyecto_vacaciones.Data;
using Prueba_1_proyecto_vacaciones.Models;
using Prueba_1_proyecto_vacaciones.Processing;
using Prueba_1_proyecto_vacaciones.Visualization;

namespace Prueba_1_proyecto_vacaciones
{
    /// <summary>
    /// Ventana principal de Data Fusion Arena.
    /// Orquesta la carga, procesamiento y visualizacion de 5 fuentes de datos.
    /// </summary>
    public partial class Form1 : Form
    {
        // ── Almacenamiento principal: estrictamente List<DataItem> ──────────
        private List<DataItem> _allItems = [];

        // ── Indice rapido de busqueda por ID (Dictionary) ──────────────────
        private Dictionary<int, DataItem> _idIndex = [];

        // ── Cadena de conexion SQL (editar segun el entorno) ───────────────
        private const string SqlConn =
            @"Server=.\SQLEXPRESS;Database=DataFusionArena;" +
            @"Trusted_Connection=true;TrustServerCertificate=true;";

        public Form1()
        {
            InitializeComponent();
        }

        // ════════════════════════════════════════════════════════════════════
        //  BOTON: Cargar Datos
        // ════════════════════════════════════════════════════════════════════

        private void btnLoad_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            _allItems.Clear();

            // Leer las 5 fuentes
            _allItems.AddRange(DataReader.ReadCsv());
            _allItems.AddRange(DataReader.ReadJson());
            _allItems.AddRange(DataReader.ReadXml());
            _allItems.AddRange(DataReader.ReadTxt());
            _allItems.AddRange(DataReader.ReadFromDatabase(SqlConn));

            // Construir indice de busqueda rapida O(1)
            _idIndex = DataProcessor.BuildIdIndex(_allItems);

            // Llenar UI
            FillDataGridView();
            FillBarChart();
            FillPieChart();
            FillDoughnutChart();
            FillLineChart();

            lblStatus.Text = $"OK  {_allItems.Count} registros de 5 fuentes.";
            Cursor = Cursors.Default;
        }

        // ════════════════════════════════════════════════════════════════════
        //  BOTON: Procesar (sort, filtros, duplicados)
        // ════════════════════════════════════════════════════════════════════

        private void btnProcess_Click(object? sender, EventArgs e)
        {
            if (_allItems.Count == 0) btnLoad_Click(sender, e);

            var sb = new StringBuilder();

            // ── Ordenar laptops por precio (Insertion Sort) ────────────────
            var laptops = DataProcessor.FilterLaptopsByMinPrice(_allItems, 0);
            DataProcessor.InsertionSort(laptops);

            sb.AppendLine("=== LAPTOPS ORDENADAS POR PRECIO (Insertion Sort) " +
                          new string('=', 26));
            foreach (var l in laptops)
                sb.AppendLine($"  ${l.Price,9:F2}  {l.Company,-10} {l.TypeName}");
            sb.AppendLine();

            // ── Filtro: laptops > $1000 ────────────────────────────────────
            var expensive = DataProcessor.FilterLaptopsByMinPrice(_allItems, 1000);
            sb.AppendLine($"=== LAPTOPS > $1000: {expensive.Count} resultados " +
                          new string('=', 30));
            foreach (var l in expensive)
                sb.AppendLine($"  ${l.Price,9:F2}  {l.Company} {l.TypeName}");
            sb.AppendLine();

            // ── Filtro: temperatura > 75 °C ────────────────────────────────
            var hotLogs = DataProcessor.FilterLogsByTemperature(_allItems, 75);
            sb.AppendLine($"=== LOGS CON TEMPERATURA > 75 C: {hotLogs.Count} " +
                          new string('=', 25));
            foreach (var log in hotLogs)
                sb.AppendLine($"  Min {log.Minuto:D2}  CPU:{log.UsoCPU:F1}%  " +
                              $"Temp:{log.Temperatura:F1} C  FPS:{log.FPS:F1}");
            sb.AppendLine();

            // ── Ordenar videojuegos por ventas (Bubble Sort) ───────────────
            var games = new List<DataItem>();
            foreach (var item in _allItems)
                if (item.Source == DataSource.JSON)
                    games.Add(item);
            DataProcessor.BubbleSort(games, ascending: false);

            sb.AppendLine("=== VIDEOJUEGOS ORDENADOS POR VENTAS DESC (Bubble Sort) " +
                          new string('=', 19));
            foreach (var g in games)
                sb.AppendLine($"  {g.Sales,8:F2}M  {g.Title}");
            sb.AppendLine();

            // ── Duplicados ─────────────────────────────────────────────────
            var dupes = DataProcessor.DetectDuplicates(_allItems);
            sb.AppendLine($"=== DUPLICADOS DETECTADOS: {dupes.Count} " +
                          new string('=', 30));
            if (dupes.Count == 0) sb.AppendLine("  (ninguno)");
            foreach (var d in dupes) sb.AppendLine($"  {d}");
            sb.AppendLine();

            // ── Busqueda rapida por ID usando Dictionary ───────────────────
            sb.AppendLine("=== BUSQUEDA RAPIDA POR ID (Dictionary) " +
                          new string('=', 25));
            int[] sampleIds = [1, 1000, 2000, 3001, 4001];
            foreach (int id in sampleIds)
            {
                if (_idIndex.TryGetValue(id, out var found))
                    sb.AppendLine($"  ID {id,5} -> {found}");
                else
                    sb.AppendLine($"  ID {id,5} -> (no encontrado)");
            }
            sb.AppendLine();

            rtbConsole.Text = sb.ToString();
            tabControl.SelectedTab = tabConsole;
            lblStatus.Text = "Procesamiento completado.";
        }

        // ════════════════════════════════════════════════════════════════════
        //  BOTON: Ver Consola (tablas + graficas ASCII)
        // ════════════════════════════════════════════════════════════════════

        private void btnConsole_Click(object? sender, EventArgs e)
        {
            if (_allItems.Count == 0) btnLoad_Click(sender, e);

            var sb = new StringBuilder();

            // Tablas alineadas
            sb.AppendLine(ConsoleVisualizer.RenderTable(_allItems));

            // Grafica ASCII: precio promedio por marca
            var avgByBrand = DataProcessor.GetAvgPriceByBrand(_allItems);
            sb.AppendLine(ConsoleVisualizer.RenderBarChart(
                avgByBrand, "Precio Promedio por Marca (USD)"));

            // Grafica ASCII: ventas por genero
            var salesByGenre = DataProcessor.GetSalesByGenre(_allItems);
            sb.AppendLine(ConsoleVisualizer.RenderBarChart(
                salesByGenre, "Ventas por Genero (Millones)"));

            // Grafica ASCII: stock por tipo
            var stockByType = DataProcessor.GetStockByType(_allItems);
            var stockDouble = new Dictionary<string, double>();
            foreach (var kv in stockByType) stockDouble[kv.Key] = kv.Value;
            sb.AppendLine(ConsoleVisualizer.RenderBarChart(
                stockDouble, "Stock por Tipo de Componente"));

            rtbConsole.Text = sb.ToString();
            tabControl.SelectedTab = tabConsole;
            lblStatus.Text = "Consola actualizada.";
        }

        // ════════════════════════════════════════════════════════════════════
        //  IMPORTACION DE ARCHIVOS EXTERNOS
        // ════════════════════════════════════════════════════════════════════

        private void btnImportCsv_Click(object? sender, EventArgs e) =>
            ImportFile("Archivos CSV|*.csv");

        private void btnImportJson_Click(object? sender, EventArgs e) =>
            ImportFile("Archivos JSON|*.json");

        private void btnImportXml_Click(object? sender, EventArgs e) =>
            ImportFile("Archivos XML|*.xml");

        private void btnImportTxt_Click(object? sender, EventArgs e) =>
            ImportFile("Archivos de texto|*.txt");

        private void btnClearData_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Limpiar todos los datos cargados?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _allItems.Clear();
            _idIndex.Clear();
            FillDataGridView();
            chartBar.Series.Clear();
            chartPie.Series.Clear();
            chartDoughnut.Series.Clear();
            chartLine.Series.Clear();
            rtbConsole.Clear();
            lblStatus.Text = "Datos limpiados.";
        }

        /// <summary>
        /// Abre el dialogo de archivo, llama a DataReader.LoadFromFile() con
        /// la ruta absoluta elegida y refresca toda la UI.
        /// </summary>
        private void ImportFile(string filter)
        {
            using var dlg = new OpenFileDialog
            {
                Title  = "Seleccionar archivo de datos",
                Filter = filter + "|Todos los archivos|*.*",
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                var newItems = DataReader.LoadFromFile(dlg.FileName);

                if (newItems.Count == 0)
                {
                    lblStatus.Text = "El archivo no contiene registros validos.";
                    return;
                }

                _allItems.AddRange(newItems);
                _idIndex = DataProcessor.BuildIdIndex(_allItems);

                // Auto-select filter to show imported source
                var importedSource = newItems[0].Source.ToString();
                int filterIdx = cmbSourceFilter.Items.IndexOf(importedSource);
                if (filterIdx >= 0 && cmbSourceFilter.SelectedIndex != filterIdx)
                    cmbSourceFilter.SelectedIndex = filterIdx; // triggers FillDataGridView
                else
                    FillDataGridView();

                FillBarChart();
                FillPieChart();
                FillDoughnutChart();
                FillLineChart();

                lblStatus.Text =
                    $"Importados {newItems.Count} registros desde " +
                    $"'{Path.GetFileName(dlg.FileName)}'  " +
                    $"(Total: {_allItems.Count})";

                tabControl.SelectedTab = tabData;
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "Formato no soportado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al importar:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  DataGridView
        // ════════════════════════════════════════════════════════════════════

        private void FillDataGridView()
        {
            dgvData.DataSource = null;

            var selectedSource = cmbSourceFilter.SelectedItem?.ToString() ?? "Todas";

            // Collect items to display
            var displayItems = new List<DataItem>();
            foreach (var item in _allItems)
            {
                if (selectedSource != "Todas" && item.Source.ToString() != selectedSource)
                    continue;
                displayItems.Add(item);
            }

            // Discover extra column names (preserving first-seen order)
            var extraColumns = new List<string>();
            var extraSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in displayItems)
                foreach (var key in item.ExtraFields.Keys)
                    if (extraSet.Add(key))
                        extraColumns.Add(key);

            // Base columns (known DataItem fields)
            var table = new DataTable();
            table.Columns.Add("ID",          typeof(int));
            table.Columns.Add("Fuente",      typeof(string));
            table.Columns.Add("Etiqueta",    typeof(string));
            table.Columns.Add("Company",     typeof(string));
            table.Columns.Add("TypeName",    typeof(string));
            table.Columns.Add("Price",       typeof(double));
            table.Columns.Add("Title",       typeof(string));
            table.Columns.Add("Genre",       typeof(string));
            table.Columns.Add("Sales",       typeof(double));
            table.Columns.Add("Tipo",        typeof(string));
            table.Columns.Add("Modelo",      typeof(string));
            table.Columns.Add("Stock",       typeof(int));
            table.Columns.Add("Minuto",      typeof(int));
            table.Columns.Add("Temperatura", typeof(double));
            table.Columns.Add("FPS",         typeof(double));
            table.Columns.Add("UserName",    typeof(string));
            table.Columns.Add("Region",      typeof(string));

            // Dynamic columns from ExtraFields
            foreach (var colName in extraColumns)
                if (!table.Columns.Contains(colName))
                    table.Columns.Add(colName, typeof(string));

            // Fill rows
            foreach (var item in displayItems)
            {
                var row = table.NewRow();
                row["ID"]          = item.Id;
                row["Fuente"]      = item.Source.ToString();
                row["Etiqueta"]    = item.Label;
                row["Company"]     = item.Company;
                row["TypeName"]    = item.TypeName;
                row["Price"]       = item.Price;
                row["Title"]       = item.Title;
                row["Genre"]       = item.Genre;
                row["Sales"]       = item.Sales;
                row["Tipo"]        = item.Tipo;
                row["Modelo"]      = item.Modelo;
                row["Stock"]       = item.Stock;
                row["Minuto"]      = item.Minuto;
                row["Temperatura"] = item.Temperatura;
                row["FPS"]         = item.FPS;
                row["UserName"]    = item.UserName;
                row["Region"]      = item.Region;

                foreach (var kv in item.ExtraFields)
                    if (table.Columns.Contains(kv.Key))
                        row[kv.Key] = kv.Value;

                table.Rows.Add(row);
            }

            dgvData.DataSource = table;
            dgvData.AutoResizeColumns();

            // Hide columns that are entirely empty/default
            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (col.Name == "ID" || col.Name == "Fuente" || col.Name == "Etiqueta")
                    continue;

                bool hasData = false;
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    var val = row.Cells[col.Index].Value;
                    if (val != null && val != DBNull.Value
                        && val.ToString() != "" && val.ToString() != "0")
                    {
                        hasData = true;
                        break;
                    }
                }
                col.Visible = hasData;
            }

            foreach (DataGridViewRow row in dgvData.Rows)
            {
                var src = row.Cells["Fuente"].Value?.ToString() ?? "";
                row.DefaultCellStyle.BackColor = GetSourceColor(src);
            }

            lblRowCount.Text = $"{table.Rows.Count} registros";
        }

        private void cmbSourceFilter_SelectedIndexChanged(object? sender, EventArgs e) =>
            FillDataGridView();

        private static Color GetSourceColor(string source) => source switch
        {
            "CSV"  => Color.FromArgb(220, 235, 255),
            "JSON" => Color.FromArgb(255, 235, 210),
            "XML"  => Color.FromArgb(215, 245, 215),
            "TXT"  => Color.FromArgb(240, 220, 255),
            "DB"   => Color.FromArgb(255, 250, 210),
            _      => Color.White,
        };

        // ════════════════════════════════════════════════════════════════════
        //  GRAFICAS  –  Chart de WinForms DataVisualization
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Grafica de Barras: precio promedio de laptops por marca.</summary>
        private void FillBarChart()
        {
            chartBar.Series.Clear();
            chartBar.Titles.Clear();

            var area = chartBar.ChartAreas[0];
            area.AxisX.Title = "Marca";
            area.AxisY.Title = "Precio Promedio (USD)";
            area.AxisX.LabelStyle.Angle = -30;
            area.AxisX.MajorGrid.Enabled = false;

            chartBar.Titles.Add("Precio Promedio de Laptops por Marca");

            var series = chartBar.Series.Add("Precio");
            series.ChartType = SeriesChartType.Column;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "${0:F0}";

            var data = DataProcessor.GetAvgPriceByBrand(_allItems);
            int idx = 0;
            foreach (var kv in data)
            {
                int pt = series.Points.AddXY(kv.Key, kv.Value);
                series.Points[pt].Color = GetColor(idx++);
            }
        }

        /// <summary>Grafica de Pastel: porcentaje de ventas por genero.</summary>
        private void FillPieChart()
        {
            chartPie.Series.Clear();
            chartPie.Titles.Clear();
            chartPie.Legends.Clear();

            chartPie.Titles.Add("Ventas por Genero de Videojuego (Millones)");

            var series = chartPie.Series.Add("Ventas");
            series.ChartType = SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0:F1}M";

            var data = DataProcessor.GetSalesByGenre(_allItems);
            int idx = 0;
            foreach (var kv in data)
            {
                int pt = series.Points.AddXY(kv.Key, kv.Value);
                series.Points[pt].Color = GetColor(idx++);
                series.Points[pt].LegendText = $"{kv.Key}: {kv.Value:F1}M";
            }

            chartPie.Legends.Add(new Legend { Docking = Docking.Bottom });
        }

        /// <summary>Grafica de Anillo: distribucion de stock por tipo.</summary>
        private void FillDoughnutChart()
        {
            chartDoughnut.Series.Clear();
            chartDoughnut.Titles.Clear();
            chartDoughnut.Legends.Clear();

            chartDoughnut.Titles.Add("Distribucion de Stock por Tipo de Componente");

            var series = chartDoughnut.Series.Add("Stock");
            series.ChartType = SeriesChartType.Doughnut;
            series["DoughnutRadius"] = "40";
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0} uds.";

            var data = DataProcessor.GetStockByType(_allItems);
            int idx = 0;
            foreach (var kv in data)
            {
                int pt = series.Points.AddXY(kv.Key, kv.Value);
                series.Points[pt].Color = GetColor(idx++);
                series.Points[pt].LegendText = $"{kv.Key}: {kv.Value} uds.";
            }

            chartDoughnut.Legends.Add(new Legend { Docking = Docking.Bottom });
        }

        /// <summary>Grafica de Lineas: temperatura y FPS en el tiempo.</summary>
        private void FillLineChart()
        {
            chartLine.Series.Clear();
            chartLine.Titles.Clear();
            chartLine.Legends.Clear();

            var area = chartLine.ChartAreas[0];
            area.AxisX.Title = "Minuto";
            area.AxisY.Title = "Valor";
            area.AxisX.Interval = 1;

            chartLine.Titles.Add("Evolucion de Temperatura y FPS en el Tiempo");

            // Serie Temperatura
            var seriesTemp = chartLine.Series.Add("Temperatura (C)");
            seriesTemp.ChartType = SeriesChartType.Line;
            seriesTemp.BorderWidth = 3;
            seriesTemp.Color = Color.OrangeRed;
            seriesTemp.MarkerStyle = MarkerStyle.Circle;
            seriesTemp.MarkerSize = 7;

            // Serie FPS
            var seriesFps = chartLine.Series.Add("FPS");
            seriesFps.ChartType = SeriesChartType.Line;
            seriesFps.BorderWidth = 3;
            seriesFps.Color = Color.SteelBlue;
            seriesFps.MarkerStyle = MarkerStyle.Diamond;
            seriesFps.MarkerSize = 7;

            // Extraer y ordenar logs por Minuto (Insertion Sort manual)
            var logs = new List<DataItem>();
            foreach (var item in _allItems)
                if (item.Source == DataSource.TXT)
                    logs.Add(item);

            DataProcessor.InsertionSortBy(logs, item => item.Minuto);

            foreach (var log in logs)
            {
                seriesTemp.Points.AddXY(log.Minuto, log.Temperatura);
                seriesFps.Points.AddXY(log.Minuto, log.FPS);
            }

            chartLine.Legends.Add(new Legend { Docking = Docking.Top });
        }

        // ── Paleta de colores para las graficas ────────────────────────────

        private static Color GetColor(int index)
        {
            Color[] palette =
            [
                Color.FromArgb(70, 130, 180),   // SteelBlue
                Color.FromArgb(255, 127, 80),   // Coral
                Color.FromArgb(60, 179, 113),   // MediumSeaGreen
                Color.FromArgb(186, 85, 211),   // MediumOrchid
                Color.FromArgb(255, 165, 0),    // Orange
                Color.FromArgb(220, 20, 60),    // Crimson
                Color.FromArgb(32, 178, 170),   // LightSeaGreen
                Color.FromArgb(218, 165, 32),   // GoldenRod
                Color.FromArgb(100, 149, 237),  // CornflowerBlue
                Color.FromArgb(255, 99, 71),    // Tomato
            ];
            return palette[index % palette.Length];
        }
    }
}
