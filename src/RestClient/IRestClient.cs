using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Rest.Client.Serializers;

namespace Rest.Client
{
    public interface IRestClient
    {
        HttpClient HttpClient { get; }

        List<IMediaTypeSerializer> MediaTypeSerializers { get; }

        Task DeleteAsync(string requestUri, string accept, IDictionary<string, string> parameters = null);

        Task<T> GetAsync<T>(string requestUri, string accept, IDictionary<string, string> parameters = null);

        Task<T> PatchAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null);

        Task<T> PostAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null);

        Task<T> PutAsync<T>(string requestUri, object body, string contentType, string accept, IDictionary<string, string> parameters = null);
    }
}