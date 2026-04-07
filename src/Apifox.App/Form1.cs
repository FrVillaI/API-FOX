using Apifox.App.Services;
using Apifox.App.Utils;
using Microsoft.Extensions.Configuration;

namespace Apifox.App;

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

        CargarConfiguracion();
        ConfigurarDataGrid();
    }

    private void CargarConfiguracion()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var baseUrl = config["WhatsApp:BaseUrl"];
        _whatsAppService.SetBaseUrl(baseUrl);
    }

    private void ConfigurarDataGrid()
    {
        dgvFacturas.Columns.Clear();
        dgvFacturas.Columns.Add("Numero", "Número");
        dgvFacturas.Columns.Add("Cliente", "Cliente");
        dgvFacturas.Columns.Add("Total", "Total");
        dgvFacturas.Columns.Add("Autorizado", "Autorizado");
        dgvFacturas.Columns.Add("Telefono", "Teléfono");

        dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvFacturas.ReadOnly = true;
        dgvFacturas.AllowUserToAddRows = false;
    }

    private void btnCargar_Click(object sender, EventArgs e)
    {
        var ruta = txtRutaDbf.Text.Trim();

        if (string.IsNullOrEmpty(ruta))
        {
            MessageBox.Show("Ingrese ruta DBF.");
            return;
        }

        _facturaService.SetRutaDbf(ruta);

        if (!_facturaService.ValidarRuta())
        {
            MessageBox.Show("Ruta inválida.");
            return;
        }

        var facturas = _facturaService.ObtenerRecientes(20);

        dgvFacturas.Rows.Clear();

        foreach (var f in facturas)
        {
            dgvFacturas.Rows.Add(f.Numero, f.Cliente, f.Total, f.Autorizado ? "Sí" : "No", f.Telefono);
        }

        lblStatus.Text = $"Cargadas {facturas.Count}";
    }

    private async void btnEnviar_Click(object sender, EventArgs e)
    {
        if (dgvFacturas.SelectedRows.Count == 0)
        {
            MessageBox.Show("Seleccione una factura.");
            return;
        }

        btnEnviar.Enabled = false;

        try
        {
            var numero = dgvFacturas.SelectedRows[0].Cells["Numero"].Value?.ToString();

            var factura = _facturaService.ObtenerFactura(numero);

            if (factura == null)
            {
                MessageBox.Show("No se pudo obtener la factura.");
                return;
            }

            var (valido, msgVal) = _validacionService.ValidarParaEnvio(factura);

            if (!valido)
            {
                MessageBox.Show(msgVal);
                return;
            }

            lblStatus.Text = "Verificando WhatsApp...";
            Application.DoEvents();

            var health = await _whatsAppService.GetHealth();

            if (health?.Service == null)
            {
                MessageBox.Show("No se pudo conectar al servicio.");
                return;
            }

            switch (health.Service.Status)
            {
                case "ready":
                    break;

                case "qr":
                    MessageBox.Show("Escanea el QR en:\nhttp://localhost:3000/qr-page");
                    return;

                case "disconnected":
                    MessageBox.Show("WhatsApp desconectado.");
                    return;

                case "error":
                    MessageBox.Show($"Error: {health.LastError}");
                    return;

                default:
                    MessageBox.Show("Estado desconocido.");
                    return;
            }

            var telefono = PhoneNormalizer.Normalize(factura.Telefono);
            var mensaje = _validacionService.GenerarMensaje(factura);

            lblStatus.Text = $"Enviando a {telefono}...";
            Application.DoEvents();

            var (exito, msg) = await _whatsAppService.EnviarMensaje(telefono, mensaje);

            if (exito)
            {
                MessageBox.Show("Enviado correctamente.");
                lblStatus.Text = "Enviado";
            }
            else
            {
                MessageBox.Show(msg);
                lblStatus.Text = "Error";
            }
        }
        finally
        {
            btnEnviar.Enabled = true;
        }
    }
}