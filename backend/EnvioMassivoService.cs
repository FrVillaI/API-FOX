using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EnvioBackend.Services;

// Phase 4: Move the delay logic to backend and support CancellationToken
// This service simulates a mass-send workflow. The real sending should trigger
// external systems (ERP/ERP-like) and can be canceled via the provided token.
public class EnvioMassivoService
{
    private readonly int _defaultDelayMs;
    private readonly ILogger<EnvioMassivoService>? _logger;

    public EnvioMassivoService(IConfiguration configuration, ILogger<EnvioMassivoService>? logger = null)
    {
        // Read default delay from configuration, fallback to 5000ms
        _defaultDelayMs = configuration.GetValue<int?>("Envio:DelayMs") ?? 5000;
        _logger = logger;
    }

    // Starts a mass send operation for a collection of enbd (facturas)
    // The cancellationToken can be used to cancel the operation from the UI (e.g., user closes form)
    public async Task EnviarMasivoAsync(IEnumerable<string> facturas, CancellationToken cancellationToken)
    {
        if (facturas == null) throw new ArgumentNullException(nameof(facturas));

        int index = 0;
        foreach (var facturaId in facturas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            index++;
            _logger?.LogInformation($"[EnvioMasivo] Inicio factura {facturaId} (#{index})");

            // Simula procesamiento y envío, respetando cancellation
            try
            {
                await Task.Delay(_defaultDelayMs, cancellationToken);
                // Aquí iría la lógica real de envío, por ejemplo llamar a WAPI, ERP o servicio externo
                _logger?.LogInformation($"[EnvioMasivo] Factura {facturaId} enviada (simulado)");
            }
            catch (OperationCanceledException)
            {
                _logger?.LogWarning($"[EnvioMasivo] Cancelado durante factura {facturaId}");
                throw;
            }
        }
    }
}
