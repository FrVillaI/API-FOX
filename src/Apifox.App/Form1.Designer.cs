namespace Apifox.App;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.TextBox txtRutaDbf;
    private System.Windows.Forms.Button btnCargar;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.DataGridView dgvFacturas;
    private System.Windows.Forms.Button btnVerDetalle;
    private System.Windows.Forms.Button btnEnviar;
    private System.Windows.Forms.Label lblRuta;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.GroupBox groupBoxRuta;
    private System.Windows.Forms.GroupBox groupBoxBuscar;
    private System.Windows.Forms.GroupBox groupBoxFacturas;
    private System.Windows.Forms.GroupBox groupBoxAcciones;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.txtRutaDbf = new System.Windows.Forms.TextBox();
        this.btnCargar = new System.Windows.Forms.Button();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.dgvFacturas = new System.Windows.Forms.DataGridView();
        this.btnVerDetalle = new System.Windows.Forms.Button();
        this.btnEnviar = new System.Windows.Forms.Button();
        this.lblRuta = new System.Windows.Forms.Label();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.lblStatus = new System.Windows.Forms.Label();
        this.groupBoxRuta = new System.Windows.Forms.GroupBox();
        this.groupBoxBuscar = new System.Windows.Forms.GroupBox();
        this.groupBoxFacturas = new System.Windows.Forms.GroupBox();
        this.groupBoxAcciones = new System.Windows.Forms.GroupBox();

        ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
        this.groupBoxRuta.SuspendLayout();
        this.groupBoxBuscar.SuspendLayout();
        this.groupBoxFacturas.SuspendLayout();
        this.groupBoxAcciones.SuspendLayout();
        this.SuspendLayout();

        // lblRuta
        this.lblRuta.AutoSize = true;
        this.lblRuta.Location = new System.Drawing.Point(15, 25);
        this.lblRuta.Name = "lblRuta";
        this.lblRuta.Size = new System.Drawing.Size(80, 15);
        this.lblRuta.TabIndex = 0;
        this.lblRuta.Text = "Ruta DBF:";

        // txtRutaDbf
        this.txtRutaDbf.Location = new System.Drawing.Point(15, 45);
        this.txtRutaDbf.Name = "txtRutaDbf";
        this.txtRutaDbf.Size = new System.Drawing.Size(550, 23);
        this.txtRutaDbf.TabIndex = 1;

        // btnCargar
        this.btnCargar.Location = new System.Drawing.Point(570, 43);
        this.btnCargar.Name = "btnCargar";
        this.btnCargar.Size = new System.Drawing.Size(120, 28);
        this.btnCargar.TabIndex = 2;
        this.btnCargar.Text = "Cargar Recientes";
        this.btnCargar.UseVisualStyleBackColor = true;
        this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);

        // groupBoxRuta
        this.groupBoxRuta.Controls.Add(this.lblRuta);
        this.groupBoxRuta.Controls.Add(this.txtRutaDbf);
        this.groupBoxRuta.Controls.Add(this.btnCargar);
        this.groupBoxRuta.Location = new System.Drawing.Point(12, 12);
        this.groupBoxRuta.Name = "groupBoxRuta";
        this.groupBoxRuta.Size = new System.Drawing.Size(700, 80);
        this.groupBoxRuta.TabIndex = 3;
        this.groupBoxRuta.TabStop = false;
        this.groupBoxRuta.Text = "Configuración";

        // lblBuscar
        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(15, 25);
        this.lblBuscar.Name = "lblBuscar";
        this.lblBuscar.Size = new System.Drawing.Size(110, 15);
        this.lblBuscar.TabIndex = 0;
        this.lblBuscar.Text = "Buscar Factura:";

        // txtBuscar
        this.txtBuscar.Location = new System.Drawing.Point(15, 45);
        this.txtBuscar.Name = "txtBuscar";
        this.txtBuscar.Size = new System.Drawing.Size(470, 23);
        this.txtBuscar.TabIndex = 1;

        // btnBuscar
        this.btnBuscar.Location = new System.Drawing.Point(490, 43);
        this.btnBuscar.Name = "btnBuscar";
        this.btnBuscar.Size = new System.Drawing.Size(100, 28);
        this.btnBuscar.TabIndex = 2;
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.UseVisualStyleBackColor = true;
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        // groupBoxBuscar
        this.groupBoxBuscar.Controls.Add(this.lblBuscar);
        this.groupBoxBuscar.Controls.Add(this.txtBuscar);
        this.groupBoxBuscar.Controls.Add(this.btnBuscar);
        this.groupBoxBuscar.Location = new System.Drawing.Point(12, 98);
        this.groupBoxBuscar.Name = "groupBoxBuscar";
        this.groupBoxBuscar.Size = new System.Drawing.Size(700, 80);
        this.groupBoxBuscar.TabIndex = 4;
        this.groupBoxBuscar.TabStop = false;
        this.groupBoxBuscar.Text = "Búsqueda";

        // dgvFacturas
        this.dgvFacturas.AllowUserToAddRows = false;
        this.dgvFacturas.AllowUserToDeleteRows = false;
        this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvFacturas.Location = new System.Drawing.Point(15, 25);
        this.dgvFacturas.MultiSelect = false;
        this.dgvFacturas.Name = "dgvFacturas";
        this.dgvFacturas.ReadOnly = true;
        this.dgvFacturas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvFacturas.Size = new System.Drawing.Size(670, 250);
        this.dgvFacturas.TabIndex = 0;

        // groupBoxFacturas
        this.groupBoxFacturas.Controls.Add(this.dgvFacturas);
        this.groupBoxFacturas.Location = new System.Drawing.Point(12, 184);
        this.groupBoxFacturas.Name = "groupBoxFacturas";
        this.groupBoxFacturas.Size = new System.Drawing.Size(700, 290);
        this.groupBoxFacturas.TabIndex = 5;
        this.groupBoxFacturas.TabStop = false;
        this.groupBoxFacturas.Text = "Facturas";

        // btnVerDetalle
        this.btnVerDetalle.Location = new System.Drawing.Point(15, 30);
        this.btnVerDetalle.Name = "btnVerDetalle";
        this.btnVerDetalle.Size = new System.Drawing.Size(150, 35);
        this.btnVerDetalle.TabIndex = 0;
        this.btnVerDetalle.Text = "Ver Detalle";
        this.btnVerDetalle.UseVisualStyleBackColor = true;
        this.btnVerDetalle.Click += new System.EventHandler(this.btnVerDetalle_Click);

        // btnEnviar
        this.btnEnviar.Location = new System.Drawing.Point(180, 30);
        this.btnEnviar.Name = "btnEnviar";
        this.btnEnviar.Size = new System.Drawing.Size(150, 35);
        this.btnEnviar.TabIndex = 1;
        this.btnEnviar.Text = "Enviar por WhatsApp";
        this.btnEnviar.UseVisualStyleBackColor = true;
        this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

        // groupBoxAcciones
        this.groupBoxAcciones.Controls.Add(this.btnVerDetalle);
        this.groupBoxAcciones.Controls.Add(this.btnEnviar);
        this.groupBoxAcciones.Location = new System.Drawing.Point(12, 480);
        this.groupBoxAcciones.Name = "groupBoxAcciones";
        this.groupBoxAcciones.Size = new System.Drawing.Size(700, 80);
        this.groupBoxAcciones.TabIndex = 6;
        this.groupBoxAcciones.TabStop = false;
        this.groupBoxAcciones.Text = "Acciones";

        // lblStatus
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic);
        this.lblStatus.ForeColor = System.Drawing.Color.Gray;
        this.lblStatus.Location = new System.Drawing.Point(12, 568);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(100, 15);
        this.lblStatus.TabIndex = 7;
        this.lblStatus.Text = "Listo.";

        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(724, 590);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.groupBoxAcciones);
        this.Controls.Add(this.groupBoxFacturas);
        this.Controls.Add(this.groupBoxBuscar);
        this.Controls.Add(this.groupBoxRuta);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "API-FOX - Envío de Facturas por WhatsApp";

        ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
        this.groupBoxRuta.ResumeLayout(false);
        this.groupBoxRuta.PerformLayout();
        this.groupBoxBuscar.ResumeLayout(false);
        this.groupBoxBuscar.PerformLayout();
        this.groupBoxFacturas.ResumeLayout(false);
        this.groupBoxAcciones.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}