using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Apifox.App.Services;

// DTOs
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

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WhatsAppService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public void SetBaseUrl(string url)
    {
        _baseUrl = url.TrimEnd('/');
    }

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
                _logger?.LogInformation("WhatsApp enviado a {Telefono}", telefono);
                return (true, "Mensaje enviado correctamente.");
            }

            _logger?.LogWarning("Error envío {Telefono} HTTP {Status} {Body}",
                telefono, (int)response.StatusCode, body);

            return response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                ? (false, "WhatsApp no está listo (QR o desconectado).")
                : (false, $"Error HTTP {(int)response.StatusCode}");
        }
        catch (TaskCanceledException)
        {
            return (false, "Timeout: el servicio no respondió.");
        }
        catch (HttpRequestException ex)
        {
            return (false, $"Error de conexión: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Error inesperado: {ex.Message}");
        }
    }

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
}