using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json.Converters
{
    public class ArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(JToken))
                return JToken.Load(reader);

            var result = Activator.CreateInstance(objectType);
            var arr = JArray.Load(reader);
            return ParseObject(arr, result, objectType);
        }

        private static object ParseObject(JArray arr, object result, Type objectType)
        {
            foreach (var property in objectType.GetProperties())
            {
                var attribute =
                    (ArrayPropertyAttribute)property.GetCustomAttribute(typeof(ArrayPropertyAttribute));
                if (attribute == null)
                    continue;

                if (attribute.Index >= arr.Count)
                    continue;

                if (property.PropertyType.BaseType == typeof(Array))
                {
                    var objType = property.PropertyType.GetElementType();
                    var innerArray = (JArray)arr[attribute.Index];
                    var count = 0;
                    if (innerArray.Count == 0)
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 0 });
                        property.SetValue(result, arrayResult);
                    }
                    else if (innerArray[0].Type == JTokenType.Array)
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { innerArray.Count });
                        foreach (var obj in innerArray)
                        {
                            var innerObj = Activator.CreateInstance(objType);
                            arrayResult[count] = ParseObject((JArray)obj, innerObj, objType);
                            count++;
                        }
                        property.SetValue(result, arrayResult);
                    }
                    else
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 1 });
                        var innerObj = Activator.CreateInstance(objType);
                        arrayResult[0] = ParseObject(innerArray, innerObj, objType);
                        property.SetValue(result, arrayResult);
                    }
                    continue;
                }

                var converterAttribute = (JsonConverterAttribute)property.GetCustomAttribute(typeof(JsonConverterAttribute)) ?? (JsonConverterAttribute)property.PropertyType.GetCustomAttribute(typeof(JsonConverterAttribute));
                var value = converterAttribute != null ? arr[attribute.Index].ToObject(property.PropertyType, new JsonSerializer { Converters = { (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType) } }) : arr[attribute.Index];

                if (value != null && property.PropertyType.IsInstanceOfType(value))
                    property.SetValue(result, value);
                else
                {
                    if (value is JToken token)
                        if (token.Type == JTokenType.Null)
                            value = null;

                    if ((property.PropertyType == typeof(decimal)
                     || property.PropertyType == typeof(decimal?))
                     && (value != null && value.ToString().Contains("e")))
                    {
                        if (decimal.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dec))
                            property.SetValue(result, dec);
                    }
                    else
                    {
                        property.SetValue(result, value == null ? null : Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            var type = value.GetType();
            var boolType = typeof(bool);
            var props = type.GetProperties();
            var ordered = props.OrderBy(p => p.GetCustomAttribute<ArrayPropertyAttribute>()?.Index);
            var last = -1;
            foreach (var prop in ordered)
            {
                var arrayProp = prop.GetCustomAttribute<ArrayPropertyAttribute>();
                if (arrayProp == null)
                    continue;

                if (arrayProp.Index == last)
                    continue;

                var shouldSerializeName = "ShouldSerialize" + prop.Name;
                var mi = type.GetMethod(shouldSerializeName);
                if (mi != null && mi.ReturnType == boolType)
                {
                    bool shouldSerialize = (bool)mi.Invoke(value, new object[] { });
                    if (shouldSerialize == false)
                        continue;
                }

                while (arrayProp.Index != last + 1)
                {
                    writer.WriteValue((string)null);
                    last += 1;
                }

                last = arrayProp.Index;
                var converterAttribute = (JsonConverterAttribute)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
                if (converterAttribute != null)
                    writer.WriteRawValue(JsonConvert.SerializeObject(prop.GetValue(value), (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType)));
                else if (!IsSimple(prop.PropertyType))
                    serializer.Serialize(writer, prop.GetValue(value));
                else
                    writer.WriteValue(prop.GetValue(value));
            }
            writer.WriteEndArray();
        }

        private static bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type == typeof(string)
              || type == typeof(decimal);
        }
    }

    public class ArrayPropertyAttribute : Attribute
    {
        public int Index { get; }

        public ArrayPropertyAttribute(int index)
        {
            Index = index;
        }
    }
}