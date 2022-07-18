using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Services.Geocoding
{
    public class GeocodingResponse
    {
        [JsonPropertyName("features")]
        public List<Feature>? Features { get; set; }
    }

    public class Feature
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("place_type")]
        public List<string>? PlaceType { get; set; }

        [JsonPropertyName("relevance")]
        public float Relevance { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("properties")]
        public Properties? Properties { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("place_name")]
        public string? PlaceName { get; set; }

        [JsonPropertyName("matching_text")]
        public string? MatchingText { get; set; }

        [JsonPropertyName("matching_place_name")]
        public string? MatchingPlaceName { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("bbox")]
        public List<float>? BoundingBox { get; set; }

        [JsonPropertyName("center")]
        public List<float>? Center { get; set; }

        //[JsonPropertyName("geometry")]
        //public Geometry? Geometry { get; set; }

        [JsonPropertyName("text_id")]
        public string? TextId { get; set; }

        [JsonPropertyName("language_id")]
        public string? LanguageId { get; set; }

        [JsonPropertyName("place_name_id")]
        public string? PlaceNameId { get; set; }

        //[JsonPropertyName("context")]
        //public List<Context>? Context { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("accuracy")]
        public string? Accuracy { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("maki")]
        public string? Maki { get; set; }

        [JsonPropertyName("wikidata")]
        public string? Wikidata { get; set; }

        [JsonPropertyName("short_code")]
        public string? ShortCode { get; set; }
    }

    /// <summary>
    /// An object describing the spatial geometry of the returned feature.
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Point", a GeoJSON type from the GeoJSON specification.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// An array in the format [longitude,latitude] at the center of the specified bbox.
        /// </summary>
        [JsonPropertyName("coordinates")]
        public List<float>? Coordinates { get; set; }

        /// <summary>
        /// Optional. If present, indicates that an address is interpolated along a road network. 
        /// The geocoder can usually return exact address points, but if an address is not present the geocoder can use interpolated data as a fallback. 
        /// In edge cases, interpolation may not be possible if surrounding address data is not present, 
        /// in which case the next fallback will be the center point of the street feature itself.
        /// </summary>
        [JsonPropertyName("interpolated")]
        public bool? Interpolated { get; set; }

        /// <summary>
        /// Optional. If present, indicates an out-of-parity match. 
        /// This occurs when an interpolated address is not in the expected range for the indicated side of the street.
        /// </summary>
        [JsonPropertyName("omitted")]
        public bool? Omitted { get; set; }
    }

    /// <summary>
    /// An array representing the hierarchy of encompassing parent features. 
    /// Each parent feature may include any of the above properties.
    /// </summary>
    public class Context
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("wikidata")]
        public string? Wikidata { get; set; }

        [JsonPropertyName("short_code")]
        public string? ShortCode { get; set; }

        [JsonPropertyName("text_id")]
        public string? TextId { get; set; }

        [JsonPropertyName("language_id")]
        public string? LanguageId { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }
    }
}

