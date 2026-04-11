using System.Text;
using Prueba_1_proyecto_vacaciones.Models;

namespace Prueba_1_proyecto_vacaciones.Visualization
{
    /// <summary>
    /// Genera salidas de texto para consola: tablas alineadas y graficas
    /// de barras con caracteres ASCII/Unicode.
    /// Los metodos devuelven strings para poder usarlos tanto en Console
    /// como en un RichTextBox de WinForms.
    /// </summary>
    public static class ConsoleVisualizer
    {
        // ════════════════════════════════════════════════════════════════════
        //  TABLA ALINEADA  –  una seccion por fuente de datos
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renderiza todos los DataItem en tablas alineadas agrupadas por fuente.
        /// </summary>
        public static string RenderTable(List<DataItem> items)
        {
            if (items.Count == 0) return "(sin datos)\r\n";

            var sb = new StringBuilder();

            // Agrupar manualmente por fuente (sin LINQ)
            var bySource = new Dictionary<DataSource, List<DataItem>>();
            foreach (var item in items)
            {
                if (!bySource.ContainsKey(item.Source))
                    bySource[item.Source] = new List<DataItem>();
                bySource[item.Source].Add(item);
            }

            if (bySource.ContainsKey(DataSource.CSV))
                RenderCsvSection(bySource[DataSource.CSV], sb);
            if (bySource.ContainsKey(DataSource.JSON))
                RenderJsonSection(bySource[DataSource.JSON], sb);
            if (bySource.ContainsKey(DataSource.XML))
                RenderXmlSection(bySource[DataSource.XML], sb);
            if (bySource.ContainsKey(DataSource.TXT))
                RenderTxtSection(bySource[DataSource.TXT], sb);
            if (bySource.ContainsKey(DataSource.DB))
                RenderDbSection(bySource[DataSource.DB], sb);

            return sb.ToString();
        }

        private static void RenderCsvSection(List<DataItem> items, StringBuilder sb)
        {
            sb.AppendLine("=== LAPTOPS (CSV) " + new string('=', 58));
            sb.AppendLine($"  {"Marca",-12} {"Tipo",-12} {"CPU",-26} {"RAM",-6} {"Precio",10}");
            sb.AppendLine("  " + new string('-', 70));
            foreach (var i in items)
                sb.AppendLine(
                    $"  {i.Company,-12} {i.TypeName,-12} {i.Cpu,-26} {i.Ram + "GB",-6} ${i.Price,9:F2}");
            sb.AppendLine();
        }

        private static void RenderJsonSection(List<DataItem> items, StringBuilder sb)
        {
            sb.AppendLine("=== VIDEOJUEGOS (JSON) " + new string('=', 53));
            sb.AppendLine($"  {"Titulo",-28} {"Genero",-12} {"Plataforma",-18} {"Ventas(M)",10}");
            sb.AppendLine("  " + new string('-', 72));
            foreach (var i in items)
                sb.AppendLine(
                    $"  {i.Title,-28} {i.Genre,-12} {i.Platform,-18} {i.Sales,10:F2}");
            sb.AppendLine();
        }

        private static void RenderXmlSection(List<DataItem> items, StringBuilder sb)
        {
            sb.AppendLine("=== INVENTARIO (XML) " + new string('=', 55));
            sb.AppendLine($"  {"Tipo",-8} {"Modelo",-30} {"Stock",8}");
            sb.AppendLine("  " + new string('-', 50));
            foreach (var i in items)
                sb.AppendLine($"  {i.Tipo,-8} {i.Modelo,-30} {i.Stock,8}");
            sb.AppendLine();
        }

        private static void RenderTxtSection(List<DataItem> items, StringBuilder sb)
        {
            sb.AppendLine("=== LOG DE RENDIMIENTO (TXT) " + new string('=', 47));
            sb.AppendLine($"  {"Min",5} {"CPU%",8} {"Temp C",10} {"FPS",8}");
            sb.AppendLine("  " + new string('-', 35));
            foreach (var i in items)
                sb.AppendLine(
                    $"  {i.Minuto,5} {i.UsoCPU,8:F1} {i.Temperatura,10:F1} {i.FPS,8:F1}");
            sb.AppendLine();
        }

        private static void RenderDbSection(List<DataItem> items, StringBuilder sb)
        {
            sb.AppendLine("=== USUARIOS (DB) " + new string('=', 58));
            sb.AppendLine($"  {"ID",5} {"Nombre",-22} {"Email",-26} {"Region",12}");
            sb.AppendLine("  " + new string('-', 68));
            foreach (var i in items)
                sb.AppendLine(
                    $"  {i.Id,5} {i.UserName,-22} {i.Email,-26} {i.Region,12}");
            sb.AppendLine();
        }

        // ════════════════════════════════════════════════════════════════════
        //  GRAFICA DE BARRAS ASCII
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Dibuja una grafica de barras horizontal usando bloques Unicode.
        /// Ejemplo:  Dell           |████████████       | 1049.99
        /// </summary>
        public static string RenderBarChart(
            Dictionary<string, double> data,
            string title,
            int maxWidth = 35)
        {
            if (data.Count == 0) return "(sin datos para grafica)\r\n";

            var sb  = new StringBuilder();
            double max = 0;
            foreach (var v in data.Values)
                if (v > max) max = v;

            sb.AppendLine();
            sb.AppendLine($"  +-- {title} --+");
            sb.AppendLine();

            foreach (var kv in data)
            {
                int len    = max > 0 ? (int)(kv.Value / max * maxWidth) : 0;
                string bar = new string('\u2588', len).PadRight(maxWidth);
                sb.AppendLine($"  {kv.Key,-16} |{bar}| {kv.Value:F2}");
            }

            sb.AppendLine($"  {"",16}  {new string('-', maxWidth + 2)}");
            sb.AppendLine($"  {"",16}  0{new string(' ', maxWidth - 4)}{max:F0}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
