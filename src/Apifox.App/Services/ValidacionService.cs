using Apifox.App.DTOs;
using Apifox.App.Utils;

namespace Apifox.App.Services;

public class ValidacionService
{
    public (bool Valido, string Mensaje) ValidarParaEnvio(FacturaDto factura)
    {
        if (string.IsNullOrWhiteSpace(factura.Numero))
            return (false, "La factura no tiene número.");

        if (factura.Total <= 0)
            return (false, "El total de la factura debe ser mayor a cero.");

        if (!PhoneNormalizer.IsValid(factura.Telefono))
            return (false, "El teléfono del cliente no es válido para WhatsApp.");

        return (true, "Validación exitosa.");
    }

    public string GenerarMensaje(FacturaDto factura)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"*FACTURA ELECTRÓNICA*");
        sb.AppendLine($"-------------------");
        sb.AppendLine($"No. {factura.Numero}");
        sb.AppendLine($"Cliente: {factura.ClienteNombre}");
        sb.AppendLine();
        sb.AppendLine($"*DETALLE:*");

        foreach (var d in factura.Detalles)
        {
            var subtotal = d.Cantidad * d.Precio;
            sb.AppendLine($"• {d.Producto}");
            sb.AppendLine($"  x{d.Cantidad} = ${subtotal:N2}");
        }

        sb.AppendLine();
        sb.AppendLine($"*SUBTOTAL:* ${factura.Total - factura.Iva:N2}");
        sb.AppendLine($"*IVA (15%):* ${factura.Iva:N2}");
        sb.AppendLine($"*TOTAL:* ${factura.Total:N2}");
        sb.AppendLine();
        sb.AppendLine($"Clave Acceso: {factura.ClaveAcceso}");

        return sb.ToString();
    }
}