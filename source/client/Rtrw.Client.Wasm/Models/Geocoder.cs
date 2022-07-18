using System.ComponentModel.DataAnnotations.Schema;

namespace Rtrw.Client.Wasm.Models
{
    public class Geocoder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string? Provinsi { get; set; }
        public string? KabupatenKota { get; set; }
        public string? Kecamatan { get; set; }
        public string? Kelurahan { get; set; }
        public string? KodePos { get; set; }
        public string? Alamat { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
