using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Apifox.App.Services;

// DTOs de respuesta
public record WhatsAppStatusResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("uptime")] string? Uptime
);

public record WhatsAppHealthResponse(
    [property: JsonPropertyName("service")] ServiceInfo? Service,
    [property: JsonPropertyName("lastError")] string? LastError
);

public record ServiceInfo(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("reconnectAttempts")] int ReconnectAttempts
);

public class WhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppService>? _logger;
    private string _baseUrl = "http://localhost:3000";
    private string _apiKey = string.Empty;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Constructor original — sigue funcionando igual
    public WhatsAppService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    // Constructor con DI (opcional, para cuando migres a inyección)
    public WhatsAppService(HttpClient httpClient, ILogger<WhatsAppService>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public void SetBaseUrl(string url) => _baseUrl = url;

    // Nuevo: configurar la API Key del servicio Node.js
    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Remove("x-api-key");
        _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }

    // Tu método original — misma firma, mejorado internamente
    public async Task<(bool Exito, string Mensaje)> EnviarMensaje(string telefono, string mensaje)
    {
        try
        {
            var payload = new { phone = telefono, message = mensaje };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/send", content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Quitado el Task.Delay(5000) — no tenía propósito real
                _logger?.LogInformation("Mensaje WhatsApp enviado a {Telefono}", telefono);
                return (true, "Mensaje enviado exitosamente.");
            }

            _logger?.LogWarning("Fallo al enviar a {Telefono}. HTTP {Status}: {Body}",
                telefono, (int)response.StatusCode, body);

            // Si es 503, damos un mensaje más útil al usuario
            return response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                ? (false, "WhatsApp no está listo. Verifica la conexión del servicio.")
                : (false, $"Error al enviar: {(int)response.StatusCode}");
        }
        catch (TaskCanceledException)
        {
            _logger?.LogError("Timeout enviando mensaje WhatsApp a {Telefono}", telefono);
            return (false, "Timeout: el servicio WhatsApp no respondió a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Error de red enviando a {Telefono}", telefono);
            return (false, $"Error de conexión: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error inesperado enviando a {Telefono}", telefono);
            return (false, $"Error inesperado: {ex.Message}");
        }
    }

    // Tu método original — mejorado para revisar el status real
    public async Task<bool> CheckConexion()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/status");
            if (!response.IsSuccessStatusCode) return false;

            var body = await response.Content.ReadAsStringAsync();
            var status = JsonSerializer.Deserialize<WhatsAppStatusResponse>(body, JsonOpts);
            return status?.Status == "ready";
        }
        catch
        {
            return false;
        }
    }

    // Nuevo: estado detallado para mostrar en un panel o para logging
    public async Task<WhatsAppHealthResponse?> GetHealth()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/health");
            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WhatsAppHealthResponse>(body, JsonOpts);
        }
        catch
        {
            return null;
        }
    }

    // Nuevo: útil para mostrar el QR en un panel de administración
    public async Task<string?> GetQr()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/qr");
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("qr").GetString();
        }
        catch
        {
            return null;
        }
    }
}