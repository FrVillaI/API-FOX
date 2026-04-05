using System.Collections.Generic;
using System.IO;
using DbfReader = DbfDataReader.DbfDataReader;
using Apifox.App.DTOs;
using Apifox.App.Core.Utils;

namespace Apifox.App.Core.Readers;

public class ArticuloReader
{
    public Dictionary<string, ArticuloDto> ReadArticulos(string rutaDbfDir)
    {
        var dict = new Dictionary<string, ArticuloDto>();
        var path = Path.Combine(rutaDbfDir, "articulos.dbf");
        if (!File.Exists(path)) return dict;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            var codigo = DbfFieldHelper.SafeGetString(reader, "CODART");
            if (string.IsNullOrEmpty(codigo)) continue;
            var articulo = new ArticuloDto
            {
                Codigo = codigo,
                Descripcion = DbfFieldHelper.SafeGetString(reader, "NOMART"),
                Precio = DbfFieldHelper.SafeGetDecimal(reader, "PRECIO_A")
            };
            dict[codigo] = articulo;
        }
        return dict;
    }
}
