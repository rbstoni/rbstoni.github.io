using Bogus;
using Bogus.DataSets;
using Rtrw.Client.Wasm.Enums;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.FakeData.Bogus
{
    public class Dummy
    {

        private List<string> KecamatanKotaJakartaUtara = new() { "Cilincing", "Kelapa Gading", "Koja", "Pademangan", "Penjaringan", "Tanjung Priok" };
        private List<string> KelurahanCilincing = new() { "Cilincing", "Kalibaru", "Marunda", "Rorotan", "Semper Barat", "Semper Timur", "Sukapura" };
        private List<string> KelurahanKelapaGading = new() { "Kelapa Gading Barat", "Kelapa Gading Timur", "Pegangsaan Dua" };
        private List<string> KelurahanKoja = new() { "Koja", "Lagoa", "Rawa Badak Selatan", "Rawa Badak Utara", "Tugu Selatan", "Tugu Utara" };
        private List<string> KelurahanPademangan = new() { "Ancol", "Pademangan Barat", "Pademangan Timur" };
        private List<string> KelurahanPenjaringan = new() { "Kamal Muara", "Kapuk Muara", "Pejagalan", "Penjaringan", "Pluit" };
        private List<string> KelurahanTanjungPriok = new() { "Kebon Bawang", "Papanggo", "Sungai Bambu", "Sunter Agung", "Sunter Jaya", "Tanjung Priok", "Warakas" };

        string DummyText => Lorem.Paragraph(50);
        Lorem Lorem => new(locale: "id_ID");

        public Faker<Geocoder> GenerateFakeAddress()
            => new Faker<Geocoder>("id_ID")
                .RuleFor(a => a.Provinsi, "DKI Jakarta")
                .RuleFor(a => a.KabupatenKota, "Kota Jakarta Utara")
                .RuleFor(a => a.Kecamatan, f => f.PickRandom(KecamatanKotaJakartaUtara))
                .RuleFor(a => a.Kelurahan, (f, x) => PickRandomKelurahan(x.Kecamatan.ToString()))
                .RuleFor(a => a.KodePos, (f, x) => SetKodeposKelurahan(x.Kelurahan.ToString()))
                .RuleFor(a => a.Alamat, f => f.Address.StreetAddress())
                .RuleFor(a => a.Longitude, GetRandomNumber(-180, 180).ToString())
                .RuleFor(a => a.Latitude, GetRandomNumber(-90, 90).ToString());
        public Faker<Geocoder> GenerateFakeAddress(string kelurahan)
            => new Faker<Geocoder>("id_ID")
                .RuleFor(a => a.Provinsi, "DKI Jakarta")
                .RuleFor(a => a.KabupatenKota, "Kota Jakarta Utara")
                .RuleFor(a => a.Kecamatan, f => f.PickRandom(KecamatanKotaJakartaUtara))
                .RuleFor(a => a.Kelurahan, kelurahan)
                .RuleFor(a => a.KodePos, (f, x) => SetKodeposKelurahan(x.Kelurahan.ToString()))
                .RuleFor(a => a.Alamat, f => f.Address.StreetAddress())
                .RuleFor(a => a.Longitude, GetRandomNumber(-180, 180).ToString())
                .RuleFor(a => a.Latitude, GetRandomNumber(-90, 90).ToString());
        public Faker<Warga> GenerateFakeWarga()
            => new Faker<Warga>("id_ID")
                .RuleFor(x => x.FullName, (f, x) => f.Person.FullName)
                .RuleFor(x => x.Email, (f, x) => x.FullName.ToLower() + "@rtrw.app")
                .RuleFor(x => x.Phone, f => f.Person.Phone)
                .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth)
                .RuleFor(x => x.AvatarUrl, f => f.Person.Avatar)
                .RuleFor(x => x.Gender, f => f.PickRandom<Gender>())
                .RuleFor(x => x.ProfileUrl, (f, x) => $"profile/{x.Id}")
                .RuleFor(x => x.Geocoder, GenerateFakeAddress().Generate());

        public Faker<Post> GenerateFakePost()
            => new Faker<Post>("id_ID")
                .RuleFor(x => x.CreatedAt, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
                .RuleFor(x => x.Author, f => GenerateFakeWarga().Generate())
                .RuleFor(x => x.Scope, f => f.PickRandom<Scope>())
                .RuleFor(x => x.PostGeocoder, GenerateFakeAddress().Generate())
                .RuleFor(x => x.Text, Lorem.Paragraph(10))
                .RuleFor(x => x.Media, f => GenerateFakeImage().GenerateBetween(0, 5))
                .RuleFor(x => x.Comments, f => GenerateFakeComment().GenerateBetween(0, 10))
                .RuleFor(x => x.Reactions, f => GenerateFakeReaction().GenerateBetween(0, 10));
        public Faker<Post> GenerateFakePost(string kelurahan)
            => new Faker<Post>("id_ID")
                .RuleFor(x => x.CreatedAt, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
                .RuleFor(x => x.Author, f => GenerateFakeWarga().Generate())
                .RuleFor(x => x.Scope, f => f.PickRandom<Scope>())
                .RuleFor(x => x.PostGeocoder, GenerateFakeAddress(kelurahan).Generate())
                .RuleFor(x => x.Text, Lorem.Paragraph(10))
                .RuleFor(x => x.Media, f => GenerateFakeImage().GenerateBetween(0, 5))
                .RuleFor(x => x.Comments, f => GenerateFakeComment().GenerateBetween(0, 10))
                .RuleFor(x => x.Reactions, f => GenerateFakeReaction().GenerateBetween(0, 10));
        public Faker<Comment> GenerateFakeComment()
            => new Faker<Comment>("id_ID")
                .RuleFor(x => x.CreatedAt, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
                .RuleFor(x => x.Commenter, f => GenerateFakeWarga().Generate())
                .RuleFor(x => x.Media, f => GenerateFakeImage().GenerateBetween(0, 5))
                .RuleFor(x => x.Text, Lorem.Paragraph(10))
                .RuleFor(x => x.Reactions, GenerateFakeReaction().GenerateBetween(0, 10))
                .RuleFor(x => x.Replies, GenerateFakeReply().GenerateBetween(0, 10));
        public Faker<Comment> GenerateFakeReply()
            => new Faker<Comment>("id_ID")
                .RuleFor(x => x.CreatedAt, f => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
                .RuleFor(x => x.Commenter, f => GenerateFakeWarga().Generate())
                .RuleFor(x => x.Text, Lorem.Paragraph(25))
                .RuleFor(x => x.Reactions, GenerateFakeReaction().GenerateBetween(1, 10));
        public Faker<Reaction> GenerateFakeReaction()
            => new Faker<Reaction>("id_ID")
                .RuleFor(x => x.CreatedAt, DateTime.Now)
                .RuleFor(x => x.Reactor, f => GenerateFakeWarga().Generate())
                .RuleFor(x => x.Emoji, f => f.PickRandom<Emoji>());

        public static Faker<Medium> GenerateFakeImage()
            => new Faker<Medium>("id_ID")
                .RuleFor(x => x.Url, f => f.Image.PicsumUrl());

        static double GetRandomNumber(int minimum, int maximum)
        {
            var random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        string PickRandomKelurahan(string kecamatan)
        {
            var faker = new Faker();
            if (kecamatan == "Cilincing")
                return faker.PickRandom(KelurahanCilincing);
            if (kecamatan == "Kelapa Gading")
                return faker.PickRandom(KelurahanKelapaGading);
            if (kecamatan == "Koja")
                return faker.PickRandom(KelurahanKoja);
            if (kecamatan == "Pademangan")
                return faker.PickRandom(KelurahanPademangan);
            if (kecamatan == "Penjaringan")
                return faker.PickRandom(KelurahanPenjaringan);
            if (kecamatan == "Tanjung Priok")
                return faker.PickRandom(KelurahanTanjungPriok);
            else
                return string.Empty;
        }

        static string SetKodeposKelurahan(string kelurahan)
        {
            if (kelurahan == "Koja Utara")
                return "14210";
            if (kelurahan == "Koja Selatan")
                return "14220";
            if (kelurahan == "Rawa Badak Utara")
                return "14230";
            if (kelurahan == "Rawa Badak Selatan")
                return "14230";
            if (kelurahan == "Tugu Utara")
                return "14260";
            if (kelurahan == "Tugu Selatan")
                return "14260";
            if (kelurahan == "Lagoa")
                return "14270";
            if (kelurahan == "Kelapa Gading Barat")
                return "14240";
            if (kelurahan == "Kelapa Gading Timur")
                return "14240";
            if (kelurahan == "Pegangsaan Dua")
                return "14250";
            if (kelurahan == "Tanjung Priok")
                return "14310";
            if (kelurahan == "Kebon Bawang")
                return "14320";
            if (kelurahan == "Sungai Bambu")
                return "14330";
            if (kelurahan == "Papanggo")
                return "14340";
            if (kelurahan == "Warakas")
                return "14340";
            if (kelurahan == "Sunter Agung")
                return "14350";
            if (kelurahan == "Sunter Jaya")
                return "14350";
            if (kelurahan == "Pademangan Barat")
                return "14420";
            if (kelurahan == "Pademangan Timur")
                return "14410";
            if (kelurahan == "Penjaringan")
                return "14430";
            if (kelurahan == "Kali Baru")
                return "14110";
            if (kelurahan == "Pluit")
                return "14440";
            if (kelurahan == "Cilincing")
                return "14120";
            if (kelurahan == "Kapuk Muara ")
                return "14460";
            if (kelurahan == "Kamal Muara")
                return "14470";
            if (kelurahan == "Semper Barat")
                return "14130";
            if (kelurahan == "Semper Timur")
                return "14130";
            if (kelurahan == "Pejagalan")
                return "14450";
            if (kelurahan == "Ancol")
                return "14430";
            if (kelurahan == "Sukapura")
                return "14140";
            if (kelurahan == "Marunda")
                return "14150";
            if (kelurahan == "Rorotan")
                return "14140";
            else
                return string.Empty;
        }
    }
}
