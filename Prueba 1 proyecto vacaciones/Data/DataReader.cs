using System.Globalization;
using System.Text.Json;
using System.Xml;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using Prueba_1_proyecto_vacaciones.Models;

namespace Prueba_1_proyecto_vacaciones.Data 
{
    public static class DataReader
    {
        private static readonly string DataFolder =
            Path.Combine(AppContext.BaseDirectory, "Data");

        // ── ENRUTADOR: carga cualquier archivo por extension ───────────────

        /// <summary>
        /// Recibe una ruta absoluta, detecta el tipo por extension y enruta
        /// al lector correcto. Soporta .csv, .json, .xml y .txt.
        /// </summary>
        public static List<DataItem> LoadFromFile(string fullPath)
        {
            var ext = Path.GetExtension(fullPath).ToLowerInvariant();
            return ext switch
            {
                ".csv"  => ImportGenericCsv(fullPath),
                ".json" => ImportGenericJson(fullPath),
                ".xml"  => ImportGenericXml(fullPath),
                ".txt"  => ImportGenericTxt(fullPath),
                _ => throw new NotSupportedException(
                    $"Extension '{ext}' no soportada. Use .csv, .json, .xml o .txt")
            };
        }

        // ── CSV ────────────────────────────────────────────────────────────

        /// <summary>
        /// Acepta nombre simple (busca en DataFolder) o ruta absoluta externa.
        /// Mapea columnas por nombre de cabecera para tolerar columnas
        /// desordenadas.
        /// </summary>
        public static List<DataItem> ReadCsv(string fileNameOrPath = "laptops.csv")
        {
            var items    = new List<DataItem>();
            var filePath = ResolvePath(fileNameOrPath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[CSV] Archivo no encontrado: {filePath}");
                return items;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 2) return items;

                var rawH    = lines[0].Split(',');
                var headers = new string[rawH.Length];
                for (int h = 0; h < rawH.Length; h++)
                    headers[h] = rawH[h].Trim().ToLowerInvariant();

                int iCompany  = Array.IndexOf(headers, "company");
                int iTypeName = Array.IndexOf(headers, "typename");
                int iCpu      = Array.IndexOf(headers, "cpu");
                int iRam      = Array.IndexOf(headers, "ram");
                int iPrice    = Array.IndexOf(headers, "price");

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var cols = lines[i].Split(',');

                    try
                    {
                        items.Add(new DataItem
                        {
                            Id       = i,
                            Source   = DataSource.CSV,
                            Company  = SafeGet(cols, iCompany),
                            TypeName = SafeGet(cols, iTypeName),
                            Cpu      = SafeGet(cols, iCpu),
                            Ram      = int.TryParse(SafeGet(cols, iRam), out int ram) ? ram : 0,
                            Price    = double.TryParse(SafeGet(cols, iPrice),
                                           NumberStyles.Any, CultureInfo.InvariantCulture,
                                           out double price) ? price : 0
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CSV] Error en linea {i + 1}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CSV] Error al leer archivo: {ex.Message}");
            }

            return items;
        }

        // ── JSON ───────────────────────────────────────────────────────────

        /// <summary>
        /// Acepta nombre simple o ruta absoluta.
        /// Maneja JSON mal formados con try/catch sobre JsonException.
        /// </summary>
        public static List<DataItem> ReadJson(string fileNameOrPath = "videogames.json")
        {
            var items    = new List<DataItem>();
            var filePath = ResolvePath(fileNameOrPath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[JSON] Archivo no encontrado: {filePath}");
                return items;
            }

            string jsonText;
            try { jsonText = File.ReadAllText(filePath); }
            catch (Exception ex)
            {
                Console.WriteLine($"[JSON] Error al leer archivo: {ex.Message}");
                return items;
            }

            try
            {
                using var doc = JsonDocument.Parse(jsonText);
                int id = 1000;

                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    try
                    {
                        items.Add(new DataItem
                        {
                            Id       = id++,
                            Source   = DataSource.JSON,
                            Title    = GetJsonString(el, "Title"),
                            Genre    = GetJsonString(el, "Genre"),
                            Sales    = GetJsonDouble(el, "Sales"),
                            Platform = GetJsonString(el, "Platform")
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[JSON] Error en elemento {id}: {ex.Message}");
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[JSON] JSON mal formado: {ex.Message}");
            }

            return items;
        }

        // ── XML ────────────────────────────────────────────────────────────

        /// <summary>Acepta nombre simple o ruta absoluta.</summary>
        public static List<DataItem> ReadXml(string fileNameOrPath = "inventory.xml")
        {
            var items    = new List<DataItem>();
            var filePath = ResolvePath(fileNameOrPath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[XML] Archivo no encontrado: {filePath}");
                return items;
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);

                var nodes = doc.SelectNodes("//Componente");
                if (nodes == null) return items;

                int id = 2000;
                foreach (XmlNode node in nodes)
                {
                    try
                    {
                        items.Add(new DataItem
                        {
                            Id     = id++,
                            Source = DataSource.XML,
                            Tipo   = node["Tipo"]?.InnerText   ?? string.Empty,
                            Modelo = node["Modelo"]?.InnerText ?? string.Empty,
                            Stock  = int.TryParse(node["Stock"]?.InnerText, out int s) ? s : 0
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[XML] Error en nodo {id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[XML] Error al leer archivo: {ex.Message}");
            }

            return items;
        }

        // ── TXT ────────────────────────────────────────────────────────────

        /// <summary>Acepta nombre simple o ruta absoluta.</summary>
        public static List<DataItem> ReadTxt(string fileNameOrPath = "performance.txt")
        {
            var items    = new List<DataItem>();
            var filePath = ResolvePath(fileNameOrPath);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[TXT] Archivo no encontrado: {filePath}");
                return items;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var parts = lines[i].Split('|');
                    if (parts.Length < 4) continue;

                    try
                    {
                        items.Add(new DataItem
                        {
                            Id          = 3000 + i,
                            Source      = DataSource.TXT,
                            Minuto      = ParseInt(parts[0]),
                            UsoCPU      = ParseDouble(parts[1]),
                            Temperatura = ParseDouble(parts[2]),
                            FPS         = ParseDouble(parts[3])
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[TXT] Error en linea {i + 1}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TXT] Error al leer archivo: {ex.Message}");
            }

            return items;
        }

        // ── SQL Server ─────────────────────────────────────────────────────

        /// <summary>
        /// Lee usuarios de SQL Server. Si la conexion falla, genera datos
        /// simulados. Columnas BD: user_id, full_name, user_email, zone.
        /// </summary>
        public static List<DataItem> ReadFromDatabase(string connectionString)
        {
            var items = new List<DataItem>();

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                const string sql =
                    "SELECT user_id, full_name, user_email, zone FROM dbo.Users";

                using var cmd    = new SqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new DataItem
                    {
                        Id       = reader.GetInt32(reader.GetOrdinal("user_id")),
                        Source   = DataSource.DB,
                        UserName = reader.GetString(reader.GetOrdinal("full_name")),
                        Email    = reader.GetString(reader.GetOrdinal("user_email")),
                        Region   = reader.GetString(reader.GetOrdinal("zone"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB] Conexion fallida, usando datos simulados: {ex.Message}");
                items = GetSimulatedDbData();
            }

            return items;
        }

        // ── Importar dinamico desde DatosIntegrados (SQL Server) ────────

        public static List<DataItem> ReadFromSqlServer(string connectionString,
            string tableName = "DatosIntegrados")
        {
            DatabaseExporter.EnsureSqlServerDatabase(connectionString);

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var check = new SqlCommand(
                $"SELECT OBJECT_ID('{tableName}', 'U')", conn);
            if (check.ExecuteScalar() is null or DBNull)
                return [];

            using var cmd = new SqlCommand($"SELECT * FROM [{tableName}]", conn);
            using var reader = cmd.ExecuteReader();
            return ReadItemsFromReader(reader);
        }

        // ── Importar dinamico desde DatosIntegrados (MariaDB) ───────────

        public static List<DataItem> ReadFromMariaDb(string connectionString,
            string tableName = "DatosIntegrados")
        {
            DatabaseExporter.EnsureMySqlDatabase(connectionString);

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            using var check = new MySqlCommand(
                "SELECT COUNT(*) FROM information_schema.tables " +
                $"WHERE table_schema = DATABASE() AND table_name = '{tableName}'", conn);
            if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                return [];

            using var cmd = new MySqlCommand($"SELECT * FROM `{tableName}`", conn);
            using var reader = cmd.ExecuteReader();
            return ReadItemsFromReader(reader);
        }

        // ── Importar dinamico desde DatosIntegrados (PostgreSQL) ────────

        public static List<DataItem> ReadFromPostgreSql(string connectionString,
            string tableName = "DatosIntegrados")
        {
            DatabaseExporter.EnsurePostgreSqlDatabase(connectionString);

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var check = new NpgsqlCommand(
                "SELECT EXISTS (SELECT FROM information_schema.tables " +
                $"WHERE table_name = '{tableName}')", conn);
            if (check.ExecuteScalar() is false)
                return [];

            using var cmd = new NpgsqlCommand($"SELECT * FROM \"{tableName}\"", conn);
            using var reader = cmd.ExecuteReader();
            return ReadItemsFromReader(reader);
        }

        private static List<DataItem> ReadItemsFromReader(DbDataReader reader)
        {
            var items = new List<DataItem>();
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columnNames.Add(reader.GetName(i));

            int id = 4000;
            while (reader.Read())
            {
                var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string val = reader.GetValue(i)?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(val))
                            fields[columnNames[i]] = val;
                    }
                }
                if (fields.Count > 0)
                    items.Add(MapFieldsToItem(fields, DataSource.DB, id++));
            }
            return items;
        }

        // ── Importadores genericos (cualquier estructura) ──────────────

        private static List<DataItem> ImportGenericCsv(string fullPath)
        {
            var items = new List<DataItem>();
            if (!File.Exists(fullPath)) return items;

            try
            {
                var lines = File.ReadAllLines(fullPath);
                if (lines.Length < 2) return items;

                var rawHeaders = lines[0].Split(',');
                var headers = new string[rawHeaders.Length];
                for (int h = 0; h < rawHeaders.Length; h++)
                    headers[h] = rawHeaders[h].Trim();

                int id = 1;
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var parts = lines[i].Split(',');

                    var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    int limit = Math.Min(headers.Length, parts.Length);
                    for (int c = 0; c < limit; c++)
                        if (!string.IsNullOrWhiteSpace(headers[c]))
                            fields[headers[c]] = parts[c].Trim();

                    if (fields.Count > 0)
                        items.Add(MapFieldsToItem(fields, DataSource.CSV, id++));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CSV Import] Error: {ex.Message}");
            }

            return items;
        }

        private static List<DataItem> ImportGenericJson(string fullPath)
        {
            var items = new List<DataItem>();
            if (!File.Exists(fullPath)) return items;

            string json;
            try { json = File.ReadAllText(fullPath); }
            catch { return items; }

            try
            {
                using var doc = JsonDocument.Parse(json);
                int id = 1000;
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in root.EnumerateArray())
                    {
                        var item = JsonElementToItem(el, id++);
                        if (item != null) items.Add(item);
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    // Buscar recursivamente el array de objetos mas grande
                    var bestArray = FindBestArray(root);
                    if (bestArray != null)
                    {
                        foreach (var el in bestArray.Value.EnumerateArray())
                        {
                            var item = JsonElementToItem(el, id++);
                            if (item != null) items.Add(item);
                        }
                    }

                    // Si no hay array anidado, tratar raiz como registro unico
                    if (items.Count == 0)
                    {
                        var item = JsonElementToItem(root, 1000);
                        if (item != null) items.Add(item);
                    }
                }
            }
            catch (JsonException) { }

            return items;
        }

        /// <summary>
        /// Recorre recursivamente el JSON buscando la propiedad de tipo array
        /// con mayor cantidad de objetos. Soporta anidamiento arbitrario.
        /// </summary>
        private static JsonElement? FindBestArray(JsonElement element)
        {
            JsonElement? best = null;
            int bestCount = 0;

            if (element.ValueKind != JsonValueKind.Object) return null;

            foreach (var prop in element.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    int objCount = 0;
                    foreach (var child in prop.Value.EnumerateArray())
                        if (child.ValueKind == JsonValueKind.Object)
                            objCount++;

                    if (objCount > bestCount)
                    {
                        bestCount = objCount;
                        best = prop.Value;
                    }
                }
                else if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    var nested = FindBestArray(prop.Value);
                    if (nested != null)
                    {
                        int nestedCount = 0;
                        foreach (var child in nested.Value.EnumerateArray())
                            if (child.ValueKind == JsonValueKind.Object)
                                nestedCount++;

                        if (nestedCount > bestCount)
                        {
                            bestCount = nestedCount;
                            best = nested;
                        }
                    }
                }
            }

            return best;
        }

        private static DataItem? JsonElementToItem(JsonElement el, int id)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            FlattenJsonObject(el, fields, "");
            return fields.Count > 0 ? MapFieldsToItem(fields, DataSource.JSON, id) : null;
        }

        /// <summary>
        /// Aplana un objeto JSON recursivamente.
        /// Objetos anidados → claves con prefijo (e.g. "info_edad").
        /// Arrays anidados → texto separado por comas.
        /// </summary>
        private static void FlattenJsonObject(
            JsonElement el, Dictionary<string, string> fields, string prefix)
        {
            foreach (var prop in el.EnumerateObject())
            {
                string key = string.IsNullOrEmpty(prefix)
                    ? prop.Name
                    : $"{prefix}_{prop.Name}";

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        FlattenJsonObject(prop.Value, fields, key);
                        break;
                    case JsonValueKind.Array:
                        var parts = new List<string>();
                        foreach (var child in prop.Value.EnumerateArray())
                            parts.Add(child.ToString());
                        fields[key] = parts.Count <= 10
                            ? string.Join(", ", parts)
                            : prop.Value.ToString();
                        break;
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        break;
                    default:
                        fields[key] = prop.Value.ToString();
                        break;
                }
            }
        }

        private static List<DataItem> ImportGenericXml(string fullPath)
        {
            var items = new List<DataItem>();
            if (!File.Exists(fullPath)) return items;

            try
            {
                var doc = new XmlDocument();
                doc.Load(fullPath);

                var root = doc.DocumentElement;
                if (root == null) return items;

                int id = 2000;
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;

                    var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (XmlNode child in node.ChildNodes)
                        if (child.NodeType == XmlNodeType.Element)
                            fields[child.Name] = child.InnerText;

                    if (node.Attributes != null)
                        foreach (XmlAttribute attr in node.Attributes)
                            fields[attr.Name] = attr.Value;

                    if (fields.Count > 0)
                        items.Add(MapFieldsToItem(fields, DataSource.XML, id++));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[XML Import] Error: {ex.Message}");
            }

            return items;
        }

        private static List<DataItem> ImportGenericTxt(string fullPath)
        {
            var items = new List<DataItem>();
            if (!File.Exists(fullPath)) return items;

            try
            {
                var lines = File.ReadAllLines(fullPath);
                if (lines.Length < 2) return items;

                char delimiter = DetectDelimiter(lines[0]);

                var rawHeaders = lines[0].Split(delimiter);
                var headers = new string[rawHeaders.Length];
                for (int h = 0; h < rawHeaders.Length; h++)
                    headers[h] = rawHeaders[h].Trim();

                int id = 3000;
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var parts = lines[i].Split(delimiter);

                    var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    int limit = Math.Min(headers.Length, parts.Length);
                    for (int c = 0; c < limit; c++)
                        if (!string.IsNullOrWhiteSpace(headers[c]))
                            fields[headers[c]] = parts[c].Trim();

                    if (fields.Count > 0)
                        items.Add(MapFieldsToItem(fields, DataSource.TXT, id++));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TXT Import] Error: {ex.Message}");
            }

            return items;
        }

        private static char DetectDelimiter(string headerLine)
        {
            char[] candidates = { '|', '\t', ';', ',' };
            char best = '|';
            int bestCount = 0;
            foreach (var c in candidates)
            {
                int count = headerLine.Split(c).Length;
                if (count > bestCount) { bestCount = count; best = c; }
            }
            return best;
        }

        private static readonly HashSet<string> KnownFields =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "Company","Marca","TypeName","Cpu","Ram","Price","Precio",
                "Title","Titulo","Genre","Genero","Sales","Ventas",
                "Platform","Plataforma","Tipo","Type","Modelo","Model",
                "Stock","Cantidad","Minuto","Minute","UsoCPU","CPU",
                "Temperatura","Temperature","Temp","FPS",
                "UserName","Nombre","Name","Email","Correo","Region","Zona"
            };

        private static DataItem MapFieldsToItem(
            Dictionary<string, string> fields, DataSource source, int id)
        {
            var item = new DataItem { Id = id, Source = source };

            if (fields.TryGetValue("Company", out var company))       item.Company = company;
            if (fields.TryGetValue("Marca", out var marca))           item.Company = marca;
            if (fields.TryGetValue("TypeName", out var typeName))     item.TypeName = typeName;
            if (fields.TryGetValue("Cpu", out var cpu))               item.Cpu = cpu;
            if (fields.TryGetValue("Ram", out var ram))               item.Ram = ParseInt(ram);
            if (fields.TryGetValue("Price", out var price))           item.Price = ParseDouble(price);
            if (fields.TryGetValue("Precio", out var precio))         item.Price = ParseDouble(precio);
            if (fields.TryGetValue("Title", out var title))           item.Title = title;
            if (fields.TryGetValue("Titulo", out var titulo))         item.Title = titulo;
            if (fields.TryGetValue("Genre", out var genre))           item.Genre = genre;
            if (fields.TryGetValue("Genero", out var genero))         item.Genre = genero;
            if (fields.TryGetValue("Sales", out var sales))           item.Sales = ParseDouble(sales);
            if (fields.TryGetValue("Ventas", out var ventas))         item.Sales = ParseDouble(ventas);
            if (fields.TryGetValue("Platform", out var platform))     item.Platform = platform;
            if (fields.TryGetValue("Plataforma", out var plat2))      item.Platform = plat2;
            if (fields.TryGetValue("Tipo", out var tipo))             item.Tipo = tipo;
            if (fields.TryGetValue("Type", out var type))             item.Tipo = type;
            if (fields.TryGetValue("Modelo", out var modelo))         item.Modelo = modelo;
            if (fields.TryGetValue("Model", out var model))           item.Modelo = model;
            if (fields.TryGetValue("Stock", out var stock))           item.Stock = ParseInt(stock);
            if (fields.TryGetValue("Cantidad", out var cantidad))     item.Stock = ParseInt(cantidad);
            if (fields.TryGetValue("Minuto", out var minuto))         item.Minuto = ParseInt(minuto);
            if (fields.TryGetValue("Minute", out var minute))         item.Minuto = ParseInt(minute);
            if (fields.TryGetValue("UsoCPU", out var usoCpu))         item.UsoCPU = ParseDouble(usoCpu);
            if (fields.TryGetValue("CPU", out var cpuUse))            item.UsoCPU = ParseDouble(cpuUse);
            if (fields.TryGetValue("Temperatura", out var temp))      item.Temperatura = ParseDouble(temp);
            if (fields.TryGetValue("Temperature", out var temp2))     item.Temperatura = ParseDouble(temp2);
            if (fields.TryGetValue("Temp", out var temp3))            item.Temperatura = ParseDouble(temp3);
            if (fields.TryGetValue("FPS", out var fps))               item.FPS = ParseDouble(fps);
            if (fields.TryGetValue("UserName", out var userName))     item.UserName = userName;
            if (fields.TryGetValue("Nombre", out var nombre))         item.UserName = nombre;
            if (fields.TryGetValue("Name", out var name))             item.UserName = name;
            if (fields.TryGetValue("Email", out var email))           item.Email = email;
            if (fields.TryGetValue("Correo", out var correo))         item.Email = correo;
            if (fields.TryGetValue("Region", out var region))         item.Region = region;
            if (fields.TryGetValue("Zona", out var zona))             item.Region = zona;

            // Store any field that is NOT a known alias → dynamic column
            foreach (var kv in fields)
                if (!KnownFields.Contains(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
                    item.ExtraFields[kv.Key] = kv.Value;

            return item;
        }

        // ── Helpers privados ───────────────────────────────────────────────

        /// <summary>
        /// Si el argumento es ruta absoluta la usa directamente;
        /// si es solo nombre de archivo la combina con DataFolder.
        /// Permite cargar tanto archivos predeterminados como externos.
        /// </summary>
        private static string ResolvePath(string fileNameOrPath) =>
            Path.IsPathRooted(fileNameOrPath)
                ? fileNameOrPath
                : Path.Combine(DataFolder, fileNameOrPath);

        private static string SafeGet(string[] arr, int idx) =>
            idx >= 0 && idx < arr.Length ? arr[idx].Trim() : string.Empty;

        private static string GetJsonString(JsonElement el, string prop) =>
            el.TryGetProperty(prop, out var v) ? v.GetString() ?? string.Empty : string.Empty;

        private static double GetJsonDouble(JsonElement el, string prop) =>
            el.TryGetProperty(prop, out var v) && v.TryGetDouble(out double d) ? d : 0;

        private static int ParseInt(string s) =>
            int.TryParse(s.Trim(), out int v) ? v : 0;

        private static double ParseDouble(string s) =>
            double.TryParse(s.Trim(), NumberStyles.Any,
                CultureInfo.InvariantCulture, out double v) ? v : 0;

        private static List<DataItem> GetSimulatedDbData() =>
        [
            new() { Id = 4001, Source = DataSource.DB, UserName = "Ana Garcia",      Email = "ana@mail.com",    Region = "CDMX"        },
            new() { Id = 4002, Source = DataSource.DB, UserName = "Carlos Lopez",    Email = "carlos@mail.com", Region = "Monterrey"   },
            new() { Id = 4003, Source = DataSource.DB, UserName = "Maria Perez",     Email = "maria@mail.com",  Region = "Guadalajara" },
            new() { Id = 4004, Source = DataSource.DB, UserName = "Juan Rodriguez",  Email = "juan@mail.com",   Region = "CDMX"        },
            new() { Id = 4005, Source = DataSource.DB, UserName = "Laura Torres",    Email = "laura@mail.com",  Region = "Monterrey"   },
            new() { Id = 4006, Source = DataSource.DB, UserName = "Pedro Sanchez",   Email = "pedro@mail.com",  Region = "Puebla"      },
            new() { Id = 4007, Source = DataSource.DB, UserName = "Sofia Martinez",  Email = "sofia@mail.com",  Region = "Guadalajara" },
            new() { Id = 4008, Source = DataSource.DB, UserName = "Diego Hernandez", Email = "diego@mail.com",  Region = "CDMX"        },
        ];
    }
}
