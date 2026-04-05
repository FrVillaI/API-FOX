using System.Data;
using Apifox.App.DTOs;
using DbfReader = DbfDataReader.DbfDataReader;

namespace Apifox.App.Services;

public class FacturaService
{
    private string _rutaDbf = string.Empty;
    private Dictionary<string, ClienteDto> _clientesCache = new();
    private Dictionary<string, ArticuloDto> _articulosCache = new();
    private Dictionary<string, DocElectronicoDto> _docsElectronicosCache = new();

    public void SetRutaDbf(string ruta)
    {
        _rutaDbf = ruta;
        _clientesCache.Clear();
        _articulosCache.Clear();
        _docsElectronicosCache.Clear();
        Logger.Log($"=== NUEVA SESION ===");
        Logger.Log($"Ruta DBF establecida: {_rutaDbf}");
    }

    public bool ValidarRuta()
    {
        var existe = Directory.Exists(_rutaDbf);
        Logger.Log($"ValidarRuta: {(existe ? "OK" : "FALLO")}");
        return existe;
    }

    private string GetPath(string fileName) => Path.Combine(_rutaDbf, fileName);

    #region HELPERS

    private string SafeGetString(IDataRecord row, string field)
    {
        try { return row[field]?.ToString()?.Trim() ?? ""; }
        catch { return ""; }
    }

    private decimal SafeGetDecimal(IDataRecord row, string field)
    {
        try 
        { 
            var val = row[field];
            if (val == null || val == DBNull.Value) return 0;
            return Convert.ToDecimal(val); 
        }
        catch { return 0; }
    }

    private DateTime? SafeGetDate(IDataRecord row, string field)
    {
        try 
        { 
            var val = row[field];
            if (val == null || val == DBNull.Value) return null;
            if (val is DateTime dt) return dt;
            return DateTime.Parse(val.ToString()!); 
        }
        catch { return null; }
    }

    private bool SafeGetBool(IDataRecord row, string field)
    {
        try 
        { 
            var val = row[field];
            if (val == null || val == DBNull.Value) return false;
            if (val is bool b) return b;
            var s = val.ToString()?.ToUpper();
            return s == "T" || s == "Y" || s == "1";
        }
        catch { return false; }
    }

    #endregion

    #region CARGA CACHE

    private void CargarClientes()
    {
        Logger.Log("Cargando clientes...");
        try
        {
            var path = GetPath("clientes.dbf");
            if (!File.Exists(path))
            {
                Logger.Log("  clientes.dbf no encontrado");
                return;
            }

            var reader = new DbfReader(path);
            int count = 0;
            while (reader.Read())
            {
                var codigo = SafeGetString(reader, "CODCLI");
                if (string.IsNullOrEmpty(codigo)) continue;

                var cliente = new ClienteDto
                {
                    Codigo = codigo,
                    Nombre = SafeGetString(reader, "NOMCLI"),
                    Telefono = SafeGetString(reader, "TLF1"),
                    Email = ""
                };
                _clientesCache[codigo] = cliente;
                count++;
            }
            reader.Dispose();
            Logger.Log($"  clientes cargados: {count}");
        }
        catch (Exception ex)
        {
            Logger.Log($"  ERROR carga clientes: {ex.Message}");
        }
    }

    private void CargarArticulos()
    {
        Logger.Log("Cargando articulos...");
        try
        {
            var path = GetPath("articulos.dbf");
            if (!File.Exists(path))
            {
                Logger.Log("  articulos.dbf no encontrado");
                return;
            }

            var reader = new DbfReader(path);
            int count = 0;
            while (reader.Read())
            {
                var codigo = SafeGetString(reader, "CODART");
                if (string.IsNullOrEmpty(codigo)) continue;

                var articulo = new ArticuloDto
                {
                    Codigo = codigo,
                    Descripcion = SafeGetString(reader, "NOMART"),
                    Precio = SafeGetDecimal(reader, "PRECIO_A")
                };
                _articulosCache[codigo] = articulo;
                count++;
            }
            reader.Dispose();
            Logger.Log($"  articulos cargados: {count}");
        }
        catch (Exception ex)
        {
            Logger.Log($"  ERROR carga articulos: {ex.Message}");
        }
    }

    private void CargarDocsElectronicos()
    {
        Logger.Log("Cargando docs electronicos...");
        try
        {
            var path = GetPath("doc_electronicos.dbf");
            if (!File.Exists(path))
            {
                Logger.Log("  doc_electronicos.dbf no encontrado");
                return;
            }

            var reader = new DbfReader(path);
            int count = 0;
            while (reader.Read())
            {
                var numero = SafeGetString(reader, "NUMDOC");
                if (string.IsNullOrEmpty(numero)) continue;

                var doc = new DocElectronicoDto
                {
                    Numero = numero,
                    ClaveAcceso = SafeGetString(reader, "CLAVE"),
                    Autorizado = SafeGetBool(reader, "PUBLICADO")
                };
                _docsElectronicosCache[numero] = doc;
                count++;
            }
            reader.Dispose();
            Logger.Log($"  docs electronicos cargados: {count}");
        }
        catch (Exception ex)
        {
            Logger.Log($"  ERROR carga docs electronicos: {ex.Message}");
        }
    }

    #endregion

public List<FacturaResumenDto> ObtenerRecientes(int cantidad = 20)
{
    Logger.Log("=== OBTENER RECIENTES OPTIMIZADO ===");

    CargarClientes();
    CargarDocsElectronicos(); // artículos no necesarios aquí

    var topFacturas = new List<FacturaResumenDto>();

    try
    {
        var path = GetPath("facturas.dbf");
        if (!File.Exists(path))
        {
            Logger.Log("ERROR: facturas.dbf no encontrado");
            return topFacturas;
        }

        var reader = new DbfReader(path);

        while (reader.Read())
        {
            var numero = SafeGetString(reader, "NUMFAC");
            if (string.IsNullOrEmpty(numero)) continue;

            var clienteCod = SafeGetString(reader, "CLIENTE");
            var total = SafeGetDecimal(reader, "TOTAL");
            var fecha = SafeGetDate(reader, "EMISION") ?? DateTime.MinValue;

            _clientesCache.TryGetValue(clienteCod, out var cliente);
            _docsElectronicosCache.TryGetValue(numero, out var docElec);

            var factura = new FacturaResumenDto
            {
                Numero = numero,
                Cliente = cliente?.Nombre ?? clienteCod,
                Fecha = fecha,
                Total = total,
                Autorizado = docElec?.Autorizado ?? false,
                Telefono = cliente?.Telefono ?? ""
            };

            // 🔥 lógica TOP N
            if (topFacturas.Count < cantidad)
            {
                topFacturas.Add(factura);
            }
            else
            {
                // buscar el más antiguo
                var minFecha = topFacturas.Min(f => f.Fecha);

                if (factura.Fecha > minFecha)
                {
                    var idx = topFacturas.FindIndex(f => f.Fecha == minFecha);
                    topFacturas[idx] = factura;
                }
            }
        }

        reader.Dispose();
    }
    catch (Exception ex)
    {
        Logger.Log($"ERROR ObtenerRecientes: {ex.Message}");
    }

    return topFacturas
        .OrderByDescending(f => f.Fecha)
        .ToList();
}

    public FacturaDto? ObtenerFactura(string numero)
    {
        Logger.Log($"=== OBTENER FACTURA: {numero} ===");
        
        try
        {
            var path = GetPath("facturas.dbf");
            if (!File.Exists(path))
            {
                Logger.Log("ERROR: facturas.dbf no encontrado");
                return null;
            }

            var reader = new DbfReader(path);
            
            while (reader.Read())
            {
                var num = SafeGetString(reader, "NUMFAC");
                if (num != numero) continue;

                Logger.Log($"Factura encontrada: {num}");

                var clienteCod = SafeGetString(reader, "CLIENTE");
                var total = SafeGetDecimal(reader, "TOTAL");
                var iva = SafeGetDecimal(reader, "IVA");
                var fecha = SafeGetDate(reader, "EMISION") ?? DateTime.Now;

                _clientesCache.TryGetValue(clienteCod, out var cliente);
                _docsElectronicosCache.TryGetValue(numero, out var docElec);

                Logger.Log($"  Cliente: {clienteCod} -> {cliente?.Nombre}");
                Logger.Log($"  Total: {total}, IVA: {iva}");
                Logger.Log($"  Autorizado: {docElec?.Autorizado ?? false}");

                var factura = new FacturaDto
                {
                    Numero = num,
                    ClienteNombre = cliente?.Nombre ?? clienteCod,
                    Telefono = cliente?.Telefono ?? "",
                    Total = total,
                    Iva = iva,
                    Fecha = fecha,
                    Autorizado = docElec?.Autorizado ?? false,
                    ClaveAcceso = docElec?.ClaveAcceso ?? "",
                    Detalles = ObtenerDetalles(numero)
                };

                reader.Dispose();
                Logger.Log($"  Detalles: {factura.Detalles.Count}");
                return factura;
            }

            reader.Dispose();
            Logger.Log($"ERROR: Factura {numero} no encontrada");
        }
        catch (Exception ex)
        {
            Logger.Log($"ERROR ObtenerFactura: {ex.Message}");
            Logger.Log($"Stack: {ex.StackTrace}");
        }

        return null;
    }

    private List<DetalleDto> ObtenerDetalles(string numeroFactura)
    {
        Logger.Log($"  Obteniendo detalles de: {numeroFactura}");
        var detalles = new List<DetalleDto>();

        try
        {
            var path = GetPath("tranfac.dbf");
            if (!File.Exists(path))
            {
                Logger.Log("  ERROR: tranfac.dbf no encontrado");
                return detalles;
            }

            var reader = new DbfReader(path);
            
            int leidos = 0;
            while (reader.Read())
            {
                if (SafeGetString(reader, "NUMFAC") != numeroFactura)
                    continue;

                var articuloCod = SafeGetString(reader, "CODART");
                var cantidad = SafeGetDecimal(reader, "CANTIDAD");
                var precio = SafeGetDecimal(reader, "PRECIO");

                _articulosCache.TryGetValue(articuloCod, out var articulo);

                var nombreProducto = articulo?.Descripcion ?? articuloCod;
                
                detalles.Add(new DetalleDto
                {
                    Producto = nombreProducto,
                    Cantidad = cantidad,
                    Precio = precio
                });
                leidos++;
            }
            reader.Dispose();
            Logger.Log($"  Detalles leidos: {leidos}");
        }
        catch (Exception ex)
        {
            Logger.Log($"  ERROR ObtenerDetalles: {ex.Message}");
        }

        return detalles;
    }
}
