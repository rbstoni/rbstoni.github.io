using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum DrawerVariant
    {
        [Description("temporary")]
        Temporary,
        //[Description("responsive")]
        //Responsive,
        [Description("persistent")]
        Persistent,
        [Description("mini")]
        Mini
    }
}
