using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FromJson
{
    public static class JsonElementExtensions
    {
        private class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (!reader.TryGetDateTime(out DateTime value))
                {
                    value = DateTime.Parse(reader.GetString());
                }

                return value;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        public static bool TryGetProperty(this JsonElement property, string propertyName, bool ignoreCase, out JsonElement value)
        {
            value = default;
            if (!ignoreCase)
            {
                return property.TryGetProperty(propertyName, out value);
            }
            else
            {
                foreach (var item in property.EnumerateObject())
                {
                    if (string.Compare(item.Name, propertyName, true) == 0)
                    {
                        value = item.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        public static object GetValue(this JsonElement property, Type conversion)
        {
            switch (property.ValueKind)
            {
                case JsonValueKind.Array:

                    //临时实现
                    if (conversion.IsArray)
                    {
                        var elementType = conversion.GetElementType();
                        Array arr = Array.CreateInstance(elementType, property.GetArrayLength());
                        var index = 0;
                        foreach (var item in property.EnumerateArray())
                        {
                            arr.SetValue(item.GetValue(elementType), index++);
                        }
                        return arr;
                    }
                    else
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(conversion))
                        {
                            var elementType = conversion.GetGenericArguments()?.FirstOrDefault();
                            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                            foreach (var item in property.EnumerateArray())
                                list.Add(item.GetValue(elementType));
                            return list;
                        }
                        else
                        {
                            throw new JsonException("This Is Not A Array or IEnumerable<T>");
                        }
                    }
                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.Number:
                    return changeType(property.GetDecimal(), conversion);

                case JsonValueKind.Object:
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback());
                    return JsonSerializer.Deserialize(property.ToString(), conversion, options); ;
                case JsonValueKind.String:
                    return changeType(property.GetString(), conversion);

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.Undefined:
                    return null;

                default:
                    throw new ArgumentException("Unkown property.ValueKind");
            }
        }

        private static object changeType(object value, Type conversion)
        {
            var t = conversion;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                t = Nullable.GetUnderlyingType(t);
            }
            if (t.IsEnum)
            {
                string s = Convert.ToString(value);
                return Enum.Parse(t, s, true);
            }
            if (t == typeof(Guid))
            {
                string s = Convert.ToString(value);
                return Guid.Parse(s);
            }
            if (!(value is IConvertible))
            {
                return value;
            }

            return Convert.ChangeType(value, t);
        }
    }
}