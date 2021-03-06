using dw_frontoffice.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace dw_frontoffice
{
    public class DwClienteHttp
    {
        private static readonly HttpClient _client = new HttpClient();
        public HttpClient client;
        public string token_anonimo { get; set; }

        public DwClienteHttp()
        {
            client = _client;
            token_anonimo = "";
        }

        public static DwClienteHttp Instance { get; } = new DwClienteHttp();

    }
}
