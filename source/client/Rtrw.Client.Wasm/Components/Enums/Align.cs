using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum Align
    {
        [Description("inherit")]
        Inherit,
        [Description("left")]
        Left,
        [Description("center")]
        Center,
        [Description("right")]
        Right,
        [Description("justify")]
        Justify,
        [Description("start")]
        Start,
        [Description("end")]
        End,
    }
}
