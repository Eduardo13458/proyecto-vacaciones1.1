# 📘 Documentación del Proyecto — Data Fusion Arena

## 📋 Información General

| Campo | Detalle |
|---|---|
| **Nombre** | Data Fusion Arena |
| **Tipo** | Aplicación de escritorio Windows Forms |
| **Lenguaje** | C# 12.0 |
| **Framework** | .NET 8 (`net8.0-windows`) |
| **IDE** | Visual Studio Community 2026 |
| **Repositorio** | [GitHub — proyecto-vacaciones1.1](https://github.com/Eduardo13458/proyecto-vacaciones1.1) |

---

## 🎯 Objetivo del Proyecto

Data Fusion Arena es una aplicación que **integra, procesa y visualiza datos provenientes de 5 fuentes distintas** (CSV, JSON, XML, TXT y Bases de Datos remotas: SQL Server, MariaDB, PostgreSQL) en una sola interfaz unificada.

El propósito es demostrar el manejo completo de:
- Estructuras de datos (`List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`)
- Algoritmos de ordenamiento manual (sin LINQ)
- Lectura de múltiples formatos de archivo
- Conexión e importación/exportación a bases de datos (SQL Server, MariaDB, PostgreSQL)
- Visualización gráfica y en consola

---

## 🏗️ Arquitectura del Proyecto

```
Prueba 1 proyecto vacaciones/
├── Models/
│   └── DataItem.cs              ← Modelo unificado de datos
├── Data/
│   ├── DataReader.cs            ← Lectura de las 5 fuentes
│   ├── DatabaseExporter.cs      ← Exportación a BD remotas
│   ├── laptops.csv              ← Datos CSV predeterminados
│   ├── videogames.json          ← Datos JSON predeterminados
│   ├── inventory.xml            ← Datos XML predeterminados
│   └── performance.txt          ← Datos TXT predeterminados
├── Processing/
│   └── DataProcessor.cs         ← Filtrado, ordenamiento, agrupación, duplicados
├── Visualization/
│   └── ConsoleVisualizer.cs     ← Tablas y gráficas ASCII/Unicode
├── Form1.cs                     ← Lógica principal del formulario
├── Form1.Designer.cs            ← Diseño de la interfaz gráfica
└── Prueba 1 proyecto vacaciones.csproj
```

### ¿Por qué esta estructura?

Cada carpeta agrupa una **responsabilidad específica**:
- **Models/** → Define *qué* son los datos
- **Data/** → Define *de dónde* vienen y *a dónde* se exportan
- **Processing/** → Define *qué se hace* con los datos (transformaciones)
- **Visualization/** → Define *cómo se muestran* en consola

`Form1.cs` actúa como **orquestador**: conecta todas las piezas y maneja la interfaz de usuario.

---

## 📦 Paquetes NuGet Utilizados

| Paquete | Versión | Propósito |
|---|---|---|
| `Microsoft.Data.SqlClient` | 5.2.2 | Conexión a SQL Server |
| `MySqlConnector` | 2.3.7 | Conexión a MariaDB / MySQL |
| `Npgsql` | 8.0.6 | Conexión a PostgreSQL |
| `System.Windows.Forms.DataVisualization` | 1.0.0-pre | Gráficas (barras, pastel, anillo, líneas) |

---

## 📂 Descripción de Cada Archivo

---

### 1. `Models/DataItem.cs` — Modelo Unificado

**¿Qué hace?** Define la clase `DataItem` que representa **cualquier registro** sin importar de qué fuente proviene.

```
DataItem
├── Id (int)                    ← Identificador único
├── Source (DataSource enum)    ← CSV, JSON, XML, TXT o DB
├── Campos CSV:  Company, TypeName, Cpu, Ram, Price
├── Campos JSON: Title, Genre, Sales, Platform
├── Campos XML:  Tipo, Modelo, Stock
├── Campos TXT:  Minuto, UsoCPU, Temperatura, FPS
├── Campos DB:   UserName, Email, Region
├── ExtraFields (Dictionary<string, string>)  ← Columnas dinámicas
└── Label (string)              ← Etiqueta resumida según fuente
```

**¿Por qué un solo modelo?** Unificar todas las fuentes en una sola clase permite almacenar **todos los registros en una única `List<DataItem>`**, facilitando el procesamiento con los mismos algoritmos sin importar el origen.

**ExtraFields** es un `Dictionary<string, string>` que almacena columnas que no coinciden con las propiedades conocidas. Esto permite importar **cualquier archivo** sin perder información.

---

### 2. `Data/DataReader.cs` — Lectura de las 5 Fuentes

**¿Qué hace?** Contiene los métodos de lectura para cada formato de archivo y para las 3 bases de datos soportadas.

#### Lectores predeterminados (5 fuentes base):

| Método | Formato | Datos | IDs |
|---|---|---|---|
| `ReadCsv()` | CSV | Laptops (Company, Price, Ram...) | 1-999 |
| `ReadJson()` | JSON | Videojuegos (Title, Genre, Sales...) | 1000-1999 |
| `ReadXml()` | XML | Inventario (Tipo, Modelo, Stock) | 2000-2999 |
| `ReadTxt()` | TXT | Log de rendimiento (Temp, FPS, CPU) | 3000-3999 |
| `ReadFromDatabase()` | SQL Server | Usuarios (UserName, Email, Region) | 4000+ |

#### Importación desde Bases de Datos Remotas (3 motores):

Cada método importa registros de la tabla `DatosIntegrados` (creada previamente al exportar). Antes de consultar, **verifica que la BD y la tabla existan** para evitar errores de conexión.

| Método | Motor | Verifica existencia |
|---|---|---|
| `ReadFromSqlServer()` | SQL Server | `EnsureSqlServerDatabase()` + `OBJECT_ID('DatosIntegrados')` |
| `ReadFromMariaDb()` | MariaDB/MySQL | `EnsureMySqlDatabase()` + `information_schema.tables` |
| `ReadFromPostgreSql()` | PostgreSQL | `EnsurePostgreSqlDatabase()` + `information_schema.tables` |

Los 3 métodos comparten un helper común `ReadItemsFromReader(DbDataReader)` que:
1. Lee los nombres de columna del `DbDataReader`
2. Convierte cada fila a un `Dictionary<string, string>`
3. Mapea los campos al modelo `DataItem` usando `MapFieldsToItem()`
4. Almacena campos desconocidos en `ExtraFields`

#### Importadores genéricos (archivos externos):

El método `LoadFromFile(string fullPath)` detecta la extensión del archivo y enruta al importador correcto:

```csharp
return ext switch
{
    ".csv"  => ImportGenericCsv(fullPath),
    ".json" => ImportGenericJson(fullPath),
    ".xml"  => ImportGenericXml(fullPath),
    ".txt"  => ImportGenericTxt(fullPath),
    _ => throw new NotSupportedException(...)
};
```

**¿Cómo maneja archivos desconocidos?**
- Cada importador genérico lee las cabeceras/campos del archivo
- Intenta mapear cada campo a una propiedad conocida de `DataItem` (usando `MapFieldsToItem`)
- Los campos que **no** coinciden se almacenan en `ExtraFields` (el `Dictionary` dinámico)
- Soporta sinónimos: `"Precio"` → `Price`, `"Marca"` → `Company`, `"Ventas"` → `Sales`, etc.

**Manejo de errores:** Cada lector envuelve la lectura en `try/catch`. Si un archivo no existe o tiene errores, el programa continúa sin detenerse.

---

### 3. `Processing/DataProcessor.cs` — Procesamiento de Datos

Este es el archivo central del procesamiento. **No utiliza LINQ en ningún momento** (verificado: no hay `OrderBy`, `Where`, `Select` ni `using System.Linq` en el proyecto).

#### 3.1 Almacenamiento en `List<T>`

Todos los datos se almacenan en una única lista definida en `Form1.cs`:

```csharp
private List<DataItem> _allItems = [];
```

Cada operación de procesamiento recibe esta lista como parámetro y trabaja sobre ella.

#### 3.2 Filtrado

| Método | Descripción | ¿Cómo funciona? |
|---|---|---|
| `FilterLaptopsByMinPrice()` | Laptops con precio ≥ mínimo | Recorre la lista con `foreach`, evalúa condición, agrega a lista resultado |
| `FilterLogsByTemperature()` | Logs con temperatura > umbral | Mismo patrón: `foreach` + condición + `result.Add()` |
| `DynamicFilter()` | Filtra por **cualquier campo numérico** | Usa `GetNumericValue()` para obtener el valor de cualquier campo |
| `ComputeThreshold()` | Calcula percentil (ej: 75%) para umbral | Ordena valores con Insertion Sort manual, toma el valor en la posición del percentil |

**Ejemplo del filtrado dinámico en `btnProcess_Click`:**

```
1. Se descubren los campos numéricos disponibles → ej: ["Price", "Ram"]
2. Se calcula el percentil 75 del primer campo   → ej: threshold = 1200.00
3. Se filtran items donde Price >= 1200.00        → resultado: 25 registros
```

#### 3.3 Ordenamiento Manual (SIN LINQ)

Se implementan **dos algoritmos de ordenamiento clásicos**, ambos sin usar `OrderBy`:

##### Insertion Sort

```
Funcionamiento:
1. Toma el elemento en posición i
2. Lo compara con los anteriores (j = i-1, i-2, ...)
3. Desplaza los mayores una posición a la derecha
4. Inserta el elemento en su posición correcta
5. Repite para i = 1 hasta n-1
```

| Variante | Método | Uso |
|---|---|---|
| Estática | `InsertionSort()` | Ordena por valor predeterminado según fuente (Price/Sales/Stock/Temp) |
| Con selector | `InsertionSortBy()` | Acepta un `Func<DataItem, double>` como clave de ordenamiento |
| Dinámica | `DynamicSort()` | Ordena por **cualquier campo** usando `GetNumericValue()` |

##### Bubble Sort

```
Funcionamiento:
1. Compara elementos adyacentes (j y j+1)
2. Si están en orden incorrecto, los intercambia
3. Repite pasadas hasta que no haya intercambios
4. Optimización: si una pasada completa no hace intercambios, termina
```

| Variante | Método | Uso |
|---|---|---|
| Estática | `BubbleSort()` | Con optimización de corte anticipado |
| Dinámica | `DynamicBubbleSort()` | Por cualquier campo, con corte anticipado |

**Ambos algoritmos se ejecutan en `btnProcess_Click`:**
- Insertion Sort: ordena por el **primer** campo numérico en orden ascendente
- Bubble Sort: ordena por el **segundo** campo numérico en orden descendente

#### 3.4 Agrupación con `Dictionary<TKey, TValue>`

El `Dictionary` se usa extensivamente para agrupar información de forma eficiente (O(n)):

| Método | Tipo del Dictionary | ¿Qué agrupa? |
|---|---|---|
| `GetAvgPriceByBrand()` | `Dictionary<string, double>` | Precio promedio por marca de laptop |
| `GetSalesByGenre()` | `Dictionary<string, double>` | Total de ventas por género de videojuego |
| `GetStockByType()` | `Dictionary<string, int>` | Stock total por tipo de componente |
| `BuildIdIndex()` | `Dictionary<int, DataItem>` | Índice de búsqueda rápida O(1) por ID |
| `GroupBySource()` | `Dictionary<DataSource, List<DataItem>>` | Items agrupados por fuente |
| `DynamicGroupSum()` | `Dictionary<string, double>` | Suma de **cualquier campo** por **cualquier categoría** |
| `DynamicGroupAvg()` | `Dictionary<string, double>` | Promedio de cualquier campo por cualquier categoría |
| `DynamicGroupCount()` | `Dictionary<string, int>` | Conteo de registros por cualquier categoría |

**¿Cómo funciona la agrupación?** Ejemplo con `DynamicGroupSum`:

```
Entrada: items = [...], categoryField = "Company", valueField = "Price"

Paso 1: Crear Dictionary vacío → {}
Paso 2: Para cada item:
   - Obtener categoría: "Dell"
   - Obtener valor: 1200.50
   - Si "Dell" no existe en el Dictionary → agregarlo con 0
   - Sumar: dict["Dell"] += 1200.50
Paso 3: Devolver Dictionary → {"Dell": 15230.00, "HP": 12400.50, ...}
```

Esto se logra en **una sola pasada** (O(n)) gracias al Dictionary, en lugar de recorrer la lista múltiples veces.

#### 3.5 Detección de Duplicados con `HashSet<T>`

```csharp
public static List<DataItem> DetectDuplicates(List<DataItem> items)
{
    var seen  = new HashSet<string>();   // O(1) para verificar existencia
    var dupes = new List<DataItem>();

    foreach (var item in items)
    {
        string key = $"{item.Source}:{item.Label}";
        if (!seen.Add(key))              // si ya existía → es duplicado
            dupes.Add(item);
    }
    return dupes;
}
```

- **Clave compuesta:** `Source` + `Label` identifica cada registro de forma única
- **HashSet:** Permite verificar en O(1) si un elemento ya fue visto
- **Resultado:** Lista con los registros duplicados encontrados

#### 3.6 Acceso Dinámico a Campos

| Método | Propósito |
|---|---|
| `GetStringValue(item, "Company")` | Obtiene el valor de texto de cualquier campo (conocido o ExtraField) |
| `GetNumericValue(item, "Price")` | Obtiene el valor numérico de cualquier campo |
| `DiscoverFields(items)` | Analiza los datos y devuelve qué campos de texto y numéricos tienen datos reales |
| `DiscoverChartPairs(items)` | Encuentra los mejores pares (categoría, número) para gráficas |

Esto permite que el procesamiento sea **completamente dinámico**: funciona con los archivos predeterminados y con **cualquier archivo externo** que el usuario importe.

---

### 4. `Visualization/ConsoleVisualizer.cs` — Visualización en Consola

Genera representaciones de texto para mostrar en el `RichTextBox` de la pestaña "Consola ASCII".

#### 4.1 Tabla Dinámica (`RenderDynamicTable`)

```
╔══════════════════════════════════════════════════════════════╗
║                    DATOS EN CONSOLA                         ║
╚══════════════════════════════════════════════════════════════╝

  Total: 150 registros   Columnas: 8

  ┌────┬────────┬──────────────────┬──────────┐
  │ ID │ Fuente │ Company          │ Price    │
  ├════╪════════╪══════════════════╪══════════┤
  │  1 │ CSV    │ Dell             │  1049.99 │
  │  2 │ CSV    │ HP               │   899.50 │
  └────┴────────┴──────────────────┴──────────┘
```

- Descubre columnas automáticamente usando `DataProcessor.DiscoverFields()`
- Solo muestra columnas que tienen datos reales
- Alineación: texto a la izquierda, números a la derecha
- Caracteres Unicode para bordes (┌ ─ ┬ ┐ │ ├ ═ ╪ ┤ └ ┴ ┘)

#### 4.2 Gráficas ASCII

| Tipo | Método | Representación |
|---|---|---|
| **Barras horizontales** | `RenderBarChart()` | `Dell  │████████████│ 1049.99` |
| **Pastel (porcentajes)** | `RenderPieAscii()` | `Dell  │████████│ 35.2% (1049.99)` |
| **Sparklines** | `RenderSparkLines()` | `Temp  ▁▂▃▄▅▆▇█▇▆▅▄▃▂▁` |
| **Líneas verticales** | `RenderVerticalLineChart()` | Cuadrícula con puntos `●` |

---

### 5. `Data/DatabaseExporter.cs` — Exportación a Bases de Datos

Permite exportar **todos los datos integrados** a 3 motores de base de datos remotos.

#### Proveedores soportados:

| Motor | Método | Paquete |
|---|---|---|
| SQL Server | `ExportToSqlServer()` | `Microsoft.Data.SqlClient` |
| MariaDB / MySQL | `ExportToMariaDb()` | `MySqlConnector` |
| PostgreSQL | `ExportToPostgreSql()` | `Npgsql` |

#### ¿Cómo funciona la exportación?

```
1. ItemToRow()        → Convierte cada DataItem a Dictionary<string, string>
                        Solo incluye campos con datos reales
                        ExtraFields se expanden como columnas individuales

2. DiscoverColumns()  → Recorre TODAS las filas y descubre la unión de columnas
                        (LinkedHashSet mantiene orden de inserción)
                        Id y Fuente siempre van primero

3. BuildCreateSql()   → Genera CREATE TABLE con las columnas descubiertas
                        Adapta sintaxis según proveedor ([col] vs `col` vs "col")

4. BuildInsertSql()   → Genera INSERT INTO con parámetros @p0, @p1, @p2...

5. AddRowParameters() → Asigna valores a cada parámetro (@p0 = valor o DBNull)
```

**Las columnas son dinámicas:** si un archivo importado tiene columnas como "Color" o "Peso", estas se crean automáticamente en la tabla de la BD.

#### Diálogo de conexión:

`ShowConnectionDialog(dbName, defaultPort, actionLabel)` muestra un formulario donde el usuario ingresa servidor, puerto, base de datos, usuario y contraseña. Para SQL Server también ofrece autenticación Windows.

El parámetro `actionLabel` diferencia visualmente la acción:
- **Importar**: `"Conectar e Importar"` — botón del diálogo indica que se traerán datos
- **Exportar**: `"Conectar y Exportar"` (valor por defecto) — indica que se enviarán datos

#### Métodos de verificación de BD (`internal`):

Los métodos `Ensure*Database()` son `internal` para que `DataReader.cs` pueda invocarlos al importar, verificando que la BD exista antes de consultarla:

| Método | Motor | Acceso |
|---|---|---|
| `EnsureSqlServerDatabase()` | SQL Server | `internal` |
| `EnsureMySqlDatabase()` | MariaDB/MySQL | `internal` |
| `EnsurePostgreSqlDatabase()` | PostgreSQL | `internal` |

---

### 6. `Form1.cs` — Orquestador Principal

Conecta todas las piezas. Contiene los handlers de todos los botones.

#### Campos principales:

```csharp
private List<DataItem> _allItems = [];              // TODA la información
private List<DataItem> _lastImportedItems = [];     // Último lote importado
private Dictionary<int, DataItem> _idIndex = [];    // Búsqueda rápida por ID
```

#### Botones y funcionalidad:

| Botón | Handler | ¿Qué hace? |
|---|---|---|
| **Procesar** | `btnProcess_Click` | Ejecuta los 6 pasos de procesamiento dinámico |
| **Ver Consola** | `btnConsole_Click` | Muestra tabla dinámica en RichTextBox |
| **CSV / JSON / XML / TXT** | `ImportFile()` | Importa archivo externo con `OpenFileDialog` |
| **📥 SQL Srv** | `btnImportSqlServer_Click` | Importa registros desde SQL Server (`DatosIntegrados`) |
| **📥 MariaDB** | `btnImportMariaDb_Click` | Importa registros desde MariaDB (`DatosIntegrados`) |
| **📥 PostgreSQL** | `btnImportPostgre_Click` | Importa registros desde PostgreSQL (`DatosIntegrados`) |
| **Limpiar Todo** | `btnClearData_Click` | Limpia `_allItems`, `_idIndex`, gráficas y consola |
| **⚡ Generar Gráfica** | `btnGenerateChart_Click` | Auto-detecta datos y llena 4 gráficas |
| **📊 Gráfica Consola** | `btnChartConsole_Click` | Barras + pastel + sparklines + líneas ASCII |
| **📤 SQL Server** | `btnExportSql_Click` | Exporta a SQL Server remoto |
| **📤 MariaDB** | `btnExportMariaDb_Click` | Exporta a MariaDB remota |
| **📤 PostgreSQL** | `btnExportPostgre_Click` | Exporta a PostgreSQL remoto |

Los 3 botones de importar BD usan el método genérico `ImportFromDb(dbName, port, readFunc)` que:
1. Muestra el diálogo de conexión con `actionLabel = "Conectar e Importar"`
2. Llama al `ReadFrom*()` correspondiente de `DataReader`
3. Agrega los registros a `_allItems` y refresca la UI

Los 3 botones de exportar BD usan el método genérico `ExportToDb(dbName, exportAction)` que:
1. Muestra el diálogo de conexión con `actionLabel = "Conectar y Exportar"` (default)
2. Llama al `ExportTo*()` correspondiente de `DatabaseExporter`
3. Muestra un `MessageBox` con el resultado

#### Los 6 pasos de `btnProcess_Click` (Procesar):

```
┌─────────────────────────────────────────────────────────────┐
│              PROCESAMIENTO DINÁMICO DE DATOS                 │
├─────────────────────────────────────────────────────────────┤
│ 1. INSERTION SORT  → Ordena por primer campo numérico ASC   │
│ 2. FILTRO          → Items > percentil 75 del campo         │
│ 3. BUBBLE SORT     → Ordena por segundo campo numérico DESC │
│ 4. AGRUPACIÓN      → Suma + Promedio + Conteo (Dictionary)  │
│ 5. DUPLICADOS      → Detección con HashSet                  │
│ 6. BÚSQUEDA POR ID → Consulta O(1) con Dictionary           │
└─────────────────────────────────────────────────────────────┘
```

---

### 7. `Form1.Designer.cs` — Interfaz Gráfica

Define los controles de la ventana:

```
┌──────────────────────────────────────────────────────────────────────┐
│  DATA FUSION ARENA               [Procesar] [Ver Consola]            │ ← pnlTop
├──────────────────────────────────────────────────────────────────────┤
│  📥 IMPORTAR:  [CSV] [JSON] [XML] [TXT]                             │
│                [📥 SQL Srv] [📥 MariaDB] [📥 PostgreSQL]             │ ← pnlFiles
│                [Limpiar] [⚡Gráfica] [📊 Gráfica Consola]            │
├──────────────────────────────────────────────────────────────────────┤
│  📤 EXPORTAR a BD:  [📤 SQL Server] [📤 MariaDB] [📤 PostgreSQL]    │ ← pnlExport
├──────────────────────────────────────────────────────────────────────┤
│  ┌─ Todos los Datos ─┬─ Consola ASCII ─┬─ ⚡ Gráfica ──┐            │
│  │                    │                  │                │            │ ← tabControl
│  │   DataGridView     │   RichTextBox    │  4 Charts en   │            │
│  │   (tabla datos)    │   (consola)      │  TableLayout   │            │
│  └────────────────────┴──────────────────┴────────────────┘            │
└──────────────────────────────────────────────────────────────────────┘
```

> **Distinción visual Importar vs Exportar:**
> - Los botones de importar BD llevan el prefijo **📥** y están en el panel superior (fondo azulado `45,45,68`)
> - Los botones de exportar BD llevan el prefijo **📤** y están en el panel inferior (fondo cálido `50,35,30`)
> - Las etiquetas de cada panel son **negritas** y con colores distintos (azul claro vs naranja)

**Pestañas (TabControl):**
1. **Todos los Datos** — `DataGridView` con filtro por fuente y colores por tipo
2. **Consola ASCII** — `RichTextBox` con fondo oscuro para tablas y gráficas
3. **⚡ Gráfica Generada** — 4 gráficas en cuadrícula 2×2 (`TableLayoutPanel`)

#### Distribución de gráficas en `tlpAutoCharts` (2×2):

```
┌──────────────────────────┬──────────────────────────┐
│  chartAutoBar            │  chartAutoPie            │
│  (Barras / Column)       │  (Pastel / Pie)          │
│  Dock = Fill             │  Dock = Fill             │
├──────────────────────────┼──────────────────────────┤
│  chartAutoDoughnut       │  chartAutoLine           │
│  (Anillo / Doughnut)     │  (Líneas / Line)         │
│  Dock = Fill             │  Dock = Fill             │
└──────────────────────────┴──────────────────────────┘
```

Cada control `Chart` se instancia en `InitializeComponent()` con `new Chart()`, se configura con `Dock = DockStyle.Fill` y una `ChartArea("Default")`, y se agrega a la celda correspondiente del `TableLayoutPanel`:

```csharp
tlpAutoCharts.Controls.Add(chartAutoBar,      0, 0);  // Arriba-izquierda
tlpAutoCharts.Controls.Add(chartAutoPie,      1, 0);  // Arriba-derecha
tlpAutoCharts.Controls.Add(chartAutoDoughnut, 0, 1);  // Abajo-izquierda
tlpAutoCharts.Controls.Add(chartAutoLine,     1, 1);  // Abajo-derecha
```

---

## ✅ Cumplimiento de la Rúbrica

### 1. Almacenamiento en `List<T>`

| Requisito | Implementación | Ubicación |
|---|---|---|
| Almacenar todos los datos en una lista | `private List<DataItem> _allItems = [];` | `Form1.cs`, línea 18 |
| Los datos importados se agregan a la misma lista | `_allItems.AddRange(newItems);` | `Form1.cs`, `ImportFile()` |
| Los datos de BD se agregan a la misma lista | `_allItems.AddRange(dbItems);` | `Form1.cs`, `ImportFromDb()` |

**Todo el procesamiento opera sobre esta lista.** Cada método de `DataProcessor` recibe `List<DataItem> items` como parámetro.

---

### 2. Creación de `Dictionary<TKey, TValue>` para Agrupar

| Dictionary | Clave | Valor | Método |
|---|---|---|---|
| `Dictionary<int, DataItem>` | ID | Item completo | `BuildIdIndex()` |
| `Dictionary<string, double>` | Marca | Precio promedio | `GetAvgPriceByBrand()` |
| `Dictionary<string, double>` | Género | Total ventas | `GetSalesByGenre()` |
| `Dictionary<string, int>` | Tipo | Stock total | `GetStockByType()` |
| `Dictionary<DataSource, List<DataItem>>` | Fuente | Lista de items | `GroupBySource()` |
| `Dictionary<string, double>` | Cualquier categoría | Suma numérica | `DynamicGroupSum()` |
| `Dictionary<string, double>` | Cualquier categoría | Promedio numérico | `DynamicGroupAvg()` |
| `Dictionary<string, int>` | Cualquier categoría | Conteo | `DynamicGroupCount()` |

---

### 3. Filtrado de Datos

| Filtro | Método | Descripción |
|---|---|---|
| Estático | `FilterLaptopsByMinPrice()` | Laptops con precio ≥ valor mínimo |
| Estático | `FilterLogsByTemperature()` | Logs con temperatura > umbral |
| Dinámico | `DynamicFilter()` | Cualquier campo numérico ≥ valor |
| Percentil | `ComputeThreshold()` | Calcula umbral basado en percentil |

El filtrado se aplica en `btnProcess_Click` → Paso 2.

---

### 4. Ordenamiento Manual (SIN LINQ)

| Algoritmo | Variantes | ¿Dónde se ejecuta? |
|---|---|---|
| **Insertion Sort** | `InsertionSort()`, `InsertionSortBy()`, `DynamicSort()` | `btnProcess_Click` → Paso 1 (ASC) |
| **Bubble Sort** | `BubbleSort()`, `DynamicBubbleSort()` | `btnProcess_Click` → Paso 3 (DESC) |

**Verificación de ausencia de LINQ:** Se buscó en todo el proyecto las cadenas `OrderBy`, `OrderByDescending`, `orderby`, `.Select(`, `.Where(` y `using System.Linq` — **todas retornaron vacío**. El proyecto no usa LINQ en absoluto.

---

### 5. Detección de Duplicados

| Estructura | Uso | Método |
|---|---|---|
| `HashSet<string>` | Verifica unicidad en O(1) | `DetectDuplicates()` |

Se ejecuta en `btnProcess_Click` → Paso 5.

---

### 6. Lectura de Múltiples Fuentes

| Fuente | Formato | Método | Librería |
|---|---|---|---|
| Laptops | CSV | `ReadCsv()` | `File.ReadAllLines` + split manual |
| Videojuegos | JSON | `ReadJson()` | `System.Text.Json` |
| Inventario | XML | `ReadXml()` | `System.Xml.XmlDocument` |
| Rendimiento | TXT | `ReadTxt()` | `File.ReadAllLines` + split por `\|` |
| BD SQL Server | SQL Server | `ReadFromSqlServer()` | `Microsoft.Data.SqlClient` |
| BD MariaDB | MariaDB | `ReadFromMariaDb()` | `MySqlConnector` |
| BD PostgreSQL | PostgreSQL | `ReadFromPostgreSql()` | `Npgsql` |

Los 3 importadores de BD:
- Verifican existencia de la BD con `Ensure*Database()` (métodos `internal` de `DatabaseExporter`)
- Verifican existencia de la tabla `DatosIntegrados` antes de hacer `SELECT *`
- Retornan `[]` (lista vacía) si la tabla no existe, sin generar error
- Comparten `ReadItemsFromReader(DbDataReader)` para mapear filas a `DataItem`

---

### 7. Visualización

| Tipo | Tecnología | Control / Método | Ubicación |
|---|---|---|---|
| Tabla de datos | `DataGridView` con colores por fuente | `dgvData` | Pestaña "Todos los Datos" |
| Tabla consola | Unicode (┌─┬┐│├═╪┤└┴┘) | `RenderDynamicTable()` | Pestaña "Consola ASCII" |
| Gráfica de barras | `Chart` — `SeriesChartType.Column` | `chartAutoBar` (0,0) | Pestaña "⚡ Gráfica Generada" |
| Gráfica de pastel | `Chart` — `SeriesChartType.Pie` | `chartAutoPie` (1,0) | Pestaña "⚡ Gráfica Generada" |
| Gráfica de anillo | `Chart` — `SeriesChartType.Doughnut` | `chartAutoDoughnut` (0,1) | Pestaña "⚡ Gráfica Generada" |
| Gráfica de líneas | `Chart` — `SeriesChartType.Line` | `chartAutoLine` (1,1) | Pestaña "⚡ Gráfica Generada" |
| Barras ASCII | Bloques █ | `RenderBarChart()` | Pestaña "Consola ASCII" |
| Pastel ASCII | Porcentajes + barras | `RenderPieAscii()` | Pestaña "Consola ASCII" |
| Sparklines | Caracteres ▁▂▃▄▅▆▇█ | `RenderSparkLines()` | Pestaña "Consola ASCII" |
| Líneas ASCII | Cuadrícula con ● | `RenderVerticalLineChart()` | Pestaña "Consola ASCII" |

Los 4 controles `Chart` se instancian con `new Chart()` en `InitializeComponent()` y se ubican en un `TableLayoutPanel` de 2×2 (`tlpAutoCharts`), cada uno con `Dock = DockStyle.Fill` para ocupar toda su celda. Es fundamental que los charts se creen (`new Chart()`) y se agreguen al panel (`tlpAutoCharts.Controls.Add(chart, col, row)`) dentro de `InitializeComponent()`, de lo contrario quedarán como `null` y provocarán errores al intentar limpiar o generar gráficas.

---

### 8. Exportación a Bases de Datos

| Motor | Método | Columnas |
|---|---|---|
| SQL Server | `ExportToSqlServer()` | Dinámicas (descubiertas de los datos) |
| MariaDB | `ExportToMariaDb()` | Dinámicas |
| PostgreSQL | `ExportToPostgreSql()` | Dinámicas |

Cada exportación:
1. Crea la base de datos si no existe (`Ensure*Database()`)
2. Elimina la tabla anterior si existe
3. Crea la tabla con columnas dinámicas
4. Inserta todos los registros

---

## 🔄 Flujo Completo de la Aplicación

```
                    ┌─────────────────┐
                    │  Usuario abre   │
                    │  la aplicación  │
                    └────────┬────────┘
                             │
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
      [Importar CSV/   [📥 Importar BD]  [Importar otro
       JSON/XML/TXT]   [SQL/Maria/PG]     archivo]
              │              │              │
              ▼              ▼              ▼
         ┌──────────────────────────────────────┐
         │     _allItems (List<DataItem>)        │
         │     Almacena TODOS los registros      │
         └──────────────┬───────────────────────┘
                        │
         ┌──────────────┼──────────────┬──────────────┐
         ▼              ▼              ▼              ▼
   [Procesar]    [Ver Consola]   [Generar       [📤 Exportar
                                  Gráfica]       a BD]
         │              │              │              │
         ▼              ▼              ▼              ▼
   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
   │ Sort ×2  │  │ Tabla    │  │ 4 Charts │  │ SQL/Maria│
   │ Filtro   │  │ dinámica │  │ auto-    │  │ DB/Postgre│
   │ Grupos   │  │ Unicode  │  │ generadas│  │ dinámico │
   │ Dupes    │  │          │  │          │  │          │
   │ Búsqueda │  │          │  │          │  │          │
   └──────────┘  └──────────┘  └──────────┘  └──────────┘
```

---

## 📝 Resumen de Estructuras de Datos Utilizadas

| Estructura | Uso Principal | Complejidad |
|---|---|---|
| `List<DataItem>` | Almacenamiento principal de todos los registros | Acceso O(1) por índice, búsqueda O(n) |
| `Dictionary<TKey, TValue>` | Agrupación, indexación por ID, conteo | Búsqueda/inserción O(1) promedio |
| `HashSet<string>` | Detección de duplicados, descubrimiento de campos | Verificación de existencia O(1) |
| `DataTable` | Alimentar el DataGridView con columnas dinámicas | N/A (framework UI) |

---

## 🚫 Lo que NO se usa (y por qué)

| Tecnología | Razón |
|---|---|
| **LINQ** (`OrderBy`, `Where`, `Select`) | La rúbrica requiere ordenamiento manual con algoritmos propios |
| **Entity Framework** | Se usa ADO.NET directo para demostrar manejo de conexiones y SQL |
| **Bibliotecas externas de gráficas** | Se usa el control `Chart` nativo de Windows Forms |

---