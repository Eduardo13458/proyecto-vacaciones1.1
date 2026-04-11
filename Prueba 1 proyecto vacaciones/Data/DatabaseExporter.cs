using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using Prueba_1_proyecto_vacaciones.Models;

namespace Prueba_1_proyecto_vacaciones.Data
{
    /// <summary>
    /// Exporta datos CSV (laptops) a SQL Server, MariaDB y PostgreSQL.
    /// Crea la tabla si no existe e inserta los registros.
    /// </summary>
    public static class DatabaseExporter
    {
        private const string TableName = "Laptops";

        // ════════════════════════════════════════════════════════════════════
        //  SQL SERVER
        // ════════════════════════════════════════════════════════════════════

        public static int ExportToSqlServer(List<DataItem> items, string connectionString)
        {
            var csvItems = FilterCsv(items);
            if (csvItems.Count == 0) return 0;

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Crear tabla si no existe
            string createSql =
                $"""
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{TableName}')
                CREATE TABLE {TableName} (
                    Id       INT PRIMARY KEY,
                    Company  NVARCHAR(100),
                    TypeName NVARCHAR(100),
                    Cpu      NVARCHAR(200),
                    Ram      INT,
                    Price    FLOAT
                )
                """;
            using (var cmd = new SqlCommand(createSql, conn))
                cmd.ExecuteNonQuery();

            // Limpiar datos previos
            using (var cmd = new SqlCommand($"DELETE FROM {TableName}", conn))
                cmd.ExecuteNonQuery();

            // Insertar registros
            int count = 0;
            foreach (var item in csvItems)
            {
                string insertSql =
                    $"INSERT INTO {TableName} (Id, Company, TypeName, Cpu, Ram, Price) " +
                    $"VALUES (@Id, @Company, @TypeName, @Cpu, @Ram, @Price)";

                using var cmd = new SqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Company", item.Company);
                cmd.Parameters.AddWithValue("@TypeName", item.TypeName);
                cmd.Parameters.AddWithValue("@Cpu", item.Cpu);
                cmd.Parameters.AddWithValue("@Ram", item.Ram);
                cmd.Parameters.AddWithValue("@Price", item.Price);
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
            var csvItems = FilterCsv(items);
            if (csvItems.Count == 0) return 0;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string createSql =
                $"""
                CREATE TABLE IF NOT EXISTS {TableName} (
                    Id       INT PRIMARY KEY,
                    Company  VARCHAR(100),
                    TypeName VARCHAR(100),
                    Cpu      VARCHAR(200),
                    Ram      INT,
                    Price    DOUBLE
                )
                """;
            using (var cmd = new MySqlCommand(createSql, conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new MySqlCommand($"DELETE FROM {TableName}", conn))
                cmd.ExecuteNonQuery();

            int count = 0;
            foreach (var item in csvItems)
            {
                string insertSql =
                    $"INSERT INTO {TableName} (Id, Company, TypeName, Cpu, Ram, Price) " +
                    $"VALUES (@Id, @Company, @TypeName, @Cpu, @Ram, @Price)";

                using var cmd = new MySqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Company", item.Company);
                cmd.Parameters.AddWithValue("@TypeName", item.TypeName);
                cmd.Parameters.AddWithValue("@Cpu", item.Cpu);
                cmd.Parameters.AddWithValue("@Ram", item.Ram);
                cmd.Parameters.AddWithValue("@Price", item.Price);
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
            var csvItems = FilterCsv(items);
            if (csvItems.Count == 0) return 0;

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string createSql =
                $"""
                CREATE TABLE IF NOT EXISTS {TableName} (
                    Id       INT PRIMARY KEY,
                    Company  VARCHAR(100),
                    TypeName VARCHAR(100),
                    Cpu      VARCHAR(200),
                    Ram      INT,
                    Price    DOUBLE PRECISION
                )
                """;
            using (var cmd = new NpgsqlCommand(createSql, conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand($"DELETE FROM {TableName}", conn))
                cmd.ExecuteNonQuery();

            int count = 0;
            foreach (var item in csvItems)
            {
                string insertSql =
                    $"INSERT INTO {TableName} (Id, Company, TypeName, Cpu, Ram, Price) " +
                    $"VALUES (@Id, @Company, @TypeName, @Cpu, @Ram, @Price)";

                using var cmd = new NpgsqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Company", item.Company);
                cmd.Parameters.AddWithValue("@TypeName", item.TypeName);
                cmd.Parameters.AddWithValue("@Cpu", item.Cpu);
                cmd.Parameters.AddWithValue("@Ram", item.Ram);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.ExecuteNonQuery();
                count++;
            }

            return count;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DIALOGO DE CONEXION
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Muestra un formulario simple para que el usuario ingrese los datos
        /// de conexion a la base de datos destino.
        /// </summary>
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
                Text = dbName == "SQL Server" ? "" : "root",
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
                Checked = dbName == "SQL Server",
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

        // ── Helper ─────────────────────────────────────────────────────────

        private static List<DataItem> FilterCsv(List<DataItem> items)
        {
            var result = new List<DataItem>();
            foreach (var item in items)
                if (item.Source == DataSource.CSV)
                    result.Add(item);
            return result;
        }
    }
}
