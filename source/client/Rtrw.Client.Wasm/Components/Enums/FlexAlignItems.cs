using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum FlexAlignItems
    {
        [Description("start")]
        FlexStart = 0,
        [Description("end")]
        FlexEnd = 1,
        [Description("center")]
        Center = 2,
        [Description("baseline")]
        BaseLine = 3,
    }
}
