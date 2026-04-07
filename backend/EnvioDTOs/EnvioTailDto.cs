namespace EnvioBackend.DTOs;

public class EnvioTailDto
{
    public string FacturaId { get; set; } = string.Empty;
    public int Intentos { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
