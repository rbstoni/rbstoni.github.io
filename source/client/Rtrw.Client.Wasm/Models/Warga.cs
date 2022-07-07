using Rtrw.Client.Wasm.Enums;

namespace Rtrw.Client.Wasm.Models
{
    public class Warga
    {
        public string Id { get; set; } = $"warga-{Guid.NewGuid().ToString("N")}";
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string Email { get; set; }
        public string Password { get; set; } = "p4ssw0rd";
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? ProfileUrl { get; set; }
        public Gender? Gender { get; set; }
        public Geocoder? Location { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<Post>? Posts { get; set; } = new();
        public List<Contact>? Contacts { get; set; } = new();
        public string? Token { get; set; } = default!;
    }
}
