using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum DrawerClipMode
    {
        [Description("never")]
        Never,
        [Description("docked")]
        Docked,
        [Description("always")]
        Always
    }
}
