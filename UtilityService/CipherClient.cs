using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZ_WebApp1.UtilityService
{
    public class CipherClient : IDisposable, ICipherClient
    {
        private readonly string _functionUri = "https://simplecipherfunction.azurewebsites.net/api/Function1?code=lrhqI-OEaPlO_IBHcxuCF5TZB-mQ5wZek98N9ixrg5ByAzFumRpSXA==";
        // private readonly string _functionUri = "http://localhost:7253/api/Function1";
        private HttpClient _functionClient;
        private bool disposedValue;

        public CipherClient()
        {
            _functionClient = new HttpClient();
            _functionClient.BaseAddress = new Uri(_functionUri);
        }

        public async Task<string> ProcessTextAsync(DataPackage textData)
        {
            var response = await _functionClient.PostAsJsonAsync<DataPackage>(_functionUri, textData);

            if (response != null && response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    if (_functionClient != null)
                    {
                        _functionClient.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CipherClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class DataPackage
    {
        public string Text { get; set; }
        public string KeyValue { get; set; }
        public string ActionType { get; set; }
    }
}
