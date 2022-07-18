using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum FlexJustifyContent
    {
        [Description("start")]
        FlexStart = 0,
        [Description("end")]
        FlexEnd = 1,
        [Description("center")]
        Center = 2,
        [Description("space-between")]
        SpaceBetween = 3,
        [Description("space-around")]
        SpaceAround = 4,
        [Description("space-evenly")]
        SpaceEvenly = 5,
    }
}
