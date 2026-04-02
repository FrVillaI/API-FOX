namespace Apifox.App.DTOs;

public class FacturaDto
{
    public string Numero { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Iva { get; set; }
    public DateTime Fecha { get; set; }
    public bool Autorizado { get; set; }
    public string ClaveAcceso { get; set; } = string.Empty;
    public List<DetalleDto> Detalles { get; set; } = new();
}

public class DetalleDto
{
    public string Producto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
}

public class FacturaResumenDto
{
    public string Numero { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public bool Autorizado { get; set; }
    public string Telefono { get; set; } = string.Empty;
}

public class ClienteDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ArticuloDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class DocElectronicoDto
{
    public string Numero { get; set; } = string.Empty;
    public string ClaveAcceso { get; set; } = string.Empty;
    public bool Autorizado { get; set; }
}