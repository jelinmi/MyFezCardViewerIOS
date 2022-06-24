using System.Threading.Tasks;

namespace CardFez
{
    public interface IRestClient
    {
        Task<T> GetAsync<T>(string url, bool UseAuthToken = true);
        Task<T> PostAsync<T>(string url, bool UseAuthToken = true);
    }
}