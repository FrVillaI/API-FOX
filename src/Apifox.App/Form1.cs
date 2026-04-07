namespace Apifox.App;

using Apifox.App.DTOs;
using Apifox.App.Services;
using Apifox.App.Utils;

public partial class Form1 : Form
{
    private readonly FacturaService _facturaService;
    private readonly ValidacionService _validacionService;
    private readonly WhatsAppService _whatsAppService;

    public Form1()
    {
        InitializeComponent();
        _facturaService = new FacturaService();
        _validacionService = new ValidacionService();
        _whatsAppService = new WhatsAppService();
        ConfigurarDataGrid();
    }

    private void ConfigurarDataGrid()
    {
        dgvFacturas.Columns.Clear();
        dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Numero", HeaderText = "Número", Width = 100 });
        dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cliente", HeaderText = "Cliente", Width = 200 });
        dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", Width = 80 });
        dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Autorizado", HeaderText = "Autorizado", Width = 80 });
        dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 120 });
        dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvFacturas.MultiSelect = false;
        dgvFacturas.ReadOnly = true;
        dgvFacturas.AllowUserToAddRows = false;
        dgvFacturas.AllowUserToDeleteRows = false;
    }

    private void btnCargar_Click(object sender, EventArgs e)
    {
        var ruta = txtRutaDbf.Text.Trim();

        if (string.IsNullOrEmpty(ruta))
        {
            MessageBox.Show("Ingrese la ruta de los archivos DBF.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _facturaService.SetRutaDbf(ruta);

        if (!_facturaService.ValidarRuta())
        {
            MessageBox.Show("La ruta ingresada no existe o no es válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        lblStatus.Text = "Cargando facturas...";
        Application.DoEvents();

        try
        {
            var facturas = _facturaService.ObtenerRecientes(20);
            dgvFacturas.Rows.Clear();

            foreach (var f in facturas)
            {
                dgvFacturas.Rows.Add(f.Numero, f.Cliente, f.Total.ToString("N2"), f.Autorizado ? "Sí" : "No", f.Telefono);
            }

            lblStatus.Text = $"Cargadas {facturas.Count} facturas.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar facturas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error al cargar.";
        }
    }

    private void btnBuscar_Click(object sender, EventArgs e)
    {
        var numero = txtBuscar.Text.Trim();

        if (string.IsNullOrEmpty(numero))
        {
            MessageBox.Show("Ingrese el número de factura a buscar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvFacturas.Rows.Count == 0)
        {
            MessageBox.Show("Cargue primero las facturas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        foreach (DataGridViewRow row in dgvFacturas.Rows)
        {
            if (row.Cells["Numero"].Value?.ToString() == numero)
            {
                row.Selected = true;
                dgvFacturas.FirstDisplayedScrollingRowIndex = row.Index;
                lblStatus.Text = $"Factura {numero} encontrada.";
                return;
            }
        }

        MessageBox.Show($"Factura {numero} no encontrada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void btnEnviar_Click(object sender, EventArgs e)
    {
        if (dgvFacturas.SelectedRows.Count == 0)
        {
            MessageBox.Show("Seleccione una factura para enviar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var numero = dgvFacturas.SelectedRows[0].Cells["Numero"].Value?.ToString();

        if (string.IsNullOrEmpty(numero))
            return;

        var resultado = MessageBox.Show($"¿Enviar factura {numero} por WhatsApp?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (resultado != DialogResult.Yes)
            return;

        lblStatus.Text = "Obteniendo factura completa...";
        Application.DoEvents();

        var factura = _facturaService.ObtenerFactura(numero);

        if (factura == null)
        {
            MessageBox.Show("No se pudo obtener los detalles de la factura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error.";
            return;
        }

        var (valido, mensajeValidacion) = _validacionService.ValidarParaEnvio(factura);

        if (!valido)
        {
            MessageBox.Show(mensajeValidacion, "Validación fallida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblStatus.Text = "Validación fallida.";
            return;
        }

        var telefono = PhoneNormalizer.Normalize(factura.Telefono);
        var mensaje = _validacionService.GenerarMensaje(factura);

        lblStatus.Text = $"Enviando a {telefono}...";
        Application.DoEvents();

        var (exito, mensajeEnvio) = await _whatsAppService.EnviarMensaje(telefono, mensaje);

        if (exito)
        {
            MessageBox.Show("Mensaje enviado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblStatus.Text = "Enviado.";
        }
        else
        {
            MessageBox.Show(mensajeEnvio, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error al enviar.";
        }
    }

    private void btnVerDetalle_Click(object sender, EventArgs e)
    {
        if (dgvFacturas.SelectedRows.Count == 0)
        {
            MessageBox.Show("Seleccione una factura para ver el detalle.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var numero = dgvFacturas.SelectedRows[0].Cells["Numero"].Value?.ToString();

        if (string.IsNullOrEmpty(numero))
            return;

        var factura = _facturaService.ObtenerFactura(numero);

        if (factura == null)
        {
            MessageBox.Show("No se pudo obtener los detalles de la factura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Factura: {factura.Numero}");
        sb.AppendLine($"Cliente: {factura.ClienteNombre}");
        sb.AppendLine($"Teléfono: {factura.Telefono}");
        sb.AppendLine($"Fecha: {factura.Fecha:dd/MM/yyyy}");
        sb.AppendLine($"Total: ${factura.Total:N2}");
        sb.AppendLine($"IVA: ${factura.Iva:N2}");
        sb.AppendLine($"Autorizado: {(factura.Autorizado ? "Sí" : "No")}");
        sb.AppendLine($"Clave Acceso: {factura.ClaveAcceso}");
        sb.AppendLine();
        sb.AppendLine("Detalles:");

        foreach (var d in factura.Detalles)
        {
            var subtotal = d.Cantidad * d.Precio;
            sb.AppendLine($"  - {d.Producto}: x{d.Cantidad} = ${subtotal:N2}");
        }

        MessageBox.Show(sb.ToString(), $"Detalle Factura {numero}", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}