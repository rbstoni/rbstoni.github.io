using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum FlexWrap
    {
        [Description("nowrap")]
        NoWrap = 0,
        [Description("wrap")]
        Wrap = 1,
        [Description("wrap-reverse")]
        WrapReverse = 2,
    }
}
