using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum FlexAlignSelf
    {
        [Description("auto")]
        Auto,
        [Description("start")]
        FlexStart,
        [Description("end")]
        FlexEnd,
        [Description("center")]
        Center,
        [Description("baseline")]
        BaseLine,
        [Description("stretch")]
        Stretch,
    }
}
