using System.Globalization;
using Prueba_1_proyecto_vacaciones.Models;

namespace Prueba_1_proyecto_vacaciones.Processing
{
    /// <summary>
    /// Procesa la lista principal List&lt;DataItem&gt;:
    /// filtrado, agrupacion con Dictionary, deteccion de duplicados
    /// y ordenamiento con algoritmos manuales (SIN LINQ .OrderBy).
    /// </summary>
    public static class DataProcessor
    {
        // ════════════════════════════════════════════════════════════════════
        //  FILTRADO
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Devuelve laptops cuyo precio sea >= minPrice.</summary>
        public static List<DataItem> FilterLaptopsByMinPrice(
            List<DataItem> items, double minPrice)
        {
            var result = new List<DataItem>();
            foreach (var item in items)
                if (item.Source == DataSource.CSV && item.Price >= minPrice)
                    result.Add(item);
            return result;
        }

        /// <summary>Devuelve registros TXT cuya temperatura sea &gt; maxTemp.</summary>
        public static List<DataItem> FilterLogsByTemperature(
            List<DataItem> items, double minTemp)
        {
            var result = new List<DataItem>();
            foreach (var item in items)
                if (item.Source == DataSource.TXT && item.Temperatura > minTemp)
                    result.Add(item);
            return result;
        }

        // ════════════════════════════════════════════════════════════════════
        //  AGRUPACION CON DICTIONARY
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Precio promedio de laptops agrupado por marca (Company).
        /// Usa Dictionary para evitar recorrer la lista mas de una vez.
        /// </summary>
        public static Dictionary<string, double> GetAvgPriceByBrand(
            List<DataItem> items)
        {
            var sumMap   = new Dictionary<string, double>();
            var countMap = new Dictionary<string, int>();

            foreach (var item in items)
            {
                if (item.Source != DataSource.CSV) continue;

                if (!sumMap.ContainsKey(item.Company))
                {
                    sumMap[item.Company]   = 0;
                    countMap[item.Company] = 0;
                }
                sumMap[item.Company]   += item.Price;
                countMap[item.Company] += 1;
            }

            var result = new Dictionary<string, double>();
            foreach (var key in sumMap.Keys)
                result[key] = sumMap[key] / countMap[key];

            return result;
        }

        /// <summary>Total de ventas de videojuegos por genero.</summary>
        public static Dictionary<string, double> GetSalesByGenre(
            List<DataItem> items)
        {
            var result = new Dictionary<string, double>();
            foreach (var item in items)
            {
                if (item.Source != DataSource.JSON) continue;
                if (!result.ContainsKey(item.Genre))
                    result[item.Genre] = 0;
                result[item.Genre] += item.Sales;
            }
            return result;
        }

        /// <summary>Stock total de inventario por tipo de componente.</summary>
        public static Dictionary<string, int> GetStockByType(
            List<DataItem> items)
        {
            var result = new Dictionary<string, int>();
            foreach (var item in items)
            {
                if (item.Source != DataSource.XML) continue;
                if (!result.ContainsKey(item.Tipo))
                    result[item.Tipo] = 0;
                result[item.Tipo] += item.Stock;
            }
            return result;
        }

        /// <summary>
        /// Indice Dictionary&lt;int, DataItem&gt; para busqueda O(1) por ID,
        /// evitando recorrer la lista principal multiples veces.
        /// </summary>
        public static Dictionary<int, DataItem> BuildIdIndex(
            List<DataItem> items)
        {
            var index = new Dictionary<int, DataItem>(items.Count);
            foreach (var item in items)
                index[item.Id] = item;
            return index;
        }

        /// <summary>Agrupa DataItems por fuente en un Dictionary.</summary>
        public static Dictionary<DataSource, List<DataItem>> GroupBySource(
            List<DataItem> items)
        {
            var dict = new Dictionary<DataSource, List<DataItem>>();
            foreach (var item in items)
            {
                if (!dict.ContainsKey(item.Source))
                    dict[item.Source] = new List<DataItem>();
                dict[item.Source].Add(item);
            }
            return dict;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DETECCION DE DUPLICADOS
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Detecta duplicados comparando la clave compuesta Source + Label.
        /// </summary>
        public static List<DataItem> DetectDuplicates(List<DataItem> items)
        {
            var seen  = new HashSet<string>();
            var dupes = new List<DataItem>();

            foreach (var item in items)
            {
                string key = $"{item.Source}:{item.Label}";
                if (!seen.Add(key))
                    dupes.Add(item);
            }
            return dupes;
        }

        // ════════════════════════════════════════════════════════════════════
        //  ORDENAMIENTO MANUAL  —  SIN LINQ (.OrderBy PROHIBIDO)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Insertion Sort sobre List&lt;DataItem&gt;.
        /// Ordena por el valor numerico principal segun la fuente.
        /// NO se utiliza LINQ.OrderBy en ningun momento.
        /// </summary>
        public static void InsertionSort(List<DataItem> items, bool ascending = true)
        {
            for (int i = 1; i < items.Count; i++)
            {
                var current = items[i];
                double currentVal = GetSortValue(current);
                int j = i - 1;

                while (j >= 0 && CompareValues(GetSortValue(items[j]), currentVal, ascending) > 0)
                {
                    items[j + 1] = items[j];
                    j--;
                }
                items[j + 1] = current;
            }
        }

        /// <summary>
        /// Bubble Sort (alternativa). Incluye la optimizacion de corte
        /// anticipado cuando no hay intercambios en una pasada.
        /// NO se utiliza LINQ.OrderBy en ningun momento.
        /// </summary>
        public static void BubbleSort(List<DataItem> items, bool ascending = true)
        {
            int n = items.Count;
            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (CompareValues(GetSortValue(items[j]),
                                      GetSortValue(items[j + 1]),
                                      ascending) > 0)
                    {
                        (items[j], items[j + 1]) = (items[j + 1], items[j]);
                        swapped = true;
                    }
                }
                if (!swapped) break;
            }
        }

        /// <summary>
        /// Insertion Sort con selector de clave personalizado.
        /// Permite ordenar por cualquier campo numerico sin usar LINQ.
        /// </summary>
        public static void InsertionSortBy(
            List<DataItem> items,
            Func<DataItem, double> keySelector,
            bool ascending = true)
        {
            for (int i = 1; i < items.Count; i++)
            {
                var current   = items[i];
                double curVal = keySelector(current);
                int j = i - 1;

                while (j >= 0 && CompareValues(keySelector(items[j]), curVal, ascending) > 0)
                {
                    items[j + 1] = items[j];
                    j--;
                }
                items[j + 1] = current;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ANALISIS UNIVERSAL PARA GRAFICAS (campos conocidos + ExtraFields)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Analiza TODOS los items y encuentra automaticamente el mejor par
        /// (campo categorico, campo numerico) para generar una grafica.
        /// Busca primero en campos conocidos de DataItem, luego en ExtraFields.
        /// Retorna datos agrupados: categoria → suma del valor numerico.
        /// </summary>
        public static Dictionary<string, double> AutoDetectChartData(
            List<DataItem> items, out string categoryLabel, out string valueLabel)
        {
            categoryLabel = "";
            valueLabel = "";
            var result = new Dictionary<string, double>();

            // ── 1. Detectar campos conocidos con datos reales ──────────────
            bool hasCompany = false, hasGenre = false, hasTipo = false;
            bool hasRegion = false, hasTypeName = false, hasTitle = false;
            bool hasPrice = false, hasSales = false, hasStock = false;
            bool hasTemp = false, hasFPS = false;

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Company))  hasCompany  = true;
                if (!string.IsNullOrEmpty(item.Genre))     hasGenre    = true;
                if (!string.IsNullOrEmpty(item.Tipo))      hasTipo     = true;
                if (!string.IsNullOrEmpty(item.Region))    hasRegion   = true;
                if (!string.IsNullOrEmpty(item.TypeName))  hasTypeName = true;
                if (!string.IsNullOrEmpty(item.Title))     hasTitle    = true;
                if (item.Price > 0)        hasPrice = true;
                if (item.Sales > 0)        hasSales = true;
                if (item.Stock > 0)        hasStock = true;
                if (item.Temperatura > 0)  hasTemp  = true;
                if (item.FPS > 0)          hasFPS   = true;
            }

            // ── 2. Probar combinaciones de (string, double) conocidas ──────
            // Formato: (tieneCategoria, tieneValor, nombreCat, nombreVal,
            //           getCategoria, getValor)
            if (hasCompany && hasPrice)
            {
                categoryLabel = "Company";
                valueLabel = "Price";
                return GroupKnownFields(items,
                    i => i.Company, i => i.Price,
                    i => !string.IsNullOrEmpty(i.Company) && i.Price > 0);
            }
            if (hasGenre && hasSales)
            {
                categoryLabel = "Genre";
                valueLabel = "Sales";
                return GroupKnownFields(items,
                    i => i.Genre, i => i.Sales,
                    i => !string.IsNullOrEmpty(i.Genre) && i.Sales > 0);
            }
            if (hasTipo && hasStock)
            {
                categoryLabel = "Tipo";
                valueLabel = "Stock";
                return GroupKnownFields(items,
                    i => i.Tipo, i => (double)i.Stock,
                    i => !string.IsNullOrEmpty(i.Tipo) && i.Stock > 0);
            }
            if (hasRegion && hasPrice)
            {
                categoryLabel = "Region";
                valueLabel = "Price";
                return GroupKnownFields(items,
                    i => i.Region, i => i.Price,
                    i => !string.IsNullOrEmpty(i.Region) && i.Price > 0);
            }
            if (hasTitle && hasSales)
            {
                categoryLabel = "Title";
                valueLabel = "Sales";
                return GroupKnownFields(items,
                    i => i.Title, i => i.Sales,
                    i => !string.IsNullOrEmpty(i.Title) && i.Sales > 0);
            }
            if (hasTypeName && hasPrice)
            {
                categoryLabel = "TypeName";
                valueLabel = "Price";
                return GroupKnownFields(items,
                    i => i.TypeName, i => i.Price,
                    i => !string.IsNullOrEmpty(i.TypeName) && i.Price > 0);
            }
            if (hasCompany && hasTemp)
            {
                categoryLabel = "Company";
                valueLabel = "Temperatura";
                return GroupKnownFields(items,
                    i => i.Company, i => i.Temperatura,
                    i => !string.IsNullOrEmpty(i.Company) && i.Temperatura > 0);
            }

            // ── 3. Buscar en ExtraFields ───────────────────────────────────
            var keySamples = new Dictionary<string, List<string>>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
                foreach (var kv in item.ExtraFields)
                {
                    if (!keySamples.ContainsKey(kv.Key))
                        keySamples[kv.Key] = new List<string>();
                    if (keySamples[kv.Key].Count < 200)
                        keySamples[kv.Key].Add(kv.Value);
                }

            if (keySamples.Count < 2) return result;

            string? numericField = null;
            string? categoryField = null;

            foreach (var kv in keySamples)
            {
                int numCount = 0;
                foreach (var val in kv.Value)
                    if (double.TryParse(val, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out _))
                        numCount++;

                double ratio = (double)numCount / kv.Value.Count;

                if (ratio >= 0.8 && numericField == null)
                    numericField = kv.Key;
                else if (ratio < 0.5 && categoryField == null)
                    categoryField = kv.Key;
            }

            if (numericField == null || categoryField == null) return result;

            categoryLabel = categoryField;
            valueLabel = numericField;

            foreach (var item in items)
            {
                if (!item.ExtraFields.TryGetValue(categoryField, out var cat)) continue;
                if (!item.ExtraFields.TryGetValue(numericField, out var numStr)) continue;
                if (!double.TryParse(numStr, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out double num)) continue;

                if (string.IsNullOrWhiteSpace(cat)) continue;
                if (!result.ContainsKey(cat))
                    result[cat] = 0;
                result[cat] += num;
            }

            return result;
        }

        /// <summary>
        /// Extrae todas las series numericas disponibles para grafica de linea.
        /// Busca en campos conocidos y en ExtraFields.
        /// Cada serie es un par (nombre, lista de valores).
        /// </summary>
        public static Dictionary<string, List<double>> AutoDetectLineSeries(
            List<DataItem> items)
        {
            var series = new Dictionary<string, List<double>>(
                StringComparer.OrdinalIgnoreCase);

            // Intentar campos conocidos que tengan datos
            bool hasTemp = false, hasFPS = false, hasPrice = false, hasSales = false;
            foreach (var item in items)
            {
                if (item.Temperatura > 0) hasTemp  = true;
                if (item.FPS > 0)         hasFPS   = true;
                if (item.Price > 0)       hasPrice = true;
                if (item.Sales > 0)       hasSales = true;
            }

            if (hasTemp)
            {
                series["Temperatura"] = new List<double>();
                foreach (var item in items)
                    if (item.Temperatura > 0)
                        series["Temperatura"].Add(item.Temperatura);
            }
            if (hasFPS)
            {
                series["FPS"] = new List<double>();
                foreach (var item in items)
                    if (item.FPS > 0)
                        series["FPS"].Add(item.FPS);
            }
            if (!hasTemp && !hasFPS && hasPrice)
            {
                series["Price"] = new List<double>();
                foreach (var item in items)
                    if (item.Price > 0)
                        series["Price"].Add(item.Price);
            }
            if (!hasTemp && !hasFPS && hasSales)
            {
                series["Sales"] = new List<double>();
                foreach (var item in items)
                    if (item.Sales > 0)
                        series["Sales"].Add(item.Sales);
            }

            // Si no hay campos conocidos, buscar en ExtraFields
            if (series.Count == 0)
            {
                foreach (var item in items)
                    foreach (var kv in item.ExtraFields)
                        if (double.TryParse(kv.Value, NumberStyles.Any,
                                CultureInfo.InvariantCulture, out double val))
                        {
                            if (!series.ContainsKey(kv.Key))
                                series[kv.Key] = new List<double>();
                            series[kv.Key].Add(val);
                        }
            }

            return series;
        }

        private static Dictionary<string, double> GroupKnownFields(
            List<DataItem> items,
            Func<DataItem, string> getCategory,
            Func<DataItem, double> getValue,
            Func<DataItem, bool> filter)
        {
            var result = new Dictionary<string, double>();
            var counts = new Dictionary<string, int>();

            foreach (var item in items)
            {
                if (!filter(item)) continue;
                string cat = getCategory(item);
                if (string.IsNullOrEmpty(cat)) continue;

                if (!result.ContainsKey(cat))
                {
                    result[cat] = 0;
                    counts[cat] = 0;
                }
                result[cat] += getValue(item);
                counts[cat]++;
            }

            return result;
        }

        /// <summary>
        /// Devuelve solo las top N categorias por valor (desc) usando Insertion Sort.
        /// Si hay mas de maxCategories, agrupa el resto en "Otros".
        /// </summary>
        public static Dictionary<string, double> LimitTopN(
            Dictionary<string, double> data, int maxCategories)
        {
            if (data.Count <= maxCategories) return data;

            // Volcar a lista para ordenar manualmente (sin LINQ)
            var list = new List<KeyValuePair<string, double>>();
            foreach (var kv in data)
                list.Add(kv);

            // Insertion Sort descendente por valor
            for (int i = 1; i < list.Count; i++)
            {
                var current = list[i];
                int j = i - 1;
                while (j >= 0 && list[j].Value < current.Value)
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = current;
            }

            var result = new Dictionary<string, double>();
            double othersSum = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (i < maxCategories)
                    result[list[i].Key] = list[i].Value;
                else
                    othersSum += list[i].Value;
            }

            if (othersSum > 0)
                result["Otros"] = othersSum;

            return result;
        }

        /// <summary>
        /// Reduce una serie numerica a maxPoints muestreando uniformemente.
        /// </summary>
        public static List<double> SampleSeries(List<double> values, int maxPoints)
        {
            if (values.Count <= maxPoints) return values;

            var sampled = new List<double>();
            double step = (double)(values.Count - 1) / (maxPoints - 1);

            for (int i = 0; i < maxPoints; i++)
            {
                int idx = (int)Math.Round(i * step);
                if (idx >= values.Count) idx = values.Count - 1;
                sampled.Add(values[idx]);
            }

            return sampled;
        }

        // ── Helpers privados ───────────────────────────────────────────────

        private static int CompareValues(double a, double b, bool ascending)
        {
            int cmp = a.CompareTo(b);
            return ascending ? cmp : -cmp;
        }

        private static double GetSortValue(DataItem item) => item.Source switch
        {
            DataSource.CSV  => item.Price,
            DataSource.JSON => item.Sales,
            DataSource.XML  => item.Stock,
            DataSource.TXT  => item.Temperatura,
            DataSource.DB   => item.Id,
            _               => 0
        };
    }
}
