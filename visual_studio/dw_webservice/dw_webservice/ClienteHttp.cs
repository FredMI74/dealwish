using System.Net.Http;

namespace dw_webservice
{
    public class ClienteHttp
    {
        private static readonly HttpClient _client = new HttpClient();
        public HttpClient client;

        public ClienteHttp()
        {
            client = _client;
        }
        
        public static ClienteHttp Instance { get; } = new ClienteHttp();
    }
}
