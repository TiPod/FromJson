using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FromJson
{
    public class FromJsonModelBinder : IModelBinder
    {
        public const string ITEM_CACHAE_KEY = "FromJson_Cache";

        public static readonly IDictionary<string, FromJsonAttribute> fromJsonAttrCache = new ConcurrentDictionary<string, FromJsonAttribute>();

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.HttpContext;
            // 不是有效的格式
            ContentType contentType = new ContentType(httpContext.Request.ContentType);
            if (string.Compare(contentType.MediaType, "application/json", true) != 0)
            {
                throw new ApplicationException("ContentType of request should be application/json");
            }
            // 取数据
            JsonElement jsonRoot;
            var itemValue = bindingContext.HttpContext.Items[ITEM_CACHAE_KEY];
            if (itemValue == null)
            {
                //设置编码
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

                try
                {
                    using (JsonDocument document = JsonDocument.Parse(bodyText))
                    {
                        jsonRoot = document.RootElement.Clone();
                    }
                    bindingContext.HttpContext.Items[ITEM_CACHAE_KEY] = jsonRoot;
                }
                catch (JsonException ex)
                {
                    throw new ApplicationException($"Parsing JSON Failed:{ex.Message}");
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
            if (parseJsonValue(jsonRoot, fieldName, bindingContext.ModelType, fromJsonAttr.IgnoreCase, out object jsonValue))
            {
                bindingContext.Result = ModelBindingResult.Success(jsonValue);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        private bool parseJsonValue(JsonElement jsonRoot, string fieldName, Type type, bool ignoreCase, out object jsonValue)
        {
            int firstDotIndex = fieldName.IndexOf('.');
            if (firstDotIndex >= 0)
            {
                string firstPropName = fieldName.Substring(0, firstDotIndex);
                string rest = fieldName.Substring(firstDotIndex + 1);
                if (jsonRoot.TryGetProperty(firstPropName, out JsonElement firstElement))
                {
                    return parseJsonValue(firstElement, rest, type, ignoreCase, out jsonValue);
                }
                else
                {
                    jsonValue = null;
                    return false;
                }
            }
            else
            {
                bool isSuccess = jsonRoot.TryGetProperty(fieldName, ignoreCase, out JsonElement jsonProperty);
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
            using (var ms = new MemoryStream())
            {
                request.EnableBuffering();
                await request.Body.CopyToAsync(ms);
                request.Body.Position = 0;
                return encoding.GetString(ms.ToArray());
            }
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
}