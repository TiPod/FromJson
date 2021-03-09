using FromJson;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromJsonAttribute : ModelBinderAttribute
    {
        public string PropertyName { get; private set; }

        public bool PropertyNameCaseInsensitive { get; private set; }


        public FromJsonAttribute(string propertyName = null, bool propertyNameCaseInsensitive = false) : base(typeof(FromJsonModelBinder))
        {
            this.PropertyName = propertyName;
            this.PropertyNameCaseInsensitive = propertyNameCaseInsensitive;
        }

        private BindingSource _bindingSource = BindingSource.Custom;

        public override BindingSource BindingSource { get => _bindingSource; protected set => _bindingSource = value; }

    }
    

}
