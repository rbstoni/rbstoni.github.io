namespace Rtrw.Client.Wasm.Models
{
    public class Geocoder
    {
        public string Id { get; set; } = $"geocoder-{Guid.NewGuid().ToString("N")}";
        public string Provinsi { get; set; }
        public string KabupatenKota { get; set; }
        public string Kecamatan { get; set; }
        public string Kelurahan { get; set; }
        public string KodePos { get; set; }
        public string Alamat { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
