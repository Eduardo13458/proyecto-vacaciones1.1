using System.Globalization;
using System.Text;
using Prueba_1_proyecto_vacaciones.Models;
using Prueba_1_proyecto_vacaciones.Processing;

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
        //  TABLA DINAMICA  –  se adapta a cualquier archivo importado
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renderiza todos los DataItem en una tabla dinamica que descubre
        /// automaticamente las columnas con datos reales.
        /// Funciona con cualquier archivo importado (CSV, JSON, XML, TXT, DB
        /// o archivos desconocidos con ExtraFields).
        /// </summary>
        public static string RenderDynamicTable(List<DataItem> items)
        {
            if (items.Count == 0) return "(sin datos)\r\n";

            var sb = new StringBuilder();

            // ── 1. Descubrir columnas con datos ────────────────────────────
            var (stringFields, numericFields) = DataProcessor.DiscoverFields(items);

            // Columnas base siempre visibles
            var columns = new List<string> { "ID", "Fuente" };

            // Agregar campos de texto que tengan datos
            foreach (var f in stringFields)
                columns.Add(f);

            // Agregar campos numericos que tengan datos
            foreach (var f in numericFields)
                columns.Add(f);

            // ── 2. Obtener valores como texto para cada celda ──────────────
            // rows[fila][columna] = texto
            var rows = new List<string[]>(items.Count);
            foreach (var item in items)
            {
                var row = new string[columns.Count];
                for (int c = 0; c < columns.Count; c++)
                {
                    string col = columns[c];
                    if (col == "ID")
                        row[c] = item.Id.ToString();
                    else if (col == "Fuente")
                        row[c] = item.Source.ToString();
                    else
                    {
                        // Intentar numerico primero
                        double numVal = DataProcessor.GetNumericValue(item, col);
                        string strVal = DataProcessor.GetStringValue(item, col);

                        if (numericFields.Contains(col))
                            row[c] = numVal != 0 ? numVal.ToString("F2", CultureInfo.InvariantCulture) : "";
                        else
                            row[c] = strVal;
                    }
                }
                rows.Add(row);
            }

            // ── 3. Calcular ancho de cada columna ──────────────────────────
            var widths = new int[columns.Count];
            for (int c = 0; c < columns.Count; c++)
                widths[c] = columns[c].Length;

            foreach (var row in rows)
                for (int c = 0; c < columns.Count; c++)
                {
                    int len = row[c]?.Length ?? 0;
                    if (len > widths[c]) widths[c] = len;
                }

            // Limitar ancho maximo por columna para legibilidad
            for (int c = 0; c < columns.Count; c++)
                if (widths[c] > 30) widths[c] = 30;

            // ── 4. Renderizar tabla ────────────────────────────────────────
            int totalWidth = 1; // borde izquierdo
            foreach (int w in widths)
                totalWidth += w + 3; // " valor " + "|"

            sb.AppendLine("╔══════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                    DATOS EN CONSOLA                         ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine($"  Total: {items.Count} registros   Columnas: {columns.Count}");
            sb.AppendLine();

            // Linea superior
            sb.Append("  ┌");
            for (int c = 0; c < columns.Count; c++)
            {
                sb.Append(new string('─', widths[c] + 2));
                sb.Append(c < columns.Count - 1 ? '┬' : '┐');
            }
            sb.AppendLine();

            // Encabezados
            sb.Append("  │");
            for (int c = 0; c < columns.Count; c++)
            {
                string header = columns[c].Length > widths[c]
                    ? columns[c][..widths[c]]
                    : columns[c];
                sb.Append($" {header.PadRight(widths[c])} │");
            }
            sb.AppendLine();

            // Separador encabezado
            sb.Append("  ├");
            for (int c = 0; c < columns.Count; c++)
            {
                sb.Append(new string('═', widths[c] + 2));
                sb.Append(c < columns.Count - 1 ? '╪' : '┤');
            }
            sb.AppendLine();

            // Filas de datos
            foreach (var row in rows)
            {
                sb.Append("  │");
                for (int c = 0; c < columns.Count; c++)
                {
                    string val = row[c] ?? "";
                    if (val.Length > widths[c])
                        val = val[..(widths[c] - 2)] + "..";

                    // Alinear a la derecha si es numerico
                    bool isNumeric = numericFields.Contains(columns[c]) || columns[c] == "ID";
                    sb.Append(isNumeric
                        ? $" {val.PadLeft(widths[c])} │"
                        : $" {val.PadRight(widths[c])} │");
                }
                sb.AppendLine();
            }

            // Linea inferior
            sb.Append("  └");
            for (int c = 0; c < columns.Count; c++)
            {
                sb.Append(new string('─', widths[c] + 2));
                sb.Append(c < columns.Count - 1 ? '┴' : '┘');
            }
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
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

        // ════════════════════════════════════════════════════════════════════
        //  GRAFICA DE PASTEL / ANILLO ASCII  (porcentajes)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renderiza datos categoricos como barras de porcentaje con etiquetas.
        /// Ideal para representar graficas de pastel/anillo en texto.
        /// </summary>
        public static string RenderPieAscii(
            Dictionary<string, double> data,
            string title,
            int maxWidth = 30)
        {
            if (data.Count == 0) return "(sin datos para grafica)\r\n";

            var sb = new StringBuilder();
            double total = 0;
            foreach (var v in data.Values)
                total += v;

            if (total == 0) return "(todos los valores son cero)\r\n";

            sb.AppendLine();
            sb.AppendLine($"  +-- {title} (%) --+");
            sb.AppendLine();

            foreach (var kv in data)
            {
                double pct = kv.Value / total * 100.0;
                int len = (int)(pct / 100.0 * maxWidth);
                if (len < 1 && kv.Value > 0) len = 1;
                string bar = new string('\u2588', len).PadRight(maxWidth);
                sb.AppendLine($"  {kv.Key,-16} |{bar}| {pct,5:F1}%  ({kv.Value:F2})");
            }

            sb.AppendLine($"  {"",16}  {new string('-', maxWidth + 2)}");
            sb.AppendLine($"  {"",16}  Total: {total:F2}");
            sb.AppendLine();
            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════════════════
        //  GRAFICA DE LINEAS ASCII  (sparklines Unicode)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renderiza series numericas como sparklines horizontales usando
        /// bloques Unicode ▁▂▃▄▅▆▇█ para mostrar la tendencia.
        /// </summary>
        public static string RenderSparkLines(
            Dictionary<string, List<double>> seriesData,
            string title,
            int maxPoints = 60)
        {
            if (seriesData.Count == 0) return "(sin series para grafica)\r\n";

            char[] sparks = ['\u2581', '\u2582', '\u2583', '\u2584',
                             '\u2585', '\u2586', '\u2587', '\u2588'];

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"  +-- {title} --+");
            sb.AppendLine();

            foreach (var kv in seriesData)
            {
                var values = kv.Value;
                if (values.Count == 0) continue;

                // Muestrear si hay demasiados puntos
                if (values.Count > maxPoints)
                {
                    var sampled = new List<double>();
                    double step = (double)(values.Count - 1) / (maxPoints - 1);
                    for (int i = 0; i < maxPoints; i++)
                    {
                        int idx = (int)Math.Round(i * step);
                        if (idx >= values.Count) idx = values.Count - 1;
                        sampled.Add(values[idx]);
                    }
                    values = sampled;
                }

                double min = values[0], max = values[0], sum = 0;
                foreach (double v in values)
                {
                    if (v < min) min = v;
                    if (v > max) max = v;
                    sum += v;
                }
                double avg = sum / values.Count;
                double range = max - min;

                var sparkLine = new StringBuilder(values.Count);
                foreach (double v in values)
                {
                    int level = range > 0
                        ? (int)((v - min) / range * (sparks.Length - 1))
                        : sparks.Length / 2;
                    if (level < 0) level = 0;
                    if (level >= sparks.Length) level = sparks.Length - 1;
                    sparkLine.Append(sparks[level]);
                }

                sb.AppendLine($"  {kv.Key,-14} {sparkLine}");
                sb.AppendLine($"  {"",14} min: {min:F1}  max: {max:F1}  avg: {avg:F1}  ({values.Count} pts)");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════════════════
        //  GRAFICA DE LINEAS ASCII  (cuadricula vertical)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Renderiza una serie numerica como grafica vertical ASCII con ejes.
        /// Usa caracteres para dibujar puntos en una cuadricula de texto.
        /// </summary>
        public static string RenderVerticalLineChart(
            Dictionary<string, List<double>> seriesData,
            string title,
            int chartHeight = 15,
            int chartWidth = 55)
        {
            if (seriesData.Count == 0) return "";

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"  +-- {title} (detalle) --+");
            sb.AppendLine();

            foreach (var kv in seriesData)
            {
                var values = kv.Value;
                if (values.Count == 0) continue;

                // Muestrear a chartWidth puntos
                if (values.Count > chartWidth)
                {
                    var sampled = new List<double>();
                    double step = (double)(values.Count - 1) / (chartWidth - 1);
                    for (int i = 0; i < chartWidth; i++)
                    {
                        int idx = (int)Math.Round(i * step);
                        if (idx >= values.Count) idx = values.Count - 1;
                        sampled.Add(values[idx]);
                    }
                    values = sampled;
                }

                double min = values[0], max = values[0];
                foreach (double v in values)
                {
                    if (v < min) min = v;
                    if (v > max) max = v;
                }

                double range = max - min;
                if (range == 0) range = 1;

                sb.AppendLine($"  Serie: {kv.Key}");

                // Dibujar fila por fila de arriba hacia abajo
                for (int row = chartHeight - 1; row >= 0; row--)
                {
                    double rowVal = min + (range * row / (chartHeight - 1));
                    sb.Append($"  {rowVal,8:F1} │");

                    for (int col = 0; col < values.Count; col++)
                    {
                        int level = (int)((values[col] - min) / range * (chartHeight - 1));
                        if (level == row)
                            sb.Append('●');
                        else if (level > row)
                            sb.Append(' ');
                        else
                            sb.Append(' ');
                    }

                    sb.AppendLine();
                }

                // Eje X
                sb.Append("           └");
                sb.AppendLine(new string('─', values.Count));
                sb.Append("            ");
                for (int i = 0; i < values.Count; i++)
                    sb.Append(i % 10 == 0 ? (i / 10 % 10).ToString() : " ");
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
