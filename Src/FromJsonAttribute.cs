using FromJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromJsonAttribute : ModelBinderAttribute
    {
        private BindingSource _bindingSource = BindingSource.Custom;

        public FromJsonAttribute(string propertyName = null, bool ignoreCase = false) : base(typeof(FromJsonModelBinder))
        {
            this.PropertyName = propertyName;
            this.IgnoreCase = ignoreCase;
        }

        public override BindingSource BindingSource { get => _bindingSource; protected set => _bindingSource = value; }
        public bool IgnoreCase { get; private set; }
        public string PropertyName { get; private set; }
    }
}