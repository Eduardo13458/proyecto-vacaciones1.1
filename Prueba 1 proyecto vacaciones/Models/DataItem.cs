namespace Prueba_1_proyecto_vacaciones.Models
{
    /// <summary>Identifica el origen de cada DataItem.</summary>
    public enum DataSource { CSV, JSON, XML, TXT, DB }

    /// <summary>
    /// Modelo central que unifica registros de 5 fuentes distintas.
    /// Solo los campos correspondientes al <see cref="Source"/> son relevantes.
    /// </summary>
    public class DataItem
    {
        // ── Metadatos ──────────────────────────────────────────────────────
        public int Id { get; set; }
        public DataSource Source { get; set; }

        // ── CSV · Laptops ──────────────────────────────────────────────────
        public string Company  { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string Cpu      { get; set; } = string.Empty;
        public int    Ram      { get; set; }
        public double Price    { get; set; }

        // ── JSON · Videojuegos ─────────────────────────────────────────────
        public string Title    { get; set; } = string.Empty;
        public string Genre    { get; set; } = string.Empty;
        public double Sales    { get; set; }
        public string Platform { get; set; } = string.Empty;

        // ── XML · Inventario ───────────────────────────────────────────────
        public string Tipo   { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int    Stock  { get; set; }

        // ── TXT · Log de rendimiento ───────────────────────────────────────
        public int    Minuto      { get; set; }
        public double UsoCPU      { get; set; }
        public double Temperatura { get; set; }
        public double FPS         { get; set; }

        // ── DB · Usuarios ──────────────────────────────────────────────────
        public string UserName { get; set; } = string.Empty;
        public string Email    { get; set; } = string.Empty;
        public string Region   { get; set; } = string.Empty;

        // ── Campos extra (columnas no mapeadas de archivos importados) ─────
        public Dictionary<string, string> ExtraFields { get; set; } = [];

        /// <summary>Etiqueta resumida segun la fuente del registro.</summary>
        public string Label => Source switch
        {
            DataSource.CSV  => $"{Company} {TypeName}",
            DataSource.JSON => Title,
            DataSource.XML  => $"{Tipo} - {Modelo}",
            DataSource.TXT  => $"Min {Minuto:D2}",
            DataSource.DB   => UserName,
            _               => $"ID:{Id}"
        };

        public override string ToString() => $"[{Source}] {Label}";
    }
}
