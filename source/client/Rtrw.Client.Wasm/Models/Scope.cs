namespace Rtrw.Client.Wasm.Models
{
    public enum Scope
    {
        SemuaWargaRtrw,
        WargaLebihLuas,
        WargaSekitar,
    }

    public class Audience
    {
        protected internal Audience(Scope scope, string name, string desc)
        {
            Scope = scope;
            Name = name;
            Description = desc;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public Scope Scope { get; set; }

        public static Audience SemuaWargaRtrw => new(Scope.SemuaWargaRtrw,"Semua Warga RTRW","Area yang lebih luas di kota Anda");
        public static Audience WargaLebihLuas => new(Scope.WargaLebihLuas, "Warga Lebih Luas", "Kelurahan sekitar Anda");
        public static Audience WargaSekitar => new(Scope.WargaSekitar, "Warga Sekitar", "Hanya di kelurahan Anda");

        public static List<Audience> AllScopes = new() { SemuaWargaRtrw, WargaLebihLuas, WargaSekitar };
    }
}

