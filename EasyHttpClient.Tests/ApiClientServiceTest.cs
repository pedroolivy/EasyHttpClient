using Moq;
using System.Net;
using System.Text;
using EasyHttpClient;

public class ApiClientServiceTest
{
    [Fact]
    public async Task GetAsync_DeveRetornar_Objeto()
    {
        // Arrange
        var jsonValido = """
            [
                { "id": "1" },
                { "id": "2" }
            ]
            """;
        var handler = new FakeHttpMessageHandler(jsonValido);
        var httpClient = new HttpClient(handler);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<teste>(httpClientFactory.Object);

        // Act
        var resultado = await service.GetAsync("http://qualquerurl.com");

        // Assert
        Assert.NotNull(resultado);
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public FakeHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            });
        }
    }

    public class teste
    {
        public string Id { get; set; }
    }
}
