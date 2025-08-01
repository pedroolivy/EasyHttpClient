using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using EasyHttpClient;

public class ApiClientServiceTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly TestModel _testModel;
    private readonly string _baseUrl;

    public ApiClientServiceTest()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _testModel = new TestModel { Id = "1", Name = "Test" };
        _baseUrl = "http://api.test.com";
    }

    [Fact]
    public async Task GetAsync_DeveRetornar_ListaDeObjetos()
    {
        // Arrange
        var jsonValido = """
            [
                { "id": "1", "name": "Test 1" },
                { "id": "2", "name": "Test 2" }
            ]
            """;
        var handler = new FakeHttpMessageHandler(jsonValido, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler);

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<TestModel>(_httpClientFactoryMock.Object);

        // Act
        var resultado = await service.GetAsync(_baseUrl);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("1", resultado[0].Id);
        Assert.Equal("Test 1", resultado[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornar_ObjetoUnico()
    {
        // Arrange
        var jsonValido = "{ \"id\": \"1\", \"name\": \"Test 1\" }";
        var handler = new FakeHttpMessageHandler(jsonValido, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler);

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<TestModel>(_httpClientFactoryMock.Object);

        // Act
        var resultado = await service.GetByIdAsync($"{_baseUrl}/1");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("1", resultado.Id);
        Assert.Equal("Test 1", resultado.Name);
    }

    [Fact]
    public async Task PostAsync_DeveRetornar_ObjetoCriado()
    {
        // Arrange
        var jsonValido = "{ \"id\": \"1\", \"name\": \"Test 1\" }";
        var handler = new FakeHttpMessageHandler(jsonValido, HttpStatusCode.Created);
        var httpClient = new HttpClient(handler);

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<TestModel>(_httpClientFactoryMock.Object);

        // Act
        var resultado = await service.PostAsync(_baseUrl, _testModel);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("1", resultado.Id);
    }

    [Fact]
    public async Task PutAsync_DeveRetornar_ObjetoAtualizado()
    {
        // Arrange
        var jsonValido = "{ \"id\": \"1\", \"name\": \"Updated Test\" }";
        var handler = new FakeHttpMessageHandler(jsonValido, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler);

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<TestModel>(_httpClientFactoryMock.Object);

        // Act
        var resultado = await service.PutAsync($"{_baseUrl}/1", _testModel);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Updated Test", resultado.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeveRetornar_True_QuandoSucesso()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler("", HttpStatusCode.NoContent);
        var httpClient = new HttpClient(handler);

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new ApiClientService<TestModel>(_httpClientFactoryMock.Object);

        // Act
        var resultado = await service.DeleteAsync($"{_baseUrl}/1");

        // Assert
        Assert.True(resultado);
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
        {
            _responseContent = responseContent;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            });
        }
    }

    public class TestModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
