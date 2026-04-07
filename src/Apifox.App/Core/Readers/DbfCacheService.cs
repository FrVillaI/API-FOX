using System.Collections.Generic;
using Apifox.App.DTOs;
using Apifox.App.Core.Readers;

namespace Apifox.App.Core.Readers;

public class DbfCacheService
{
    public Dictionary<string, ClienteDto> Clientes { get; private set; } = new();
    public Dictionary<string, ArticuloDto> Articulos { get; private set; } = new();
    public Dictionary<string, DocElectronicoDto> DocsElectronicos { get; private set; } = new();

    private readonly string _rutaDbfDir;
    private readonly ClienteReader _clienteReader;
    private readonly ArticuloReader _articuloReader;
    private readonly DocElectronicoReader _docElectronicoReader;

    public DbfCacheService(string rutaDbfDir, ClienteReader clienteReader, ArticuloReader articuloReader, DocElectronicoReader docElectronicoReader)
    {
        _rutaDbfDir = rutaDbfDir;
        _clienteReader = clienteReader;
        _articuloReader = articuloReader;
        _docElectronicoReader = docElectronicoReader;
        LoadAll();
    }

    public void LoadAll()
    {
        Clientes = _clienteReader.ReadClientes(_rutaDbfDir);
        Articulos = _articuloReader.ReadArticulos(_rutaDbfDir);
        DocsElectronicos = _docElectronicoReader.ReadDocsElectronicos(_rutaDbfDir);
    }

    public void Recargar()
    {
        LoadAll();
    }
}
