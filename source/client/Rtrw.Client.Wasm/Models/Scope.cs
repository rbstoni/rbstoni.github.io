using System.ComponentModel;

namespace Rtrw.Client.Wasm.Models
{
    public enum Scope
    {
        [Description("Semua Warga")]
        SemuaWarga,
        [Description("Warga Sekitar")]
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

        public static Audience SemuaWarga => new(Scope.SemuaWarga, "Semua Warga", "Area yang lebih luas di kota kamu");
        public static Audience WargaSekitar => new(Scope.WargaSekitar, "Warga Sekitar", "Hanya di kelurahan kamu");

        public static List<Audience> AllScopes = new() { SemuaWarga, WargaSekitar };
    }
}

