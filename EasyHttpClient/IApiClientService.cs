namespace EasyHttpClient
{
    public interface IApiClientService<T>
    {
        Task<List<T>> GetAsync(string url, CancellationToken token = default);
        Task<T> GetByIdAsync(string url, CancellationToken token = default);
        Task<T> PostAsync(string url, T data, CancellationToken token = default);
        Task<T> PutAsync(string url, T data, CancellationToken token = default);
        Task<bool> DeleteAsync(string url, CancellationToken token = default);
    }
}