using Rtrw.Client.Wasm.Components.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Services.Geocoding
{
    public class GeocodingParameters
    {
        public List<string>? Country { get; set; } = new() { "id" };
        public string AccessToken { get; set; } = "pk.eyJ1IjoiZGhhcmlvc3V0ZWpvIiwiYSI6ImNrenp0anQ3MzBkbmszZHFjMXVwMWZ5Z2wifQ.QS_6oRMj0Yriiy9oX5GYaQ";
        public Proximity Proximity { get; set; } = Proximity.None;
        public List<string>? Language { get; set; } = new() { "id" };
        public List<string>? Types { get; set; }
        public int? Limit { get; set; }
        public int? MinLongitude { get; set; } = -180;
        public int? MaxLongitude { get; set; } = -180;
        public int? MinLatitude { get; set; } = -90;
        public int? MaxLatitude { get; set; } = 90;
        public WorldView WorldView { get; set; } = WorldView.Default;
        public bool EnableBoundingBox { get; set; } = false;
        public bool? AutoComplete { get; set; }
        public bool? Routing { get; set; }
        public bool? FuzzyMatch { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public Dictionary<string, string> Queries => QueryParameters;
        protected Dictionary<string, string> QueryParameters => new()
        {
            ["country"] = Country != null ? GeocodingHelpers.ConvertListOfStringToString(Country) : string.Empty,
            ["bbox"] = GetBoundingBoxBoundary(EnableBoundingBox),
            ["limit"] = Limit.HasValue ? Limit.Value.ToString() : string.Empty,
            ["proximity"] = ProximityToString(Proximity),
            ["types"] = Types != null ? GeocodingHelpers.ConvertListOfStringToString(Types) : string.Empty,
            ["language"] = Language != null ? GeocodingHelpers.ConvertListOfStringToString(Language) : string.Empty,
            ["autocomplete"] = AutoComplete.HasValue ? AutoComplete.Value.ToString() : string.Empty,
            ["fuzzyMatch"] = FuzzyMatch.HasValue ? FuzzyMatch.Value.ToString() : string.Empty,
            ["routing"] = Routing.HasValue ? Routing.Value.ToString() : string.Empty,
            ["worldview"] = WorldView != WorldView.Default ? WorldView.EnumToDescriptionString() : string.Empty,
            ["access_token"] = AccessToken
        };

        private string GetBoundingBoxBoundary(bool isBboxEnable)
        {
            if (isBboxEnable)
            {
                StringBuilder builder = new();
                return builder.AppendJoin("%2C", MinLongitude, MinLatitude, MaxLongitude, MaxLatitude).ToString();
            }
            return string.Empty;
        }

        private string ProximityToString(Proximity proximity)
        {
            if (proximity == Proximity.Ip)
            {
                return "ip";
            }
            else if (proximity == Proximity.None)
            {
                return string.Empty;
            }

            else
            {
                return new StringBuilder().AppendJoin("%2C", Longitude, Latitude).ToString();
            }
        }
    }
    public enum Proximity
    {
        [Description("ip")]
        Ip,
        [Description("coordinate")]
        Coordinate,
        [Description("none")]
        None
    }
    public enum WorldView
    {
        [Description("default")]
        Default,
        [Description("cn")]
        Chinese,
        [Description("id")]
        Indian,
        [Description("jp")]
        Japanese,
        [Description("us")]
        American
    }
}
