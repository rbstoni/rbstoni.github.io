using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum ModalPosition
    {
        [Description("center")]
        Center,
        [Description("centerleft")]
        CenterLeft,
        [Description("centerright")]
        CenterRight,
        [Description("topcenter")]
        TopCenter,
        [Description("topleft")]
        TopLeft,
        [Description("topright")]
        TopRight,
        [Description("bottomcenter")]
        BottomCenter,
        [Description("bottomleft")]
        BottomLeft,
        [Description("bottomright")]
        BottomRight,
        [Description("custom")]
        Custom
    }
}
