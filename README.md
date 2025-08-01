# EasyHttpClient

Uma biblioteca .NET simples e genérica para fazer requisições HTTP com suporte a deserialização automática para objetos tipados.

## Características

- Cliente HTTP genérico com suporte a tipos fortemente tipados
- Métodos para operações HTTP comuns (GET, POST, PUT, DELETE)
- Deserialização automática de JSON para objetos .NET
- Suporte a cancelamento de operações
- Integração com o sistema de injeção de dependências do .NET

## Instalação

```bash
dotnet add package EasyHttpClient
```

## Configuração

Adicione o serviço no seu `Program.cs` ou `Startup.cs`:

```csharp
builder.Services.AddHttpClient();
builder.Services.AddScoped(typeof(IApiClientService<>), typeof(ApiClientService<>));
```

## Uso

### Definindo seu modelo

```csharp
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
}
```

### Injetando e utilizando o serviço

```csharp
public class ProdutoService
{
    private readonly IApiClientService<Produto> _apiClient;
    private readonly string _baseUrl = "https://api.exemplo.com/produtos";

    public ProdutoService(IApiClientService<Produto> apiClient)
    {
        _apiClient = apiClient;
    }

    // Obter todos os produtos
    public async Task<List<Produto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync(_baseUrl, cancellationToken);
    }

    // Obter produto por ID
    public async Task<Produto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetByIdAsync($"{_baseUrl}/{id}", cancellationToken);
    }

    // Criar novo produto
    public async Task<Produto> CriarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync(_baseUrl, produto, cancellationToken);
    }

    // Atualizar produto existente
    public async Task<Produto> AtualizarAsync(int id, Produto produto, CancellationToken cancellationToken = default)
    {
        return await _apiClient.PutAsync($"{_baseUrl}/{id}", produto, cancellationToken);
    }

    // Excluir produto
    public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _apiClient.DeleteAsync($"{_baseUrl}/{id}", cancellationToken);
    }
}
```

## Tratamento de Erros

Os métodos `GetByIdAsync`, `PostAsync` e `PutAsync` lançam exceções quando a requisição não é bem-sucedida. Você pode capturar essas exceções usando um bloco try-catch:

```csharp
try
{
    var produto = await _apiClient.GetByIdAsync($"{_baseUrl}/999");
    // Processar o produto
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    // Tratar erro 404 - Não encontrado
}
catch (HttpRequestException ex)
{
    // Tratar outros erros HTTP
}
```

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para detalhes.