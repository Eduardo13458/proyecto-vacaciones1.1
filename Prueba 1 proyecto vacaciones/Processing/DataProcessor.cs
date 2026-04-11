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
