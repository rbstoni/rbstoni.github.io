using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum Placement
    {
        [Description("left")]
        Left,
        [Description("right")]
        Right,
        [Description("end")]
        End,
        [Description("start")]
        Start,
        [Description("top")]
        Top,
        [Description("bottom")]
        Bottom
    }
}
