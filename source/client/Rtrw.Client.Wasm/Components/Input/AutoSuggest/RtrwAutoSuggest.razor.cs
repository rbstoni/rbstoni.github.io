using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Components.Enums;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Input;
using Rtrw.Client.Wasm.Components.Services.Scroll;
using Rtrw.Client.Wasm.Components.Utilities;
using Rtrw.Client.Wasm.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Components
{
    public partial class RtrwAutoSuggest<T> : RtrwBaseInput<T>, IDisposable
    {
        private readonly string _componentId = Guid.NewGuid().ToString();
        private bool _dense;
        int _elementKey = 0;
        private RtrwInput<string>? _elementReference;
        private IList<int> _enabledItemIndices = new List<int>();
        private bool _isCleared;
        private bool _isOpen;
        private T[]? _items;
        private int _itemsReturned;
        private int _selectedListItemIndex = 0;
        private Timer? _timer;
        private Func<T, string>? _toStringFunc;

        [Parameter] public Origin AnchorOrigin { get; set; } = Origin.BottomCenter;
        [Parameter] public bool Clearable { get; set; } = false;
        [Parameter] public bool CoerceText { get; set; } = true;
        [Parameter] public bool CoerceValue { get; set; }
        [Parameter] public int DebounceInterval { get; set; } = 100;
        [Parameter]
        public bool Dense
        {
            get { return _dense; }
            set { _dense = value; }
        }
        public bool IsOpen
        {
            get => _isOpen;
            // Note: the setter is protected because it was needed by a user who derived his own autocomplete from this class.
            // Note: setting IsOpen will not open or close it. Use ToggleMenu() for that. 
            protected set
            {
                if (value == _isOpen)
                    return;
                _isOpen = value;

                IsOpenChanged.InvokeAsync(_isOpen).AndForget();
            }
        }
        [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
        [Parameter] public Func<T, bool>? ItemDisabledFunc { get; set; }
        [Parameter] public RenderFragment<T>? ItemDisabledTemplate { get; set; }
        [Parameter] public RenderFragment<T>? ItemSelectedTemplate { get; set; }
        [Parameter] public RenderFragment<T>? ItemTemplate { get; set; }
        [Parameter] public int MaxHeight { get; set; } = 300;
        [Parameter] public int? MaxItems { get; set; } = 10;
        [Parameter] public int MinCharacters { get; set; } = 0;
        [Parameter] public RenderFragment? MoreItemsTemplate { get; set; }
        [Parameter] public RenderFragment? NoItemsTemplate { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
        [Parameter] public string? PopoverClass { get; set; }
        [Parameter] public bool ResetValueOnEmptyText { get; set; } = false;
        [Parameter] public Func<string, Task<IEnumerable<T>>>? SearchFunc { get; set; }
        [Parameter] public bool SelectOnClick { get; set; } = true;
        [Parameter] public bool SelectValueOnTab { get; set; } = false;
        [Parameter]
        public Func<T, string>? ToStringFunc
        {
            get => _toStringFunc;
            set
            {
                if (_toStringFunc == value) return;
                _toStringFunc = value;
                Converter = new Converter<T>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }
        [Parameter] public Origin TransformOrigin { get; set; } = Origin.TopCenter;
        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();
        [Inject] IScrollManager? ScrollManager { get; set; }
        public async Task Clear()
        {
            _isCleared = true;
            IsOpen = false;
            await SetTextAsync(string.Empty, updateValue: false);
            await CoerceValueToText();
            if (_elementReference != null)
                await _elementReference.SetText("");
            _timer?.Dispose();
            StateHasChanged();
        }
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }
        public ValueTask ScrollToListItem(int index)
        {
            var id = GetListItemId(index);
            //id of the scrolled element
            return ScrollManager.ScrollToListItemAsync(id);
        }
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }
        public async Task SelectOption(T value)
        {
            await SetValueAsync(value);
            if (_items != null)
                _selectedListItemIndex = Array.IndexOf(_items, value);
            var optionText = GetItemString(value);
            if (!_isCleared)
                await SetTextAsync(optionText, false);
            _timer?.Dispose();
            IsOpen = false;
            BeginValidate();
            if (!_isCleared)
                _elementReference?.SetText(optionText);
            _elementReference?.FocusAsync().AndForget();
            StateHasChanged();
        }
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }
        public async Task ToggleMenu()
        {
            if ((Disabled || ReadOnly) && !IsOpen)
                return;
            await ChangeMenu(!IsOpen);
        }
        internal Task OnEnterKey()
        {
            if (IsOpen == false)
                return Task.CompletedTask;
            if (_items == null || _items.Length == 0)
                return Task.CompletedTask;
            if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                return SelectOption(_items[_selectedListItemIndex]);
            return Task.CompletedTask;
        }
        internal virtual async Task OnInputKeyDown(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Tab":
                    // NOTE: We need to catch Tab in Keydown because a tab will move focus to the next element and thus
                    // in OnInputKeyUp we'd never get the tab key
                    if (!IsOpen)
                        return;
                    if (SelectValueOnTab)
                        await OnEnterKey();
                    else
                        IsOpen = false;
                    break;
            }
        }
        internal virtual async Task OnInputKeyUp(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        await OnEnterKey();
                    }
                    break;
                case "ArrowDown":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        var increment = _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) + 1) - _selectedListItemIndex;
                        await SelectNextItem(increment < 0 ? 1 : increment);
                    }
                    break;
                case "ArrowUp":
                    if (args.AltKey == true)
                    {
                        await ChangeMenu(open: false);
                    }
                    else if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        var decrement = _selectedListItemIndex - _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) - 1);
                        await SelectNextItem(-(decrement < 0 ? 1 : decrement));
                    }
                    break;
                case "Escape":
                    await ChangeMenu(open: false);
                    break;
                case "Tab":
                    await Task.Delay(1);
                    if (!IsOpen)
                        return;
                    if (SelectValueOnTab)
                        await OnEnterKey();
                    else
                        await ToggleMenu();
                    break;
                case "Backspace":
                    if (args.CtrlKey == true && args.ShiftKey == true)
                    {
                        Reset();
                    }
                    break;
            }
            base.InvokeKeyUp(args);
        }
        protected override void Dispose(bool disposing)
        {
            _timer?.Dispose();
            base.Dispose(disposing);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            _isCleared = false;
            base.OnAfterRender(firstRender);
        }
        protected override void OnInitialized()
        {
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }
        protected override async void ResetValue()
        {
            await Clear();
            base.ResetValue();
        }
        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            _timer?.Dispose();
            // This keeps the text from being set when clear() was called
            if (_isCleared)
                return Task.CompletedTask;
            return base.UpdateTextPropertyAsync(updateValue);
        }
        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            _timer?.Dispose();
            if (ResetValueOnEmptyText && string.IsNullOrWhiteSpace(Text))
                await SetValueAsync(default(T), updateText);
            if (DebounceInterval <= 0)
                await OnSearchAsync();
            else
                _timer = new Timer(OnTimerComplete, null, DebounceInterval, Timeout.Infinite);
        }
        private async Task ChangeMenu(bool open)
        {
            if (open)
            {
                if (SelectOnClick)
                    await _elementReference.SelectAsync();
                await OnSearchAsync();
            }
            else
            {
                _timer?.Dispose();
                RestoreScrollPosition();
                await CoerceTextToValue();
                IsOpen = false;
                StateHasChanged();
            }
        }
        private Task CoerceTextToValue()
        {
            if (CoerceText == false)
                return Task.CompletedTask;

            _timer?.Dispose();

            var text = Value == null ? null : GetItemString(Value);

            // Don't update the value to prevent the popover from opening again after coercion
            if (text != Text)
                return SetTextAsync(text, updateValue: false);

            return Task.CompletedTask;
        }
        private Task CoerceValueToText()
        {
            if (CoerceValue == false)
                return Task.CompletedTask;
            _timer?.Dispose();
            var value = Converter.Get(Text);
            return SetValueAsync(value, updateText: false);
        }
        private string GetItemString(T item)
        {
            if (item == null)
                return string.Empty;
            try
            {
                return Converter.Set(item);
            }
            catch (NullReferenceException) { }
            return "null";
        }
        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }
        private async Task ListItemOnClick(T item)
        {
            await SelectOption(item);
        }
        private Task OnInputBlurred(FocusEventArgs args)
        {
            OnBlur.InvokeAsync(args);
            return Task.CompletedTask;
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value,
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
        }
        private async Task OnSearchAsync()
        {
            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                IsOpen = false;
                StateHasChanged();
                return;
            }

            IEnumerable<T> searched_items = Array.Empty<T>();
            try
            {
                searched_items = (await SearchFunc(Text)) ?? Array.Empty<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("The search function failed to return results: " + e.Message);
            }
            _itemsReturned = searched_items.Count();
            if (MaxItems.HasValue)
            {
                searched_items = searched_items.Take(MaxItems.Value);
            }
            _items = searched_items.ToArray();

            _enabledItemIndices = _items.Select((item, idx) => (item, idx)).Where(tuple => ItemDisabledFunc?.Invoke(tuple.item) != true).Select(tuple => tuple.idx).ToList();
            _selectedListItemIndex = _enabledItemIndices.Any() ? _enabledItemIndices.First() : -1;

            IsOpen = true;

            if (_items?.Length == 0)
            {
                await CoerceValueToText();
                StateHasChanged();
                return;
            }

            StateHasChanged();
        }
        private async Task OnTextChanged(string text)
        {
            await base.TextChanged.InvokeAsync();

            if (text == null)
                return;
            await SetTextAsync(text, true);
        }
        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnSearchAsync);
        //This restores the scroll position after closing the menu and element being 0
        private void RestoreScrollPosition()
        {
            if (_selectedListItemIndex != 0) return;
            ScrollManager.ScrollToListItemAsync(GetListItemId(0));
        }
        private ValueTask SelectNextItem(int increment)
        {
            if (increment == 0 || _items == null || _items.Length == 0 || !_enabledItemIndices.Any())
                return ValueTask.CompletedTask;
            // if we are at the end, or the beginning we just do an rollover
            _selectedListItemIndex = Math.Clamp(value: (10 * _items.Length + _selectedListItemIndex + increment) % _items.Length, min: 0, max: _items.Length - 1);
            return ScrollToListItem(_selectedListItemIndex);
        }

    }
}
