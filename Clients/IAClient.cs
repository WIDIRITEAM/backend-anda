using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public class ServicioIAClient
{
    private readonly HttpClient _http;

    public ServicioIAClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("http://localhost:5001");
    }

    public async Task<JsonDocument?> EnviarCotizacionAsync(string prompt, List<object> servicios)
    {
        var payload = new
        {
            prompt,
            servicios
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/cotizar", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Error al llamar a la IA: {response.StatusCode}");
            return null;
        }

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}