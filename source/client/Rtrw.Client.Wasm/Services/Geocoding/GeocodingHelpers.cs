using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Services.Geocoding
{
    public static class GeocodingHelpers
    {
        public static string ConvertArrayOfStringToString(string[] stringArray)
        {
            StringBuilder builder = new();
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(stringArray[i]))
                {
                    builder.Append(stringArray[i]);
                    if (i != stringArray.Length - 1)
                    {
                        builder.Append("%2C");
                    }
                }
            }
            return builder.ToString();
        }
        public static string ConvertListOfStringToString(List<string> listString)
        {
            StringBuilder builder = new();
            for (int i = 0; i < listString.Count; i++)
            {
                if (!string.IsNullOrEmpty(listString[i]))
                {
                    builder.Append(listString[i]);
                    if (i != listString.Count - 1)
                    {
                        builder.Append("%2C");
                    }
                }
            }
            return builder.ToString();
        }
    }
}
