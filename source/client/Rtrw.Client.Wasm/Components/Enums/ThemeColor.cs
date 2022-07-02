using System.ComponentModel;

namespace Rtrw.Client.Wasm.Components.Enums
{
    public enum ThemeColor
    {
        [Description("default")]
        Default,
        [Description("primary")]
        Primary,
        [Description("secondary")]
        Secondary,
        [Description("info")]
        Info,
        [Description("success")]
        Success,
        [Description("warning")]
        Warning,
        [Description("danger")]
        Error,
        [Description("dark")]
        Dark,
        [Description("light")]
        Light,
        [Description("transparent")]
        Transparent,
        [Description("inherit")]
        Inherit
    }
}
