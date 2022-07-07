using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum Transition
    {
        [Description("None")]
        None = 0,
        [Description("Fade")]
        Fade = 1,
        [Description("Fade")]
        Slide = 2,
        [Description("Custom")]
        Custom = 99
    }
}
