namespace GroceryShopping.Infrastructure.Network;

public interface IHttpNamedClient
{
    Task<T?> GetAsync<T>(HttpClientName clientName, string requestUri);

    Task PostAsync(HttpClientName clientName, string requestUri);

    Task PostAsync(HttpClientName clientName, string requestUri, object jsonContent);

    Task<T?> PostAsync<T>(HttpClientName clientName, string requestUri, object jsonContent);

    Task PostAsync(HttpClientName clientName, string requestUri, string textContent);

    Task<T?> PostAsync<T>(HttpClientName clientName, string requestUri, Dictionary<string, string> formData);

    Task PutAsync(HttpClientName clientName, string requestUri, object jsonContent);

    Task DeleteAsync(HttpClientName clientName, string requestUri);
}