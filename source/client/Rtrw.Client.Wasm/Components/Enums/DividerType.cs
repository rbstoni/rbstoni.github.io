using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum DividerType
    {
        [Description("fullwidth")]
        FullWidth,
        [Description("inset")]
        Inset,
        [Description("middle")]
        Middle
    }
}
