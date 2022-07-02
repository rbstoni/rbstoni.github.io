using Microsoft.AspNetCore.Components;

namespace Rtrw.Client.Wasm.Components.Base
{
    public abstract class RtrwComponentBase : ComponentBase
    {
        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public object Tag { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

        public string FieldId => (UserAttributes?.ContainsKey("id") == true ? UserAttributes["id"].ToString() : $"rtrw-input-{Guid.NewGuid()}");
    }
}
