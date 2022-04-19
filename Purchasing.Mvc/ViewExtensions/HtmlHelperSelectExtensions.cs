using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    public static class HtmlHelperSelectExtensions
    {
        public static IHtmlContent Select(this IHtmlHelper helper, string expression, Action<SelectOptions> setOptions)
        {
            var selectOptions = new SelectOptions(expression);
            setOptions(selectOptions);

            return selectOptions.GetDropDownItem(helper);
        }
    }

    public class SelectOptions
    {
        public SelectOptions(string expression)
        {
            _expression = expression;
        }

        private string _expression;

        private List<SelectListItem> _items = new List<SelectListItem>();

        private string _defaultOption = null;

        private Dictionary<string, string> _htmlAttributes = new Dictionary<string, string>();

        internal IHtmlContent GetDropDownItem(IHtmlHelper helper)
        {
            var html = helper.DropDownList(_expression, _items, _defaultOption, _htmlAttributes);
            return html;
        }

        public SelectOptions Options<T>(IEnumerable<T> items, Func<T, string> valueSelector, Func<T, string> textSelector)
        {
            _items.AddRange(items.Select(item => new SelectListItem
            {
                Value = valueSelector(item),
                Text = textSelector(item)
            }));
            return this;
        }

        public SelectOptions Options(IEnumerable<string> items)
        {
            _items.AddRange(items.Select(item => new SelectListItem
            {
                Value = item,
                Text = item
            }));
            return this;
        }

        public SelectOptions Options(IEnumerable<SelectListItem> items)
        {
            _items.AddRange(items);
            return this;
        }

        public SelectOptions FirstOption(string text)
        {
            _defaultOption = text;
            return this;
        }

        public SelectOptions Selected(string value)
        {
            value ??= "";
            _items.ForEach(item => item.Selected = item.Value == value);
            return this;
        }

        public SelectOptions Class(string cssClass)
        {
            _htmlAttributes["class"] = cssClass;
            return this;
        }

        public SelectOptions Disabled(bool disabled) {
            _htmlAttributes["disabled"] = disabled.ToString().ToLower();
            return this;
        }
        
        public SelectOptions Title(string title)
        {
            _htmlAttributes["title"] = title;
            return this;
        }

        public SelectOptions Styles(params Func<string, string>[] values)
		{
			var sb = new StringBuilder();
			foreach (var func in values)
			{
				sb.AppendFormat("{0}:{1};", func.Method.GetParameters()[0].Name.Replace('_', '-'), func(null));
			}
			_htmlAttributes["style"] = sb.ToString();
			return this;
		}
    }
}

