using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Enums
{
    public enum ContactType
    {
        [Description("Personal")]
        Personal = 0,

        [Description("Group")]
        Group = 1
    }
}
