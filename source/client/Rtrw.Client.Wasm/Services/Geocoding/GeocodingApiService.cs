using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Services.Geocoding
{
    public interface IGeocodingApiService
    {
        Task<GeocodingResponse> ForwardGeocoding(string searchText);
        Task<GeocodingResponse> ReverseGeocoding(double longitude, double latitude);
    }

    public class GeocodingApiService : IGeocodingApiService
    {
        private readonly Dictionary<string, string> parameters;
        private readonly GeocodingParameters geocodingParameters = new();
        private readonly HttpClient httpClient;

        public GeocodingApiService(HttpClient httpClient)
        {
            parameters = geocodingParameters.Queries;
            this.httpClient = httpClient;
        }
        public async Task<GeocodingResponse?> ForwardGeocoding(string searchText)
        {
            bool startingQuestionMarkAdded = false;
            StringBuilder builder = new();
            builder.Append("https://api.mapbox.com/geocoding/v5/mapbox.places/");
            builder.AppendJoin("", UrlEncoder.Default.Encode(searchText), ".json");
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Value))
                {
                    builder.Append(startingQuestionMarkAdded ? '&' : '?');
                    builder.Append(parameter.Key);
                    builder.Append('=');
                    builder.Append(parameter.Value);
                }
                startingQuestionMarkAdded = true;
            }
            var stream = await httpClient.GetStreamAsync(builder.ToString());
            var result = await JsonSerializer.DeserializeAsync<GeocodingResponse>(stream);

            return result;
        }

        public async Task<GeocodingResponse?> ReverseGeocoding(double longitude, double latitude)
        {
            bool startingQuestionMarkAdded = false;
            StringBuilder builder = new();
            builder.Append("https://api.mapbox.com/geocoding/v5/mapbox.places/");
            builder.AppendJoin("", UrlEncoder.Default.Encode(new StringBuilder().AppendJoin(",", longitude, latitude).ToString()), ".json");
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Value))
                {
                    builder.Append(startingQuestionMarkAdded ? '&' : '?');
                    builder.Append(parameter.Key);
                    builder.Append('=');
                    builder.Append(parameter.Value);
                }
                startingQuestionMarkAdded = true;
            }
            var streamTask = httpClient.GetStreamAsync(builder.ToString());
            return await JsonSerializer.DeserializeAsync<GeocodingResponse>(await streamTask) ?? null;
        }
    }
}
