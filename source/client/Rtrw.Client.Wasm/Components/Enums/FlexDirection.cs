using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum FlexDirection
    {
        [Description("row")]
        Row = 0,
        [Description("column")]
        Column = 1,
        [Description("row-reverse")]
        RowReverse = 2,
        [Description("column-reverse")]
        ColumnReverse = 3,
    }
}
