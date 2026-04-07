using System.Collections.Generic;
using System.Linq;
using DbfReader = DbfDataReader.DbfDataReader;
using Apifox.App.DTOs;
using Apifox.App.Core.Utils;

namespace Apifox.App.Core.Readers;

public class FacturaReader
{
    private readonly string _rutaFacturasDbf;

    public FacturaReader(string rutaFacturasDbf)
    {
        _rutaFacturasDbf = rutaFacturasDbf;
    }

    public List<FacturaResumenDto> ObtenerRecientes(int cantidad,
        Dictionary<string, ClienteDto> clientes,
        Dictionary<string, DocElectronicoDto> docsElectronicos,
        Dictionary<string, ArticuloDto> articulos)
    {
        var topFacturas = new List<FacturaResumenDto>();
        if (string.IsNullOrEmpty(_rutaFacturasDbf)) return topFacturas;

        var path = _rutaFacturasDbf;
        if (!System.IO.File.Exists(path)) return topFacturas;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            var numero = DbfFieldHelper.SafeGetString(reader, "NUMFAC");
            if (string.IsNullOrEmpty(numero)) continue;

            var clienteCod = DbfFieldHelper.SafeGetString(reader, "CLIENTE");
            var total = DbfFieldHelper.SafeGetDecimal(reader, "TOTAL");
            var fecha = DbfFieldHelper.SafeGetDate(reader, "EMISION") ?? DateTime.MinValue;

            clientes.TryGetValue(clienteCod, out var cliente);
            docsElectronicos.TryGetValue(numero, out var docElec);

            var factura = new FacturaResumenDto
            {
                Numero = numero,
                Cliente = cliente?.Nombre ?? clienteCod,
                Fecha = fecha,
                Total = total,
                Autorizado = docElec?.Autorizado ?? false,
                Telefono = cliente?.Telefono ?? ""
            };

            if (topFacturas.Count < cantidad)
            {
                topFacturas.Add(factura);
            }
            else
            {
                var minFecha = topFacturas.Min(f => f.Fecha);
                if (factura.Fecha > minFecha)
                {
                    var idx = topFacturas.FindIndex(f => f.Fecha == minFecha);
                    topFacturas[idx] = factura;
                }
            }
        }

        return topFacturas
            .OrderByDescending(f => f.Fecha)
            .ToList();
    }

    public FacturaDto? ObtenerFactura(string numero,
        Dictionary<string, ClienteDto> clientes,
        Dictionary<string, DocElectronicoDto> docsElectronicos,
        Dictionary<string, ArticuloDto> articulos,
        string rutaFacturasDbf,
        System.Func<string, FacturaDto> hook = null)
    {
        if (string.IsNullOrEmpty(rutaFacturasDbf)) return null;
        var path = rutaFacturasDbf;
        if (!System.IO.File.Exists(path)) return null;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            var num = DbfFieldHelper.SafeGetString(reader, "NUMFAC");
            if (num != numero) continue;

            var clienteCod = DbfFieldHelper.SafeGetString(reader, "CLIENTE");
            var total = DbfFieldHelper.SafeGetDecimal(reader, "TOTAL");
            var iva = DbfFieldHelper.SafeGetDecimal(reader, "IVA");
            var fecha = DbfFieldHelper.SafeGetDate(reader, "EMISION") ?? DateTime.Now;

            clientes.TryGetValue(clienteCod, out var cliente);
            docsElectronicos.TryGetValue(numero, out var docElec);

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
                Detalles = ObtenerDetalles(numero, articulos, rutaFacturasDbf)
            };

            return factura;
        }
        return null;
    }

    private List<DetalleDto> ObtenerDetalles(string numeroFactura,
        Dictionary<string, ArticuloDto> articulos,
        string rutaTranfacDbf)
    {
        var detalles = new List<DetalleDto>();
        var path = rutaTranfacDbf;
        if (!System.IO.File.Exists(path)) return detalles;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            if (DbfFieldHelper.SafeGetString(reader, "NUMFAC") != numeroFactura) continue;
            var articuloCod = DbfFieldHelper.SafeGetString(reader, "CODART");
            var cantidad = DbfFieldHelper.SafeGetDecimal(reader, "CANTIDAD");
            var precio = DbfFieldHelper.SafeGetDecimal(reader, "PRECIO");

            articulos.TryGetValue(articuloCod, out var articulo);
            var nombreProducto = articulo?.Descripcion ?? articuloCod;

            detalles.Add(new DetalleDto
            {
                Producto = nombreProducto,
                Cantidad = cantidad,
                Precio = precio
            });
        }

        return detalles;
    }
}
