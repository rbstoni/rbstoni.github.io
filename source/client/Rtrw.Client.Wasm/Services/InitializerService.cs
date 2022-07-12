using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.FakeData;

namespace Rtrw.Client.Wasm.Services
{
    public interface IInitializerService
    {
        Task InitializeFakeRandomPostAsync();

    }

    public class InitializerService : IInitializerService
    {
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;
        private Dummy Dummy { get; } = new Dummy();

        public InitializerService(IApplicationDbContextFactory<SqliteDbContext> dbContextFactory)
        { this.dbContextFactory = dbContextFactory; }

        public async Task InitializeFakeRandomPostAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            if (!dbContext.Posts.Any())
            {
                var randomPost = Dummy.GenerateFakePost().Generate(10);
                await dbContext.Posts.AddRangeAsync(randomPost);
                await dbContext.SaveChangesAsync();
            }
        }

        async Task InitializeFakeRandomWargaAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.Warga.Count() == 0)
            {
                Warga newWarga = new()
                {
                    DateOfBirth = new DateTime(1985, 5, 28),
                    FirstName = "Toni",
                    LastName= "Ribas",
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
                        Longitude = "106.85654880943139",
                        Latitude = "-6.144607199436237",
                    },
                };
                await dbContext.Warga.AddAsync(newWarga);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
