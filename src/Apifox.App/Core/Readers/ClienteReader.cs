using System.Collections.Generic;
using System.IO;
using DbfReader = DbfDataReader.DbfDataReader;
using Apifox.App.DTOs;
using Apifox.App.Core.Utils;

namespace Apifox.App.Core.Readers;

public class ClienteReader
{
    public Dictionary<string, ClienteDto> ReadClientes(string rutaDbfDir)
    {
        var dict = new Dictionary<string, ClienteDto>();
        var path = Path.Combine(rutaDbfDir, "clientes.dbf");
        if (!File.Exists(path)) return dict;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            var codigo = DbfFieldHelper.SafeGetString(reader, "CODCLI");
            if (string.IsNullOrEmpty(codigo)) continue;
            var cliente = new ClienteDto
            {
                Codigo = codigo,
                Nombre = DbfFieldHelper.SafeGetString(reader, "NOMCLI"),
                Telefono = DbfFieldHelper.SafeGetString(reader, "TLF1"),
                Email = ""
            };
            dict[codigo] = cliente;
        }
        return dict;
    }
}
