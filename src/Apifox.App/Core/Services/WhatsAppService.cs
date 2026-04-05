using System.Net.Http.Json;
using System.Text.Json;

namespace Apifox.App.Services;

public class WhatsAppService
{
    private readonly HttpClient _httpClient;
    private string _baseUrl = "http://localhost:3000";

    public WhatsAppService()
    {
        _httpClient = new HttpClient();
    }

    public void SetBaseUrl(string url)
    {
        _baseUrl = url;
    }

    public async Task<(bool Exito, string Mensaje)> EnviarMensaje(string telefono, string mensaje)
    {
        try
        {
            var payload = new
            {
                phone = telefono,
                message = mensaje
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/send", content);

            if (response.IsSuccessStatusCode)
            {
                await Task.Delay(5000);
                return (true, "Mensaje enviado exitosamente.");
            }

            var error = await response.Content.ReadAsStringAsync();
            return (false, $"Error al enviar: {response.StatusCode}");
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

    public async Task<bool> CheckConexion()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/status");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
