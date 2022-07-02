using Bogus;
using Bogus.DataSets;
using Rtrw.Client.Wasm.Enums;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.FakeData
{
    public class Dummy
    {
        public Comment FakeComment => GenerateFakeComment();
        public Medium FakeImage => GenerateFakeImage();
        public Geocoder FakeLocation => GenerateFakeAddress();
        public Post FakePost => GenerateFakePost();
        public Reaction FakeReaction => GenerateFakeReaction();
        public Comment FakeReply => GenerateFakeReply();
        public Warga FakeWarga => GenerateFakeWarga();
        string DummyText => Lorem.Paragraph(50);
        Lorem Lorem => new(locale: "id_ID");

        Faker<Geocoder> GenerateFakeAddress()
        {
            return new Faker<Geocoder>("id_ID")
                .RuleFor(a=> a.Provinsi,"DKI Jakarta")
                .RuleFor(a=> a.KabupatenKota,"Kota Jakarta Utara")
                .RuleFor(a=> a.Kecamatan,"Pademangan")
                .RuleFor(a=> a.Kelurahan,"Pademangan Barat")
                .RuleFor(a=> a.Alamat,f=> f.Address.StreetAddress())
                .RuleFor(a=> a.KodePos,f=> f.Address.ZipCode())
                .RuleFor(a=> a.Longitude,GetRandomNumber(-180, 180).ToString())
                .RuleFor(a=> a.Latitude,GetRandomNumber(-90, 90).ToString());
        }

        Faker<Comment> GenerateFakeComment() => new Faker<Comment>("id_ID")
            .RuleFor(x=> x.CreatedAt,f=> f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
            .RuleFor(x=> x.Commenter,GenerateFakeWarga())
            .RuleFor(x=> x.Media,f=> GenerateFakeImage().GenerateBetween(0, 5))
            .RuleFor(x=> x.Text,Lorem.Paragraph(10))
            .RuleFor(x=> x.Reactions,GenerateFakeReaction().GenerateBetween(0, 100))
            .RuleFor(x=> x.Replies,GenerateFakeReply().GenerateBetween(0, 100));

        Faker<Medium> GenerateFakeImage() => new Faker<Medium>("id_ID")
            .RuleFor(x=> x.CreatedAt,DateTime.Now)
            .RuleFor(x=> x.Type,"Image")
            .RuleFor(x=> x.MediumUrl,f=> f.Image.PicsumUrl());

        Faker<Post> GenerateFakePost() => new Faker<Post>("id_ID")
            .RuleFor(x=> x.CreatedAt,f=> f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
            //.RuleFor(x => x.Category, f => f.PickRandom<PostCategory>())
            .RuleFor(x=> x.Author,GenerateFakeWarga())
            .RuleFor(x=> x.Scope,f=> f.PickRandom<Scope>())
            .RuleFor(x=> x.PostLocation,GenerateFakeAddress())
            .RuleFor(x=> x.Text,Lorem.Paragraph(10))
            .RuleFor(
                x
                    => x.Media,
                f
                    => GenerateFakeImage().GenerateBetween(0, 5))
            .RuleFor(
                x
                    => x.Comments,
                f
                    => GenerateFakeComment().GenerateBetween(0, 100))
            .RuleFor(
                x
                    => x.Reactions,
                f
                    => GenerateFakeReaction().GenerateBetween(0, 100));

        Faker<Reaction> GenerateFakeReaction() => new Faker<Reaction>()
            .RuleFor(
                x
                    => x.CreatedAt,
                DateTime.Now)
            .RuleFor(
                x
                    => x.Reactor,
                GenerateFakeWarga())
            .RuleFor(
                x
                    => x.Emoji,
                f
                    => f.PickRandom<Emoji>());

        Faker<Comment> GenerateFakeReply() => new Faker<Comment>("id_ID")
            .RuleFor(
                x
                    => x.CreatedAt,
                f
                    => f.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 5, 31)))
            .RuleFor(
                x
                    => x.Commenter,
                GenerateFakeWarga())
            .RuleFor(
                x
                    => x.Text,
                Lorem.Paragraph(10))
            .RuleFor(
                x
                    => x.Reactions,
                GenerateFakeReaction().GenerateBetween(0, 100));

        Faker<Warga> GenerateFakeWarga() => new Faker<Warga>("id_ID")
            .RuleFor(
                x
                    => x.FirstName,
                (f, x)
                    => f.Person.FirstName)
            .RuleFor(
                x
                    => x.LastName,
                (f, x)
                    => f.Person.LastName)
            .RuleFor(
                x
                    => x.Gender,
                f
                    => f.PickRandom<Gender>())
            .RuleFor(
                x
                    => x.Email,
                f
                    => f.Person.Email)
            .RuleFor(
                x
                    => x.ProfileUrl,
                (f, x)
                    => $"profile/{x.Id}")
            .RuleFor(
                x
                    => x.DateOfBirth,
                f
                    => f.Person.DateOfBirth)
            .RuleFor(
                x
                    => x.PhoneNumber,
                f
                    => f.Person.Phone)
            .RuleFor(
                x
                    => x.AvatarUrl,
                f
                    => f.Internet.Avatar())
            .RuleFor(
                x
                    => x.Location,
                GenerateFakeAddress());

        private double GetRandomNumber(int minimum, int maximum)
        {
            var random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
