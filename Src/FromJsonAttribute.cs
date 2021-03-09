using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromJsonAttribute : ModelBinderAttribute
    {
        public string PropertyName { get; private set; }

        public FromJsonAttribute(string propertyName = null) : base(typeof(FromJsonModelBinder))
        {
            this.PropertyName = propertyName;
        }

        private BindingSource _bindingSource = BindingSource.Custom;

        public override BindingSource BindingSource { get => _bindingSource; protected set => _bindingSource = value; }

    }

    public class FromJsonModelBinder : IModelBinder
    {
        public const string ITEM_CACHAE_KEY = "FromJson_Cache";

        public static readonly IDictionary<string, FromJsonAttribute> fromJsonAttrCache = new ConcurrentDictionary<string, FromJsonAttribute>();
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var httpContext = bindingContext.HttpContext;
            // 不是有效的格式

            var contentTypeStr = httpContext.Request.ContentType;
            if (string.Compare(contentTypeStr, "application/json", true) != 0)
            {
                throw new ApplicationException("ContentType of request should be application/json");
            }

            //设置编码
            ContentType contentType = new ContentType(contentTypeStr);
            var charSet = contentType.CharSet;
            Encoding encoding;
            if (string.IsNullOrWhiteSpace(charSet))
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.GetEncoding(charSet);
            }

            //获取Body内容
            var bodyText = await getBodyAsync(httpContext.Request, encoding);

            if (string.IsNullOrWhiteSpace(bodyText))
            {
                return;
            }


            // 取数据
            JsonElement jsonRoot;
            var itemValue = bindingContext.HttpContext.Items[ITEM_CACHAE_KEY];
            if (itemValue == null)
            {


                try
                {
                    using JsonDocument document = JsonDocument.Parse(bodyText);
                    jsonRoot = document.RootElement.Clone();
                    bindingContext.HttpContext.Items[ITEM_CACHAE_KEY] = jsonRoot;
                }
                catch (JsonException)
                {
                    throw new ApplicationException("Parsing json failed:" + bodyText);
                }
            }
            else
            {
                jsonRoot = (JsonElement)itemValue;
            }

            // 获取字段名称
            string fieldName = bindingContext.FieldName;
            var fromJsonAttr = getFromJsonAttr(bindingContext, fieldName);

            if (!string.IsNullOrWhiteSpace(fromJsonAttr.PropertyName))
            {
                fieldName = fromJsonAttr.PropertyName;
            }
            if (ParseJsonValue(jsonRoot, fieldName, bindingContext.ModelType, out object jsonValue))
            {
                bindingContext.Result = ModelBindingResult.Success(jsonValue);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        private static bool ParseJsonValue(JsonElement jsonObj, string fieldName, Type type, out object jsonValue)
        {
            int firstDotIndex = fieldName.IndexOf('.');
            if (firstDotIndex >= 0)
            {
                string firstPropName = fieldName.Substring(0, firstDotIndex);
                string rest = fieldName.Substring(firstDotIndex + 1);
                if (jsonObj.TryGetProperty(firstPropName, out JsonElement firstElement))
                {
                    return ParseJsonValue(firstElement, rest, type, out jsonValue);
                }
                else
                {
                    jsonValue = null;
                    return false;
                }
            }
            else
            {
                bool isSuccess = jsonObj.TryGetProperty(fieldName, out JsonElement jsonProperty);
                if (isSuccess)
                {
                    jsonValue = jsonProperty.GetValue(type);
                }
                else
                {
                    jsonValue = null;
                }
                return isSuccess;
            }
        }

        private async Task<string> getBodyAsync(HttpRequest request, Encoding encoding)
        {
            using var ms = new MemoryStream();
            request.EnableBuffering();
            await request.Body.CopyToAsync(ms);
            request.Body.Position = 0;

            return encoding.GetString(ms.ToArray());


        }
        private FromJsonAttribute getFromJsonAttr(ModelBindingContext bindingContext, string fieldName)
        {
            var actionDesc = bindingContext.ActionContext.ActionDescriptor;
            string actionId = actionDesc.Id;
            string cacheKey = $"{actionId}:{fieldName}";

            FromJsonAttribute fromJsonAttr;
            if (!fromJsonAttrCache.TryGetValue(cacheKey, out fromJsonAttr))
            {
                var ctrlActionDesc = bindingContext.ActionContext.ActionDescriptor as ControllerActionDescriptor;
                var fieldParameter = ctrlActionDesc.MethodInfo.GetParameters().Single(p => p.Name == fieldName);
                fromJsonAttr = fieldParameter.GetCustomAttributes(typeof(FromJsonAttribute), false).Single() as FromJsonAttribute;
                fromJsonAttrCache[cacheKey] = fromJsonAttr;
            }
            return fromJsonAttr;
        }

    }



    public static class JsonElementExtensions
    {

        public static object GetValue(this JsonElement property, Type conversion)
        {
            switch (property.ValueKind)
            {
                case JsonValueKind.Array:

                    //临时实现
                    Type elementType = null;
                    if (conversion.IsArray)
                    {
                        elementType = conversion.GetElementType();
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
                            elementType = conversion.GetGenericArguments()?.FirstOrDefault();
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
                    return JsonSerializer.Deserialize(property.ToString(), conversion); ;
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
