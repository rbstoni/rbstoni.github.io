namespace Rtrw.Client.Wasm.Components.Input.Radio
{
    internal interface IRtrwRadioGroup
    {
        //This interface need to throw exception properly.
        void CheckGenericTypeMatch(object select_item);
    }
}
