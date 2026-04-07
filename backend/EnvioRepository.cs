using System;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace EnvioBackend.Data;

public class EnvioRepository
{
    private readonly string _connectionString;

    public EnvioRepository(string connectionString)
    {
        _connectionString = connectionString;
        InicializarEsquema();
    }

    private void InicializarEsquema()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
CREATE TABLE IF NOT EXISTS envios (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  FacturaId TEXT NOT NULL,
  Intentos INTEGER NOT NULL DEFAULT 0,
  Estado TEXT,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME
);
";

        connection.Execute(sql);
    }
}
