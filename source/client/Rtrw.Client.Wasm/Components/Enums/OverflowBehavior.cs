using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum OverflowBehavior
    {
        [Description("flip-never")]
        FlipNever,
        [Description("flip-onopen")]
        FlipOnOpen,
        [Description("flip-always")]
        FlipAlways,
    }
}
