using System.Net.Http.Headers;
using System.Net.Http.Json;
using CurrencyConverter.Api.Contracts.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CurrencyConverter.IntegrationTests;

public sealed class RatesApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RatesApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task Convert_ShouldReturnBadRequest_WhenExcludedCurrencyUsed()
    {
        var client = _factory.CreateClient();
        var token = await GetTokenAsync(client, new[] { "Converter", "RatesReader" });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/rates/convert?amount=100&fromCurrency=TRY&toCurrency=USD");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Latest_ShouldReturnUnauthorized_WithoutToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/rates/latest?baseCurrency=EUR");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Latest_ShouldReturnSuccess_WithValidToken()
    {
        var client = _factory.CreateClient();
        var token = await GetTokenAsync(client, new[] { "RatesReader" });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/rates/latest?baseCurrency=EUR");

        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> GetTokenAsync(HttpClient client, string[] roles)
    {
        var tokenResponse = await client.PostAsJsonAsync("/api/v1/auth/token", new TokenRequest("integration-tests", roles));
        tokenResponse.EnsureSuccessStatusCode();

        var payload = await tokenResponse.Content.ReadFromJsonAsync<TokenPayload>();
        return payload!.accessToken;
    }

    private sealed record TokenPayload(string accessToken);
}
