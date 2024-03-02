using System.Net;
using BusinessLogic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Tests;

public class RandomUserServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<ILogger<RandomUserService>> _loggerMock = new();
    private readonly IConfiguration _configuration;
    private readonly Mock<HttpMessageHandler> _handlerMock = new();

    public RandomUserServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"RandomUserApi:BaseUrl", "https://randomuser.me/api/"},
            {"RandomUserApi:DefaultResults", "5"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    private RandomUserService CreateService(HttpResponseMessage responseMessage)
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(_handlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return new RandomUserService(_httpClientFactoryMock.Object, _loggerMock.Object, _configuration);
    }

    [Fact]
    public async Task GetRandomUsersAsync_ShouldReturnUsers_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var responseContent = "{\"results\":[{\"name\":{\"title\":\"Mr\",\"first\":\"John\",\"last\":\"Doe\"},\"dob\":{\"age\":30},\"location\":{\"country\":\"USA\"}}]}";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        var service = CreateService(responseMessage);

        // Act
        var result = await service.GetRandomUsersAsync(1);

        // Assert
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("Mr John Doe");
        result.First().Age.Should().Be(30);
        result.First().Country.Should().Be("USA");

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get
                && req.RequestUri.ToString().Contains("https://randomuser.me/api/?results=1")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetRandomUsersAsync_ShouldLogError_WhenApiResponseIsUnsuccessful()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("")
        };

        var service = CreateService(responseMessage);

        // Act
        var result = await service.GetRandomUsersAsync(1);

        // Assert
        result.Should().BeEmpty();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch users from RandomUser.me API")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().Contains("https://randomuser.me/api/?results=1")),
            ItExpr.IsAny<CancellationToken>());
    }
}
