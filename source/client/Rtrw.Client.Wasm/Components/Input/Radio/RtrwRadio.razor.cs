using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Base;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input.Radio;
using Rtrw.Client.Wasm.Components.Services;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwRadio<T> : RtrwComponentBase, IDisposable
    {

        private string _elementId = "radio" + Guid.NewGuid().ToString().Substring(0, 8);
        private IRtrwRadioGroup _parent;

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public ThemeColor Color { get; set; } = ThemeColor.Default;
        [Parameter] public bool Dense { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public T Option { get; set; }
        [Parameter] public Placement Placement { get; set; } = Placement.Right;
        [Parameter] public ThemeColor? UnCheckedColor { get; set; } = null;
        internal bool Checked { get; private set; }
        [CascadingParameter]
        internal IRtrwRadioGroup IRtrwRadioGroup
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);
                //RtrwRadioGroup<T>?.Add(this);
            }
        }
        internal RtrwRadioGroup<T> RtrwRadioGroup => (RtrwRadioGroup<T>)IRtrwRadioGroup;
        protected string ButtonClassname =>
        new CssBuilder("rtrw-button-root rtrw-icon-button")
            .AddClass($"rtrw-{Color.EnumToDescriptionString()}-text hover:rtrw-{Color.EnumToDescriptionString()}-hover", UnCheckedColor == null || (UnCheckedColor != null && Checked == true))
            .AddClass($"rtrw-{UnCheckedColor?.EnumToDescriptionString()}-text hover:rtrw-{UnCheckedColor?.EnumToDescriptionString()}-hover", UnCheckedColor != null && Checked == false)
            .AddClass($"rtrw-radio-dense", Dense)
            .AddClass($"rtrw-disabled", Disabled)
            .AddClass($"rtrw-checked", Checked)
            .Build();
        protected string CheckedIconClassName =>
        new CssBuilder("rtrw-icon-root rtrw-svg-icon rtrw-radio-icon-checked")
            .Build();
        protected string Classname
            => new CssBuilder("rtrw-radio")
            .AddClass($"rtrw-disabled", Disabled)
            .AddClass($"rtrw-radio-content-placement-{Placement.EnumToDescriptionString()}")
            .AddClass(Class)
            .Build();
        protected string RadioIconsClassNames =>
        new CssBuilder("rtrw-radio-icons")
            .AddClass($"rtrw-checked", Checked)
            .Build();
        protected string UnCheckedIconClassName =>
        new CssBuilder("rtrw-icon-root rtrw-svg-icon")
            .Build();
        [Parameter] public int IconSize { get; set; } = 24;
        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }

        public void Dispose()
        {
            RtrwRadioGroup?.UnregisterRadio(this);
        }
        public void Select()
        {
            RtrwRadioGroup?.SetSelectedRadioAsync(this).AndForget();
        }
        internal Task OnClick()
        {
            if (RtrwRadioGroup != null)
                return RtrwRadioGroup.SetSelectedRadioAsync(this);

            return Task.CompletedTask;
        }
        internal void SetChecked(bool value)
        {
            if (Checked != value)
            {
                Checked = value;
                StateHasChanged();
            }
        }
        protected internal void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled)
                return;
            switch (obj.Key)
            {
                case "Enter":
                case "NumpadEnter":
                case " ":
                    Select();
                    break;
                case "Backspace":
                    RtrwRadioGroup.Reset();
                    break;
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "rtrw-button-root",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none", PreventUp = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                    },
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (RtrwRadioGroup != null)
                await RtrwRadioGroup.RegisterRadioAsync(this);
        }

    }
}
