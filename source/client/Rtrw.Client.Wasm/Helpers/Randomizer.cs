namespace Rtrw.Client.Wasm.Helpers
{
    public static class Randomizer
    {
        public static string GenerateRandomHexColor()
        {
            Random random = new Random();
            var bytes = new Byte[30];
            random.NextBytes(bytes);
            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = String.Concat(hexArray);

            return $"#{hexStr}";
        }
    }
}
