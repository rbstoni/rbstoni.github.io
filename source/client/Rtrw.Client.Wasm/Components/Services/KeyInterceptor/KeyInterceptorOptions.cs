// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Rtrw.Client.Wasm.Components.Services
{
    public class KeyInterceptorOptions
    {
        public string TargetClass { get; set; }
        public bool EnableLogging { get; set; } = false;
        public List<KeyOptions> Keys { get; set; } = new List<KeyOptions>();
    }

    public class KeyOptions
    {
        public string Key { get; set; }
        public bool SubscribeDown { get; set; }
        public bool SubscribeUp { get; set; }
        public string PreventDown { get; set; } = "none";
        public string PreventUp { get; set; } = "none";
        public string StopDown { get; set; } = "none";
        public string StopUp { get; set; } = "none";
    }
}
