using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum ObjectPosition
    {
        [Description("center")]
        Center,
        [Description("top")]
        Top,
        [Description("bottom")]
        Bottom,
        [Description("left")]
        Left,
        [Description("left-top")]
        LeftTop,
        [Description("left-bottom")]
        LeftBottom,
        [Description("right")]
        Right,
        [Description("right-top")]
        RightTop,
        [Description("right-bottom")]
        RightBottom,
    }
}
