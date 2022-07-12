using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.FakeData.Converter
{
    public static class EnumToStringConverter
    {
        public static T FromString<T>(string text) where T : struct
        {
            if (Enum.TryParse<T>(text, out T output))
            {
                return output;
            }
            else
            {
                throw new InvalidCastException($"{text} is an invalid value for {typeof(T)}");
            }
            //return result;
        }

        public class StringEnumConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : struct
        {
            public StringEnumConverter() : base(v => v.ToString(), v => FromString<TEnum>(v))
            { }
        }

        public class StringEnumObjectConverter<TEnum> : ValueConverter<EnumObject<TEnum>, string> where TEnum : struct, Enum
        {
            public StringEnumObjectConverter() : base(v => v.ToString(), v => EnumObject<TEnum>.ConvertFromString(v))
            { }
        }

        public class EnumObject<TEnum> where TEnum : struct, Enum
        {
            public EnumObject(string text) : this()
            {
                if (!string.IsNullOrWhiteSpace(text))
                {

                    if (Enum.TryParse(text, out TEnum result))
                        Value = result;
                    else
                    {
                        Value = Enum.Parse<TEnum>("Other");
                        Other = text;
                    }
                }
            }

            public EnumObject()
            {
                if (!Enum.IsDefined(typeof(TEnum), "Other"))
                    throw new ArgumentException("Generic parameter invalid : underlying enum does not contain Other");
            }

            public TEnum? Value { get; protected set; }
            public string Other { get; protected set; }
            public override bool Equals(object obj)
            {
                var valueObject = obj as EnumObject<TEnum>;

                if (valueObject is null)
                    return false;

                return EqualsCore(valueObject);
            }

            public override string ToString()
            {
                return ConvertToString(this);
            }

            public static string ConvertToString(EnumObject<TEnum> obj)
            {
                if (obj.Value.HasValue)
                    return obj.Value.Value.ToString() != "Other" ? obj.Value.Value.ToString() : obj.Other;
                else
                    return string.Empty;
            }

            public static EnumObject<TEnum> ConvertFromString(string text)
                => new EnumObject<TEnum>(text);

            protected virtual bool EqualsCore(EnumObject<TEnum> other)
            {
                if (Value.HasValue && other.Value.HasValue)
                    return (Value.Value.Equals(other.Value.Value)) && (Other == other.Other);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return GetHashCodeCore();
            }

            protected virtual int GetHashCodeCore()
            {
                return (Value, Other).GetHashCode();
            }

            public static bool operator ==(EnumObject<TEnum> a, EnumObject<TEnum> b)
            {
                if ((a is null) && (b is null))
                    return true;

                if ((a is null) || (b is null))
                    return false;

                return a.Equals(b);
            }

            public static bool operator !=(EnumObject<TEnum> a, EnumObject<TEnum> b)
            {
                return !(a == b);
            }
        }
    }
}
