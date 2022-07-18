using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Features.Mapbox
{
    public class MapboxRenderedFeatured
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("properties")]
        public MapboxFeatureProperties? Properties { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("layer")]
        public MapboxFeatureLayer? Layer { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }
    }

    public class MapboxFeatureProperties
    {
        [JsonPropertyName("ADM0_EN")]
        public string? ADM0_EN { get; set; }

        [JsonPropertyName("ADM0_PCODE")]
        public string? ADM0_PCODE { get; set; }

        [JsonPropertyName("ADM1_EN")]
        public string? ADM1_EN { get; set; }

        [JsonPropertyName("ADM1_PCODE")]
        public string? ADM1_PCODE { get; set; }

        [JsonPropertyName("ADM2_EN")]
        public string? ADM2_EN { get; set; }

        [JsonPropertyName("ADM2_PCODE")]
        public string? ADM2_PCODE { get; set; }

        [JsonPropertyName("ADM3_EN")]
        public string? ADM3_EN { get; set; }

        [JsonPropertyName("ADM3_PCODE")]
        public string? ADM3_PCODE { get; set; }

        [JsonPropertyName("ADM4ALT1EN")]
        public string? ADM4ALT1EN { get; set; }

        [JsonPropertyName("ADM4ALT2EN")]
        public string? ADM4ALT2EN { get; set; }

        [JsonPropertyName("ADM4_EN")]
        public string? ADM4_EN { get; set; }

        [JsonPropertyName("ADM4_PCODE")]
        public string? ADM4_PCODE { get; set; }

        [JsonPropertyName("ADM4_REF")]
        public string? ADM4_REF { get; set; }
        [JsonPropertyName("Shape_Area")]
        public float ShapeArea { get; set; }
        [JsonPropertyName("Shape_Leng")]
        public float ShapeLeng { get; set; }
        [JsonPropertyName("Date")]
        public string? Date { get; set; }
        [JsonPropertyName("validOn")]
        public string? ValidOn { get; set; }
        [JsonPropertyName("validTo")]
        public string? ValidTo { get; set; }
    }

    public class MapboxFeatureLayer
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        [JsonPropertyName("minzoom")]
        public int MinZoom { get; set; }
    }
}
