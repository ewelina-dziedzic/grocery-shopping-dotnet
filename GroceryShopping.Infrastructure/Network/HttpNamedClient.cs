using System.Text;
using System.Text.Json;

namespace GroceryShopping.Infrastructure.Network;

public class HttpNamedClient(IHttpClientFactory httpClientFactory) : IHttpNamedClient
{
    public async Task<T?> GetAsync<T>(HttpClientName clientName, string requestUri)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(responseStream);
    }

    public async Task PostAsync(HttpClientName clientName, string requestUri)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var response = await client.PostAsync(requestUri, new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
    }

    public async Task PostAsync(HttpClientName clientName, string requestUri, object jsonContent)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var stringContent = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUri, stringContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T?> PostAsync<T>(HttpClientName clientName, string requestUri, object jsonContent)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var stringContent = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUri, stringContent);
        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(responseStream);
    }

    public async Task PostAsync(HttpClientName clientName, string requestUri, string textContent)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var stringContent = new StringContent(textContent, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync(requestUri, stringContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T?> PostAsync<T>(
        HttpClientName clientName,
        string requestUri,
        Dictionary<string, string> formData)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var formContent = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync(requestUri, formContent);
        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(responseStream);
    }

    public async Task PutAsync(HttpClientName clientName, string requestUri, object jsonContent)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var stringContent = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(requestUri, stringContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(HttpClientName clientName, string requestUri)
    {
        var client = httpClientFactory.CreateClient(clientName.ToString());
        var response = await client.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
    }
}