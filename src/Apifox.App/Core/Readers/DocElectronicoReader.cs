using System.Collections.Generic;
using System.IO;
using DbfReader = DbfDataReader.DbfDataReader;
using Apifox.App.DTOs;
using Apifox.App.Core.Utils;

namespace Apifox.App.Core.Readers;

public class DocElectronicoReader
{
    public Dictionary<string, DocElectronicoDto> ReadDocsElectronicos(string rutaDbfDir)
    {
        var dict = new Dictionary<string, DocElectronicoDto>();
        var path = Path.Combine(rutaDbfDir, "doc_electronicos.dbf");
        if (!File.Exists(path)) return dict;

        using var reader = new DbfReader(path);
        while (reader.Read())
        {
            var numero = DbfFieldHelper.SafeGetString(reader, "NUMDOC");
            if (string.IsNullOrEmpty(numero)) continue;
            var doc = new DocElectronicoDto
            {
                Numero = numero,
                ClaveAcceso = DbfFieldHelper.SafeGetString(reader, "CLAVE"),
                Autorizado = DbfFieldHelper.SafeGetBool(reader, "PUBLICADO")
            };
            dict[numero] = doc;
        }
        return dict;
    }
}
