using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.Services
{
    public interface ICurrentUser
    {
        Warga Warga { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        readonly Warga warga = new()
        {
            DateOfBirth = new DateTime(1985, 5, 28),
            FirstName = "Toni",
            LastName = "Ribas",
            Email = "toni@rtrw.app",
            Gender = Enums.Gender.Lakilaki,
            Geocoder = new Geocoder()
            {
                Provinsi = "DKI Jakarta",
                KabupatenKota = "Kota Jakarta Utara",
                Kecamatan = "Tanjung Priok",
                Kelurahan = "Sunter Agung",
                KodePos = "14350",
                Alamat = "Sunter Muara 1B",
                Longitude = "106.8563909153426",
                Latitude = "-6.144614964014737",               
            },
        };
        public Warga Warga => warga;
    }
}
