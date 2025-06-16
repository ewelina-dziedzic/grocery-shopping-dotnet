using System.Net.Http.Headers;

using GroceryShopping.Infrastructure.Network;

using Microsoft.Extensions.Options;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoAccessTokenHandler(IHttpNamedClient httpClient, IOptions<FriscoOptions> friscoOptions)
    : DelegatingHandler
{
    public const string UserIdPlaceholder = "###UserId###";

    private static readonly SemaphoreSlim TokenLock = new(1, 1);
    private static FriscoAccessToken? _accessToken;
    private static DateTime _accessTokenExpiry;

    private readonly FriscoOptions _friscoOptions = friscoOptions.Value;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        request.RequestUri = new Uri(request.RequestUri!.ToString().Replace(UserIdPlaceholder, token.UserId));
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<FriscoAccessToken> GetAccessTokenAsync()
    {
        if (_accessToken != null && DateTime.Now < _accessTokenExpiry)
        {
            return _accessToken;
        }

        await TokenLock.WaitAsync();
        try
        {
            if (_accessToken != null)
            {
                _accessToken = await RefreshAccessToken(_accessToken.RefreshToken);
            }
            else
            {
                _accessToken = await GetAccessToken();
            }

            if (_accessToken == null)
            {
                throw new InvalidOperationException("Access token must not be null.");
            }

            _accessTokenExpiry = DateTime.Now.AddSeconds(_accessToken.ExpiresIn);
            return _accessToken;
        }
        finally
        {
            TokenLock.Release();
        }
    }

    private async Task<FriscoAccessToken?> RefreshAccessToken(string refreshToken)
    {
        var refreshTokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
        };
        return await httpClient.PostAsync<FriscoAccessToken>(
            HttpClientName.FriscoAuthentication,
            "/app/commerce/connect/token",
            refreshTokenRequest);
    }

    private async Task<FriscoAccessToken?> GetAccessToken()
    {
        var logInRequest = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", _friscoOptions.Username },
            { "password", _friscoOptions.Password },
        };
        return await httpClient.PostAsync<FriscoAccessToken>(
            HttpClientName.FriscoAuthentication,
            "/app/commerce/connect/token",
            logInRequest);
    }
}