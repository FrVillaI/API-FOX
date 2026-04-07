using System.Data;
using System.IO;
using Apifox.App.DTOs;
using Apifox.App.Core.Readers;
using Apifox.App.Core.Utils;
using DbfReader = DbfDataReader.DbfDataReader;

namespace Apifox.App.Services;

public class FacturaService
{
    private string _rutaDbf = string.Empty;
    private DbfCacheService? _dbfCache;
    private FacturaReader? _facturaReader;
    // Phase 1 extended: local caches (kept for compatibility with existing methods)
    private Dictionary<string, ClienteDto> _clientesCache = new();
    private Dictionary<string, ArticuloDto> _articulosCache = new();
    private Dictionary<string, DocElectronicoDto> _docsElectronicosCache = new();

    public void SetRutaDbf(string ruta)
    {
        _rutaDbf = ruta;
        // initialize readers/cache (Phase 1 extended)
        var cr = new ClienteReader();
        var ar = new ArticuloReader();
        var dr = new DocElectronicoReader();
        _dbfCache = new DbfCacheService(ruta, cr, ar, dr);
        _facturaReader = new FacturaReader(Path.Combine(ruta, "facturas.dbf"));
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
    // Las cargas de DBF han sido refactorizadas a Readers y a DbfCacheService (Phase 1 extendido)
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
