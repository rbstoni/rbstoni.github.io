using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum ObjectFit
    {
        [Description("none")]
        None,
        [Description("cover")]
        Cover,
        [Description("contain")]
        Contain,
        [Description("fill")]
        Fill,
        [Description("scale-down")]
        ScaleDown
    }
}
