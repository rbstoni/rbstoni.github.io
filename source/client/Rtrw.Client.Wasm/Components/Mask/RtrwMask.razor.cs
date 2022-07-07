using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input.Base;
using Rtrw.Client.Wasm.Components.Interfaces;
using Rtrw.Client.Wasm.Components.Mask;
using Rtrw.Client.Wasm.Components.Services;
using Rtrw.Client.Wasm.Components.Services.JsEvents;
using Rtrw.Client.Wasm.Utilities;
using System.Text.RegularExpressions;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwMask : RtrwBaseInput<string>, IDisposable
    {

        private int caret;
        private string elementId = "mask_" + Guid.NewGuid().ToString().Substring(0, 8);
        private ElementReference elementReference;
        private ElementReference elementReference1;
        private IJsEvent jsEvent;
        private IMask mask = new PatternMask("** **-** **");
        private (int, int)? selection;
        private bool showClearable;
        private bool updating;

        public RtrwMask() { TextUpdateSuppression = false; }

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public RenderFragment AdornmentContent { get; set; }
        [Parameter] public RenderFragment AdornmentSvg { get; set; }
        [Parameter] public bool Clearable { get; set; } = false;
        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public IMask Mask { get => mask; set => SetMask(value); }
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        protected string AdornmentClassname => new CssBuilder("rtrw-input-adornment")
                .AddClass($"rtrw-input-adornment-{Adornment.EnumToDescriptionString()}", Adornment != Adornment.None)
            .AddClass($"rtrw-input-root-filled-shrink", Variant == Variant.Filled)
            .AddClass(Class)
            .Build();
        protected string Classname => new CssBuilder("rtrw-input")
                .AddClass($"rtrw-input-{Variant.EnumToDescriptionString()}")
            .AddClass($"rtrw-input-adorned-{Adornment.EnumToDescriptionString()}", Adornment != Adornment.None)
            .AddClass(
                $"rtrw-input-margin-{Margin.EnumToDescriptionString()}",
                when: ()
                    => Margin != Margin.None)
            .AddClass(
                "rtrw-input-underline",
                when: ()
                    => DisableUnderLine == false && Variant != Variant.Outlined)
            .AddClass(
                "rtrw-shrink",
                when: ()
                    => !string.IsNullOrEmpty(Text) ||
                    Adornment == Adornment.Start ||
                    !string.IsNullOrWhiteSpace(Placeholder))
            .AddClass("rtrw-disabled", Disabled)
            .AddClass("rtrw-input-error", HasErrors)
            .AddClass("rtrw-ltr", GetInputType() == InputType.Email || GetInputType() == InputType.Telephone)
            .AddClass(Class)
            .Build();
        protected string ClearButtonClassname => new CssBuilder()
                // .AddClass("me-n1", Adornment == Adornment.End && HideSpinButtons == false)
                .AddClass("rtrw-icon-button-edge-end", Adornment == Adornment.End)
            // .AddClass("me-6", Adornment != Adornment.End && HideSpinButtons == false)
            .AddClass("rtrw-icon-button-edge-margin-end", Adornment != Adornment.End)
            .Build();
        protected string InputClassname => new CssBuilder("rtrw-input-slot")
                .AddClass("rtrw-input-root")
            .AddClass($"rtrw-input-root-{Variant.EnumToDescriptionString()}")
            .AddClass($"rtrw-input-root-adorned-{Adornment.EnumToDescriptionString()}", Adornment != Adornment.None)
            .AddClass(
                $"rtrw-input-root-margin-{Margin.EnumToDescriptionString()}",
                when: ()
                    => Margin != Margin.None)
            .AddClass(Class)
            .Build();
        [Inject] private IJsApiService _jsApiService { get; set; }
        [Inject] private IJsEventFactory jsEventFactory { get; set; }

        /// <summary>
        /// Clear the text field.
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            Mask.Clear();
            return Update();
        }
        public override ValueTask FocusAsync() { return elementReference.FocusAsync(); }
        public void OnSelect(int start, int end)
        {
            Mask.Selection = selection = (start, end);
            //Console.WriteLine($"OnSelect: {Mask}");
        }
        internal override InputType GetInputType() => InputType;
        internal async void HandleClearButton(MouseEventArgs e)
        {
            Mask.Clear();
            await Update();
            await elementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }
        // from JS event     
        internal void OnCaretPositionChanged(int pos)
        {
            if (Mask.Selection != null)
            {
                // do not clear selection if pos change is at selection border
                var sel = Mask.Selection.Value;
                if (pos == sel.Item1 || pos == sel.Item2)
                    return;
            }

            if (pos == Mask.CaretPos)
                return;
            Mask.Selection = null;
            Mask.CaretPos = pos;
            //Console.WriteLine($"OnCaretPositionChanged: '{Mask}' ({pos})");
        }
        //public override ValueTask SelectAsync()
        //{
        //    return elementReference.RtrwSelectAsync();
        //}
        //public override ValueTask SelectRangeAsync(int pos1, int pos2)
        //{
        //    return elementReference.RtrwSelectRangeAsync(pos1, pos2);
        //}
        internal void OnCopy()
        {
            //Console.WriteLine($"Copy: {text}");
            var text = Text;
            if (Mask.Selection != null)
            {
                (_, text, _) = BaseMask.SplitSelection(text, Mask.Selection.Value);
            }
            _jsApiService.CopyToClipboardAsync(text);
        }
        internal void OnFocused(FocusEventArgs obj)
        {
            isFocused = true;
            //Console.WriteLine($"OnFocused: {Mask}");
        }
        internal async void OnPaste(string text)
        {
            //Console.WriteLine($"Paste: {text}");
            if (text == null)
                return;
            Mask.Insert(text);
            await Update();
        }
        protected internal async Task HandleKeyDown(KeyboardEventArgs e)
        {
            try
            {
                if ((e.CtrlKey && e.Key != "Backspace") || e.AltKey)
                    return;
                // Console.WriteLine($"HandleKeyDown: '{e.Key}'");
                switch (e.Key)
                {
                    case "Backspace":
                        if (e.CtrlKey)
                        {
                            Mask.Clear();
                            await Update();
                            return;
                        }
                        Mask.Backspace();
                        await Update();
                        return;
                    case "Delete":
                        Mask.Delete();
                        await Update();
                        return;
                }

                if (Regex.IsMatch(e.Key, @"^.$"))
                {
                    Mask.Insert(e.Key);
                    await Update();
                }
            }
            finally
            {
                // call user callback
                await OnKeyDown.InvokeAsync(e);
            }
        }
        protected internal override void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            isFocused = false;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing == true)
            {
                jsEvent?.Dispose();
                //_keyInterceptor?.Dispose();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                jsEvent = jsEventFactory.Create();

                await jsEvent.Connect(
                    elementId,
                    new JsEventOptions
                    {
                        //EnableLogging = true,
                        TargetClass = "rtrw-input-slot",
                        TagName = "INPUT"
                    });
                jsEvent.CaretPositionChanged += OnCaretPositionChanged;
                jsEvent.Paste += OnPaste;
                jsEvent.Select += OnSelect;

                //_keyInterceptor = _keyInterceptorFactory.Create();

                //await _keyInterceptor.Connect(elementId, new KeyInterceptorOptions()
                //{
                //    //EnableLogging = true,
                //    TargetClass = "rtrw-input-slot",
                //    Keys =
                //    {
                //        new KeyOptions
                //        {
                //            Key = " ", PreventDown = "key+none"
                //        }, //prevent scrolling page, toggle open/close
                //        new KeyOptions { Key = "ArrowUp", PreventDown = "key+none" }, // prevent scrolling page
                //        new KeyOptions { Key = "ArrowDown", PreventDown = "key+none" }, // prevent scrolling page
                //        new KeyOptions { Key = "PageUp", PreventDown = "key+none" }, // prevent scrolling page
                //        new KeyOptions { Key = "PageDown", PreventDown = "key+none" }, // prevent scrolling page
                //        new KeyOptions { Key = @"/^.$/", PreventDown = "key+none|key+shift" },
                //        new KeyOptions { Key = "/./", SubscribeDown = true },
                //        new KeyOptions { Key = "Backspace", PreventDown = "key+none" },
                //        new KeyOptions { Key = "Delete", PreventDown = "key+none" },
                //    },
                //});
                //_keyInterceptor.KeyDown += e => HandleKeyDown(e).AndForget();
            }
            if (isFocused && Mask.Selection == null)
                SetCaretPosition(Mask.CaretPos, selection, render: false);
            await base.OnAfterRenderAsync(firstRender);
        }
        protected override async Task OnInitializedAsync()
        {
            //Console.WriteLine($"OnInitialized Text:{Text}, Value:{Value}, Mask:{Mask}");
            if (Text != Mask.Text)
                await SetTextAsync(Mask.Text, updateValue: false);
            await base.OnInitializedAsync();
        }
        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            // allow this only via changes from the outside
            if (updating)
                return;
            var text = Converter.Set(Value);
            var cleanText = Mask.GetCleanText();
            if (cleanText == text || string.IsNullOrEmpty(cleanText) && string.IsNullOrEmpty(text))
                return;
            var maskText = Mask.Text;
            Mask.SetText(text);
            if (maskText == Mask.Text)
                return; // no change, stop update loop
            //Console.WriteLine("UpdateTextPropertyAsync: " + Mask);
            await Update();
        }
        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            // allow this only via changes from the outside
            if (updating)
                return;
            var text = Text;
            if (Mask.Text == text)
                return;
            var maskText = Mask.Text;
            Mask.SetText(text);
            if (maskText == Mask.Text)
                return; // no change, stop update loop
            //Console.WriteLine("UpdateValuePropertyAsync: " + Mask);
            await Update();
        }
        private string GetCounterText() => Counter == null
            ? string.Empty
            : (Counter == 0
                ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}")
                : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));
        private async void OnCut(ClipboardEventArgs obj)
        {
            if (selection != null)
                Mask.Delete();
            await Update();
            //Console.WriteLine($"OnCut: '{Mask}'");
        }
        private void SetCaretPosition(int _caret, (int, int)? _selection = null, bool render = true)
        {
            if (!isFocused)
                return;
            caret = _caret;
            if (caret == 0)
                caret = 0;
            selection = _selection;
            //if (selection == null)
            //{
            //    //Console.WriteLine("#Setting Caret Position: " + caret);
            //    elementReference.RtrwSelectRangeAsync(caret, caret).AndForget();
            //}
            //else
            //{
            //    var sel = selection.Value;
            //    //Console.WriteLine($"#Setting Selection: ({sel.Item1}..{sel.Item2})");
            //    elementReference.RtrwSelectRangeAsync(sel.Item1, sel.Item2).AndForget();
            //}
        }
        private void SetMask(IMask other)
        {
            if (mask == null || other == null || mask?.GetType() != other?.GetType())
            {
                mask = other;
                if (mask == null)
                    mask = new PatternMask("null ********"); // warn the user that the mask parameter is missing
                return;
            }

            // set new mask properties without loosing state
            mask.UpdateFrom(other);
        }
        private async Task Update()
        {
            var caret = Mask.CaretPos;
            var selection = Mask.Selection;
            var text = Mask.Text;
            var cleanText = Mask.GetCleanText();
            updating = true;
            try
            {
                await base.SetTextAsync(text, updateValue: false);
                if (Clearable)
                    UpdateClearable(Text);
                var v = Converter.Get(cleanText);
                Value = v;
                await ValueChanged.InvokeAsync(v);
                SetCaretPosition(caret, selection);
            }
            finally
            {
                updating = false;
            }
        }
        private void UpdateClearable(object value)
        {
            var _showClearable = Clearable && !string.IsNullOrWhiteSpace(Text);
            if (showClearable != _showClearable)
                showClearable = _showClearable;
        }

    }
}
