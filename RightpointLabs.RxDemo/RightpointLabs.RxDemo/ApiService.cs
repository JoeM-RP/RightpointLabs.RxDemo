using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RightpointLabs.RxDemo.Models;

namespace RightpointLabs.RxDemo
{
    public interface IApiService
    {
        Task<ApiResponse> Refresh(string stationId = null);
    }

    public class ApiService : IApiService
    {
        public async Task<ApiResponse> Refresh(string stationId = null)
        {
            var stId = App.MapId;

            if (stationId != null)
                stId = stationId;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{App.Host}{App.Version}/ttarrivals.aspx?key={App.ApiKey}&mapid={stId}&max={App.Max}")
                };

                try
                {

                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode) throw new HttpRequestException(response.ReasonPhrase);

                    var serializer = new XmlSerializer(typeof(ApiResponse));

                    return (ApiResponse)serializer.Deserialize(response.Content.ReadAsStreamAsync().Result);
                }
                catch
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    throw;
                }
            }

        }
    }
}
