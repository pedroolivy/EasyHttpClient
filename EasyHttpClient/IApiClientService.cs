namespace EasyHttpClient
{
    public interface IApiClientService<T>
    {
        Task<List<T>> GetAsync(string url, CancellationToken token);
    }
}