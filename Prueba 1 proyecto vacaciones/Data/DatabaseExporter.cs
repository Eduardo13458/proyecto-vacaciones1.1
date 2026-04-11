using System.Data.Common;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using Prueba_1_proyecto_vacaciones.Models;

namespace Prueba_1_proyecto_vacaciones.Data
{
    /// <summary>
    /// Exporta todos los datos integrados a SQL Server, MariaDB y PostgreSQL.
    /// Las columnas de la tabla se descubren dinamicamente a partir de los datos
    /// cargados: cada propiedad no vacia y cada clave de ExtraFields se convierte
    /// en su propia columna.
    /// </summary>
    public static class DatabaseExporter
    {
        private const string TableName = "DatosIntegrados";

        // ════════════════════════════════════════════════════════════════════
        //  SQL SERVER
        // ════════════════════════════════════════════════════════════════════

        public static int ExportToSqlServer(List<DataItem> items, string connectionString)
        {
            if (items.Count == 0) return 0;

            EnsureSqlServerDatabase(connectionString);

            var rows = items.Select(ItemToRow).ToList();
            var columns = DiscoverColumns(rows);

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using (var cmd = new SqlCommand(
                $"IF OBJECT_ID('{TableName}', 'U') IS NOT NULL DROP TABLE [{TableName}]", conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new SqlCommand(BuildCreateSql(columns, "sqlserver"), conn))
                cmd.ExecuteNonQuery();

            int count = 0;
            string insertSql = BuildInsertSql(columns, "sqlserver");
            foreach (var row in rows)
            {
                using var cmd = new SqlCommand(insertSql, conn);
                AddRowParameters(cmd.Parameters, columns, row);
                cmd.ExecuteNonQuery();
                count++;
            }

            return count;
        }

        // ════════════════════════════════════════════════════════════════════
        //  MARIADB / MYSQL
        // ════════════════════════════════════════════════════════════════════

        public static int ExportToMariaDb(List<DataItem> items, string connectionString)
        {
            if (items.Count == 0) return 0;

            EnsureMySqlDatabase(connectionString);

            var rows = items.Select(ItemToRow).ToList();
            var columns = DiscoverColumns(rows);

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            using (var cmd = new MySqlCommand($"DROP TABLE IF EXISTS `{TableName}`", conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new MySqlCommand(BuildCreateSql(columns, "mysql"), conn))
                cmd.ExecuteNonQuery();

            int count = 0;
            string insertSql = BuildInsertSql(columns, "mysql");
            foreach (var row in rows)
            {
                using var cmd = new MySqlCommand(insertSql, conn);
                AddRowParameters(cmd.Parameters, columns, row);
                cmd.ExecuteNonQuery();
                count++;
            }

            return count;
        }

        // ════════════════════════════════════════════════════════════════════
        //  POSTGRESQL
        // ════════════════════════════════════════════════════════════════════

        public static int ExportToPostgreSql(List<DataItem> items, string connectionString)
        {
            if (items.Count == 0) return 0;

            EnsurePostgreSqlDatabase(connectionString);

            var rows = items.Select(ItemToRow).ToList();
            var columns = DiscoverColumns(rows);

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using (var cmd = new NpgsqlCommand(
                $"DROP TABLE IF EXISTS \"{TableName}\"", conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand(BuildCreateSql(columns, "pgsql"), conn))
                cmd.ExecuteNonQuery();

            int count = 0;
            string insertSql = BuildInsertSql(columns, "pgsql");
            foreach (var row in rows)
            {
                using var cmd = new NpgsqlCommand(insertSql, conn);
                AddRowParameters(cmd.Parameters, columns, row);
                cmd.ExecuteNonQuery();
                count++;
            }

            return count;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DIALOGO DE CONEXION
        // ════════════════════════════════════════════════════════════════════

        public static string? ShowConnectionDialog(string dbName, string defaultPort)
        {
            using var frm = new Form
            {
                Text = $"Conexion a {dbName}",
                Size = new Size(420, 310),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(240, 240, 245)
            };

            int y = 15;
            var lblServer = new Label { Text = "Servidor:", Location = new Point(15, y), AutoSize = true };
            var txtServer = new TextBox { Text = "localhost", Location = new Point(130, y - 3), Width = 250 };
            y += 35;

            var lblPort = new Label { Text = "Puerto:", Location = new Point(15, y), AutoSize = true };
            var txtPort = new TextBox { Text = defaultPort, Location = new Point(130, y - 3), Width = 250 };
            y += 35;

            var lblDb = new Label { Text = "Base de datos:", Location = new Point(15, y), AutoSize = true };
            var txtDb = new TextBox { Text = "DataFusionArena", Location = new Point(130, y - 3), Width = 250 };
            y += 35;

            var lblUser = new Label { Text = "Usuario:", Location = new Point(15, y), AutoSize = true };
            var txtUser = new TextBox
            {
                Text = dbName == "SQL Server" ? "sa" : "root",
                Location = new Point(130, y - 3),
                Width = 250
            };
            y += 35;

            var lblPass = new Label { Text = "Contraseña:", Location = new Point(15, y), AutoSize = true };
            var txtPass = new TextBox
            {
                Location = new Point(130, y - 3),
                Width = 250,
                UseSystemPasswordChar = true
            };
            y += 35;

            var chkWinAuth = new CheckBox
            {
                Text = "Autenticacion Windows (solo SQL Server)",
                Location = new Point(130, y),
                AutoSize = true,
                Checked = false,
                Visible = dbName == "SQL Server"
            };
            y += 35;

            var btnOk = new Button
            {
                Text = "Conectar y Exportar",
                DialogResult = DialogResult.OK,
                Location = new Point(130, y),
                Size = new Size(150, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            var btnCancel = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(290, y),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat
            };

            frm.Controls.AddRange([
                lblServer, txtServer, lblPort, txtPort,
                lblDb, txtDb, lblUser, txtUser,
                lblPass, txtPass, chkWinAuth, btnOk, btnCancel
            ]);
            frm.AcceptButton = btnOk;
            frm.CancelButton = btnCancel;

            if (frm.ShowDialog() != DialogResult.OK)
                return null;

            string server = txtServer.Text.Trim();
            string port = txtPort.Text.Trim();
            string db = txtDb.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            if (dbName == "SQL Server")
            {
                if (chkWinAuth.Checked)
                    return $"Server={server},{port};Database={db};" +
                           $"Trusted_Connection=true;TrustServerCertificate=true;";
                else
                    return $"Server={server},{port};Database={db};" +
                           $"User Id={user};Password={pass};TrustServerCertificate=true;";
            }

            if (dbName == "MariaDB")
            {
                return $"Server={server};Port={port};Database={db};" +
                       $"User={user};Password={pass};";
            }

            // PostgreSQL
            return $"Host={server};Port={port};Database={db};" +
                   $"Username={user};Password={pass};";
        }

        // ════════════════════════════════════════════════════════════════════
        //  DYNAMIC COLUMN HELPERS
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convierte un DataItem a un diccionario clave-valor con solo los
        /// campos que contienen datos.  Los ExtraFields se expanden como
        /// columnas individuales.
        /// </summary>
        private static Dictionary<string, string> ItemToRow(DataItem item)
        {
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = item.Id.ToString(CultureInfo.InvariantCulture),
                ["Fuente"] = item.Source.ToString()
            };

            string label = item.Label;
            if (!string.IsNullOrEmpty(label))
                row["Etiqueta"] = label;

            // Propiedades conocidas: solo agregar si tienen valor significativo
            if (!string.IsNullOrEmpty(item.Company))   row["Company"]     = item.Company;
            if (!string.IsNullOrEmpty(item.TypeName))   row["TypeName"]    = item.TypeName;
            if (!string.IsNullOrEmpty(item.Cpu))        row["Cpu"]         = item.Cpu;
            if (item.Ram != 0)                          row["Ram"]         = item.Ram.ToString(CultureInfo.InvariantCulture);
            if (item.Price != 0)                        row["Price"]       = item.Price.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(item.Title))      row["Title"]       = item.Title;
            if (!string.IsNullOrEmpty(item.Genre))      row["Genre"]       = item.Genre;
            if (item.Sales != 0)                        row["Sales"]       = item.Sales.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(item.Platform))   row["Platform"]    = item.Platform;
            if (!string.IsNullOrEmpty(item.Tipo))       row["Tipo"]        = item.Tipo;
            if (!string.IsNullOrEmpty(item.Modelo))     row["Modelo"]      = item.Modelo;
            if (item.Stock != 0)                        row["Stock"]       = item.Stock.ToString(CultureInfo.InvariantCulture);
            if (item.Minuto != 0)                       row["Minuto"]      = item.Minuto.ToString(CultureInfo.InvariantCulture);
            if (item.UsoCPU != 0)                       row["UsoCPU"]      = item.UsoCPU.ToString(CultureInfo.InvariantCulture);
            if (item.Temperatura != 0)                  row["Temperatura"] = item.Temperatura.ToString(CultureInfo.InvariantCulture);
            if (item.FPS != 0)                          row["FPS"]         = item.FPS.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(item.UserName))   row["UserName"]    = item.UserName;
            if (!string.IsNullOrEmpty(item.Email))      row["Email"]       = item.Email;
            if (!string.IsNullOrEmpty(item.Region))     row["Region"]      = item.Region;

            // ExtraFields: cada clave se convierte en su propia columna
            foreach (var kv in item.ExtraFields)
            {
                string col = SanitizeColumnName(kv.Key);
                if (!string.IsNullOrEmpty(col) && !row.ContainsKey(col))
                    row[col] = kv.Value;
            }

            return row;
        }

        /// <summary>
        /// Recorre todas las filas y devuelve la lista ordenada de columnas
        /// (union de todas las claves).  Id y Fuente siempre van primero.
        /// </summary>
        private static List<string> DiscoverColumns(List<Dictionary<string, string>> rows)
        {
            var set = new LinkedHashSet();

            // Columnas fijas al inicio
            set.Add("Id");
            set.Add("Fuente");
            set.Add("Etiqueta");

            foreach (var row in rows)
                foreach (var key in row.Keys)
                    set.Add(key);

            return set.ToList();
        }

        /// <summary>Limpia un nombre de campo para usarlo como nombre de columna SQL.</summary>
        private static string SanitizeColumnName(string name)
        {
            var sb = new StringBuilder(name.Length);
            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else if (c == ' ')
                    sb.Append('_');
            }
            return sb.ToString();
        }

        /// <summary>Entrecomilla un nombre de columna segun el proveedor.</summary>
        private static string QuoteColumn(string col, string provider)
        {
            return provider switch
            {
                "sqlserver" => $"[{col}]",
                "mysql"     => $"`{col}`",
                "pgsql"     => $"\"{col}\"",
                _           => col
            };
        }

        /// <summary>Genera CREATE TABLE con todas las columnas descubiertas.</summary>
        private static string BuildCreateSql(List<string> columns, string provider)
        {
            string varcharType = provider == "sqlserver" ? "NVARCHAR(500)" : "VARCHAR(500)";
            string tableName = provider switch
            {
                "sqlserver" => $"[{TableName}]",
                "mysql"     => $"`{TableName}`",
                "pgsql"     => $"\"{TableName}\"",
                _           => TableName
            };

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {tableName} (");

            for (int i = 0; i < columns.Count; i++)
            {
                string quoted = QuoteColumn(columns[i], provider);
                sb.Append($"    {quoted} {varcharType}");
                if (i < columns.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>Genera INSERT INTO con parametros @p0, @p1, …</summary>
        private static string BuildInsertSql(List<string> columns, string provider)
        {
            string tableName = provider switch
            {
                "sqlserver" => $"[{TableName}]",
                "mysql"     => $"`{TableName}`",
                "pgsql"     => $"\"{TableName}\"",
                _           => TableName
            };

            var cols = string.Join(", ", columns.Select(c => QuoteColumn(c, provider)));
            var pars = string.Join(", ", columns.Select((_, i) => $"@p{i}"));

            return $"INSERT INTO {tableName} ({cols}) VALUES ({pars})";
        }

        /// <summary>Agrega los parametros @p0..@pN al comando a partir del diccionario fila.</summary>
        private static void AddRowParameters(
            DbParameterCollection col, List<string> columns, Dictionary<string, string> row)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                string paramName = $"@p{i}";
                object value = row.TryGetValue(columns[i], out string? v)
                    ? v
                    : (object)DBNull.Value;

                switch (col)
                {
                    case SqlParameterCollection sql:
                        sql.AddWithValue(paramName, value);
                        break;
                    case MySqlParameterCollection mysql:
                        mysql.AddWithValue(paramName, value);
                        break;
                    case NpgsqlParameterCollection npg:
                        npg.AddWithValue(paramName, value);
                        break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Ensure Database Exists
        // ════════════════════════════════════════════════════════════════════

        private static void EnsureMySqlDatabase(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            string dbName = builder.Database;
            builder.Database = string.Empty;

            using var conn = new MySqlConnection(builder.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                $"CREATE DATABASE IF NOT EXISTS `{dbName}`", conn);
            cmd.ExecuteNonQuery();
        }

        private static void EnsureSqlServerDatabase(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using var conn = new SqlConnection(builder.ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]", conn);
            cmd.ExecuteNonQuery();
        }

        private static void EnsurePostgreSqlDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            string dbName = builder.Database!;
            builder.Database = "postgres";

            using var conn = new NpgsqlConnection(builder.ConnectionString);
            conn.Open();

            using var checkCmd = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @db", conn);
            checkCmd.Parameters.AddWithValue("@db", dbName);
            var exists = checkCmd.ExecuteScalar();

            if (exists is null)
            {
                using var createCmd = new NpgsqlCommand(
                    $"CREATE DATABASE \"{dbName}\"", conn);
                createCmd.ExecuteNonQuery();
            }
        }

        // ── LinkedHashSet auxiliar (mantiene orden de insercion) ────────────

        private sealed class LinkedHashSet
        {
            private readonly List<string> _list = [];
            private readonly HashSet<string> _set = new(StringComparer.OrdinalIgnoreCase);

            public void Add(string item)
            {
                if (_set.Add(item))
                    _list.Add(item);
            }

            public List<string> ToList() => [.. _list];
        }
    }
}
