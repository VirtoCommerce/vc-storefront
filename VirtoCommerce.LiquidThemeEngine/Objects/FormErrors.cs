using DotLiquid;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    /// <summary>
    /// array of strings if the form was not submitted successfully. The strings returned depend on which fields of the form were left empty or contained errors. Possible values are:
    /// </summary>
    public class FormErrors : IScriptObject, IEnumerable
    {
        public FormErrors()
        {
            Messages = new Dictionary<string, string>();
        }

        public static FormErrors FromModelState(ModelStateDictionary modelState)
        {
            var result = new FormErrors
            {
                Messages = modelState
              .Where(x => x.Value.Errors.Any())
              .ToDictionary(x => x.Key, x => x.Value.Errors.Select(y => y.ErrorMessage).FirstOrDefault())
            };
            return result;
        }

        public IDictionary<string, string> Messages { get; set; }

        public int Count => Messages.Keys.Count();

        public bool IsReadOnly { get => true; set => throw new System.NotImplementedException(); }


        public IEnumerable<string> GetMembers()
        {
            return Messages.Keys.Concat(new[] { "messages", "size" });
        }

        public bool Contains(string member)
        {
            var result = Messages.ContainsKey(member);
            if (!result)
            {
                result = member.EqualsInvariant("messages") || member.EqualsInvariant("size");
            }
            return result;
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, string member, out object value)
        {
            string strValue;
            var result = Messages.TryGetValue(member, out strValue);
            value = strValue;
            if (!result && member.EqualsInvariant("messages"))
            {
                value = Messages;
            }
            if (!result && member.EqualsInvariant("size"))
            {
                value = Messages.Count();
            }
            return result;
        }

        public bool CanWrite(string member)
        {
            return false;
        }

        public void SetValue(TemplateContext context, SourceSpan span, string member, object value, bool readOnly)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string member)
        {
            throw new System.NotImplementedException();
        }

        public void SetReadOnly(string member, bool readOnly)
        {
            throw new System.NotImplementedException();
        }

        public IScriptObject Clone(bool deep)
        {
            return this;
        }

        public IEnumerator GetEnumerator()
        {
            return Messages.Keys.GetEnumerator();
        }
    }
}
