using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Input
{
    internal interface IRtrwSelect
    {
        void CheckGenericTypeMatch(object select_item);
        bool MultiSelection { get; set; }
    }

    internal interface IRtrwShadowSelect
    {
    }
}
