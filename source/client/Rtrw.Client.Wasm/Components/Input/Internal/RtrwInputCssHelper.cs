using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input.Base;
using Rtrw.Client.Wasm.Utilities;

namespace Rtrw.Client.Wasm.Components.Input.Internal
{
    internal static class RtrwInputCssHelper
    {
        public static string GetClassname<T>(RtrwBaseInput<T> baseInput, Func<bool> shrinkWhen) 
            => new CssBuilder("rtrw-input")
                .AddClass($"rtrw-input-{baseInput.Variant.EnumToDescriptionString()}")
                .AddClass($"rtrw-input-adorned-{baseInput.Adornment.EnumToDescriptionString()}",baseInput.Adornment != Adornment.None)
                .AddClass($"rtrw-input-margin-{baseInput.Margin.EnumToDescriptionString()}",when: ()=> baseInput.Margin != Margin.None)
                .AddClass("rtrw-input-underline",when: ()=> baseInput.DisableUnderLine == false && baseInput.Variant != Variant.Outlined)
                .AddClass("rtrw-shrink", when: shrinkWhen)
                .AddClass("rtrw-disabled", baseInput.Disabled)
                .AddClass("rtrw-input-error", baseInput.HasErrors)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetInputClassname<T>(RtrwBaseInput<T> baseInput) 
            => new CssBuilder("rtrw-input-slot")
                .AddClass("rtrw-input-root")
                .AddClass($"rtrw-input-root-{baseInput.Variant.EnumToDescriptionString()}")
                .AddClass($"rtrw-input-root-adorned-{baseInput.Adornment.EnumToDescriptionString()}",baseInput.Adornment != Adornment.None)
                .AddClass($"rtrw-input-root-margin-{baseInput.Margin.EnumToDescriptionString()}",when: ()=> baseInput.Margin != Margin.None)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetAdornmentClassname<T>(RtrwBaseInput<T> baseInput) 
            => new CssBuilder("rtrw-input-adornment")
                .AddClass($"rtrw-input-adornment-{baseInput.Adornment.EnumToDescriptionString()}",baseInput.Adornment != Adornment.None)
                .AddClass($"rtrw-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
                .AddClass(baseInput.Class)
                .Build();
    }
}