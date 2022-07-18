using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.FakeData;
using Rtrw.Client.Wasm.FakeData.Bogus;
using Rtrw.Client.Wasm.Enums;

namespace Rtrw.Client.Wasm.Services
{
    public interface IInitializerService
    {
        Task InitializeFakeRandomWargaAsync();

    }

    public class InitializerService : IInitializerService
    {
        private readonly ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory;

        public InitializerService(ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory)
        { this.dbContextFactory = dbContextFactory; }

        public async Task InitializeFakeRandomWargaAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var warga = dbContext.Warga;
            if (warga == null || !warga.Any())
            {
                var randomWarga = new Warga()
                {
                    FirstName = "Toni",
                    LastName = "Ribas",
                    FullName = "Toni Ribas",
                    Phone = "+6281904084351",
                    Email = "toni@rtrw.app",
                    Gender = Gender.LakiLaki,
                    DateOfBirth = new DateTime(1985, 5, 28),
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
                await warga!.AddAsync(randomWarga);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}