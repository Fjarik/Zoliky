using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ZolikyWeb.Tools
{
	public static class CustomHtmlHelpers
	{
		public static MvcHtmlString NoSelectLaberFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			IDictionary<string, object> htmlAttributes = null)
		{
			if (htmlAttributes == null) {
				htmlAttributes = new Dictionary<string, object>();
			}
			string c = "NoSelect";
			if (htmlAttributes.ContainsKey("class")) {
				htmlAttributes["class"] = htmlAttributes["class"] + $" {c}";
			} else {
				htmlAttributes.Add("class", c);
			}
			var mvcString = htmlHelper.LabelFor(expression, htmlAttributes);

			return mvcString;
		}

		public static MvcHtmlString MaterialTextBoxFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			IDictionary<string, object> htmlAttributes = null)
		{
			return MaterialTextBoxFor(htmlHelper, expression, null, htmlAttributes);
		}

		public static MvcHtmlString MaterialTextBoxFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex,
			string autoComplete,
			IDictionary<string, object> htmlAttributes = null,
			bool includePlaceholder = true)
		{
			htmlAttributes = NormalizeAttributes(expression, tabindex, htmlAttributes, autoComplete, includePlaceholder);
			var mvcString = htmlHelper.TextBoxFor(expression, htmlAttributes);
			return mvcString;
		}

		public static MvcHtmlString MaterialTextBoxFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex = null,
			IDictionary<string, object> htmlAttributes = null)
		{
			htmlAttributes = NormalizeAttributes(expression, tabindex, htmlAttributes);
			var mvcString = htmlHelper.TextBoxFor(expression, htmlAttributes);
			return mvcString;
		}

		public static MvcHtmlString MaterialTextAreaFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex = null,
			IDictionary<string, object> htmlAttributes = null)
		{
			htmlAttributes = NormalizeAttributes(expression, tabindex, htmlAttributes);
			var mvcString = htmlHelper.TextAreaFor(expression, htmlAttributes);
			return mvcString;
		}

		public static MvcHtmlString MaterialPasswordFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex,
			string autoComplete,
			IDictionary<string, object> htmlAttributes = null,
			bool includePlaceholder = true)
		{
			htmlAttributes = NormalizeAttributes(expression, tabindex, htmlAttributes, autoComplete, includePlaceholder);
			var mvcString = htmlHelper.PasswordFor(expression, htmlAttributes);
			return mvcString;
		}

		public static MvcHtmlString MaterialPasswordFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex = null,
			IDictionary<string, object> htmlAttributes = null,
			bool includePlaceholder = true)
		{
			htmlAttributes = NormalizeAttributes(expression, tabindex, htmlAttributes, includePlaceholder: includePlaceholder);
			var mvcString = htmlHelper.PasswordFor(expression, htmlAttributes);
			return mvcString;
		}

		private static IDictionary<string, object> NormalizeAttributes<TModel, TProperty>(
			Expression<Func<TModel, TProperty>> expression,
			int? tabindex = null,
			IDictionary<string, object> htmlAttributes = null,
			string autoComplete = null,
			bool includePlaceholder = true)
		{
			if (htmlAttributes == null) {
				htmlAttributes = new Dictionary<string, object>();
			}
			string c = "form-control";
			if (htmlAttributes.ContainsKey("class")) {
				htmlAttributes["class"] = htmlAttributes["class"] + $" {c}";
			} else {
				htmlAttributes.Add("class", c);
			}

			if (tabindex != null && !htmlAttributes.ContainsKey("tabindex")) {
				htmlAttributes.Add("tabindex", (int) tabindex);
			}

			if (!string.IsNullOrWhiteSpace(autoComplete) && !htmlAttributes.ContainsKey("autocomplete")) {
				htmlAttributes.Add("autocomplete", autoComplete);
			}

			if (!htmlAttributes.ContainsKey("data-toggle")) {
				htmlAttributes.Add("data-toggle", "tooltip");
			}

			if (includePlaceholder && expression.Body is MemberExpression memberExpr) {
				var attribute = memberExpr.Member
										  .GetCustomAttributes(false)
										  .OfType<PlaceHolderAttribute>()
										  .SingleOrDefault();
				if (attribute != null) {
					htmlAttributes.Add("placeholder", attribute.Text);
				}
			}
			return htmlAttributes;
		}
	}
}