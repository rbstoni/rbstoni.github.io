using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum Position
    {
        [Description("static")]
        Static,
        [Description("relative")]
        Relative,
        [Description("absolute")]
        Absolute,
        [Description("sticky")]
        Sticky
    }
}
