using Microsoft.AspNetCore.Components;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.Shared
{
    public partial class RtrwBisnisLokalCard
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Parameter] public List<ProductResponse> Products { get; set; } = new();
        void NavigateTo(string href)
        {
            NavigationManager.NavigateTo(href);
        }
    }

    public class ProductResponse
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public Geocoder Location { get; set; }
        public string ProductUrl { get; set; }
        public string ProductAvatar { get; set; }
        public string LikeCount { get; set; }
    }
}
