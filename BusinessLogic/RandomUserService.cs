using System.Text.Json;
using BusinessLogic.Dto;
using BusinessLogic.Service;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;

namespace BusinessLogic;

/// <summary>
/// Provides services to fetch random user data from the RandomUser.me API.
/// </summary>
public sealed class RandomUserService : IRandomUserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RandomUserService> _logger;
    private readonly string _baseUrl;
    private readonly int _defaultResults;
    private readonly AsyncPolicy<HttpResponseMessage> _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the RandomUserService with specified dependencies.
    /// </summary>
    /// <param name="httpClientFactory">The factory to create HttpClient instances.</param>
    /// <param name="logger">The logger for logging messages.</param>
    /// <param name="configuration">The application configuration settings.</param>
    public RandomUserService(IHttpClientFactory httpClientFactory, ILogger<RandomUserService> logger, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _baseUrl = configuration["RandomUserApi:BaseUrl"] ?? string.Empty;
        _defaultResults = int.Parse(configuration["RandomUserApi:DefaultResults"] ?? "5");
        _retryPolicy = CreateRetryPolicy();
    }

    /// <summary>
    /// Fetches random user data asynchronously from the RandomUser.me API.
    /// </summary>
    /// <param name="numberOfUsers">The number of random users to fetch. Defaults to 5 if not specified.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="UserDto"/>.</returns>
    public async Task<IEnumerable<UserDto>> GetRandomUsersAsync(int numberOfUsers = 5)
    {
        var effectiveNumberOfUsers = numberOfUsers > 0 ? numberOfUsers : _defaultResults;
        var url = $"{_baseUrl}?results={effectiveNumberOfUsers}";

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var randomUserResponse = JsonSerializer.Deserialize<RandomUserResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (randomUserResponse?.Results is null)
            {
                _logger.LogError("Received null or empty results from RandomUser.me API. URL: {Url}", url);
                return Enumerable.Empty<UserDto>();
            }

            return randomUserResponse.Results.Select(user => new UserDto(
                $"{user.Name.Title} {user.Name.First} {user.Name.Last}",
                user.Dob.Age,
                user.Location.Coordinates.Latitude,
                user.Location.Coordinates.Longitude,
                user.Location.Country
            ));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch users from RandomUser.me API. URL: {Url}", url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing the RandomUser.me API response. URL: {Url}", url);
        }
        return Enumerable.Empty<UserDto>();
    }

    /// <summary>
    /// Creates a retry policy for handling transient faults when calling the RandomUser.me API.
    /// </summary>
    /// <returns>The retry policy.</returns>
    private AsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && (int)r.StatusCode >= 500 || (int)r.StatusCode == 408)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                });
    }
}
