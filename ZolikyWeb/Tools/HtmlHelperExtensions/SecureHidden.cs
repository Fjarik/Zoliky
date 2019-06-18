using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace ZolikyWeb.Tools
{
	public static class SecureHiddenExtension
	{
		public static MvcHtmlString SecureHidden(this HtmlHelper htmlHelper, string name)
		{
			return SecureHidden(htmlHelper, name, value: null, htmlAttributes: null);
		}

		public static MvcHtmlString SecureHidden(this HtmlHelper htmlHelper, string name, object value)
		{
			return SecureHidden(htmlHelper, name, value, htmlAttributes: null);
		}

		public static MvcHtmlString SecureHidden(this HtmlHelper htmlHelper, string name, object value,
												 object htmlAttributes)
		{
			return SecureHidden(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString SecureHidden(
			this HtmlHelper htmlHelper,
			string name,
			object value,
			IDictionary<string, object> htmlAttributes)
		{
			return SecureHiddenHelper(htmlHelper, value: value, name: name, htmlAttributes: htmlAttributes);
		}

		public static MvcHtmlString SecureHiddenFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression)
		{
			return SecureHiddenFor(htmlHelper, expression, null);
		}

		public static MvcHtmlString SecureHiddenFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			object htmlAttributes)
		{
			return SecureHiddenFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString SecureHiddenFor<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			IDictionary<string, object> htmlAttributes)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

			return SecureHiddenHelper(htmlHelper, metadata.Model, ExpressionHelper.GetExpressionText(expression),
									  htmlAttributes);
		}

		public static MvcHtmlString DisableIf(this MvcHtmlString instance, Func<bool> expression)
		{
			const string Disabled = "\"disabled\"";

			if (expression.Invoke()) {
				var html = instance.ToString();
				html = html.Insert(html.IndexOf(">", StringComparison.Ordinal), " disabled= " + Disabled);
				return new MvcHtmlString(html);
			}

			return instance;
		}

		private static MvcHtmlString SecureHiddenHelper(
			HtmlHelper htmlHelper,
			object value,
			string name,
			IDictionary<string, object> htmlAttributes)
		{
			var binaryValue = value as Binary;

			if (binaryValue != null) {
				value = binaryValue.ToArray();
			}

			var byteArrayValue = value as byte[];

			if (byteArrayValue != null) {
				value = Convert.ToBase64String(byteArrayValue);
			}

			return InputHelper(htmlHelper, name, value, setId: true, format: null, htmlAttributes: htmlAttributes);
		}

		private static MvcHtmlString InputHelper(
			HtmlHelper htmlHelper,
			string name,
			object value,
			bool setId,
			string format,
			IDictionary<string, object> htmlAttributes)
		{
			var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

			if (string.IsNullOrEmpty(fullName)) {
				throw new ArgumentException("name");
			}

			var inputItemBuilder = new StringBuilder();

			var hiddenInput = new TagBuilder("input");
			hiddenInput.MergeAttributes(htmlAttributes);
			hiddenInput.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
			hiddenInput.MergeAttribute("name", fullName, true);
			hiddenInput.MergeAttribute("value", htmlHelper.FormatValue(value, format));

			var hiddenInputHash = new TagBuilder("input");
			hiddenInputHash.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
			hiddenInputHash.MergeAttribute("name", string.Format("__{0}Token", fullName), true);

			var identity = htmlHelper.ViewContext.HttpContext.User.Identity;

			if (!string.IsNullOrEmpty(identity.Name)) {
				value = string.Format("{0}_{1}", identity.Name, value);
			}

			var encodedValue = Encoding.Unicode.GetBytes(htmlHelper.FormatValue(value, format));

			hiddenInputHash.MergeAttribute(
										   "value",
										   Convert.ToBase64String(MachineKey.Protect(encodedValue,
																					 "Protected Hidden Input Token")));

			if (setId) {
				hiddenInput.GenerateId(fullName);
				hiddenInputHash.GenerateId(string.Format("__{0}Token", fullName));
			}

			inputItemBuilder.Append(hiddenInput.ToString(TagRenderMode.SelfClosing));
			inputItemBuilder.Append(hiddenInputHash.ToString(TagRenderMode.SelfClosing));

			return MvcHtmlString.Create(inputItemBuilder.ToString());
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class ValidateSecureHiddenInputsAttribute : FilterAttribute, IAuthorizationFilter
	{
		private readonly string[] properties;

		public ValidateSecureHiddenInputsAttribute(params string[] properties)
		{
			if (properties == null || !properties.Any()) {
				throw new ArgumentException("properties");
			}

			this.properties = properties;
		}

		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null) {
				throw new ArgumentNullException("filterContext");
			}

			properties.ToList().ForEach(property => Validate(filterContext, property));
		}

		private static void Validate(AuthorizationContext filterContext, string property)
		{
			var protectedValue = filterContext.HttpContext.Request.Form[string.Format("__{0}Token", property)];
			var decodedValue = Convert.FromBase64String(protectedValue);

			var decryptedValue = MachineKey.Unprotect(decodedValue, "Protected Hidden Input Token");

			if (decryptedValue == null) {
				throw new HttpSecureHiddenInputException("A required security token was not supplied or was invalid.");
			}

			protectedValue = Encoding.Unicode.GetString(decryptedValue);

			var originalValue = filterContext.HttpContext.Request.Form[property];

			var identity = filterContext.HttpContext.User.Identity;

			if (!string.IsNullOrEmpty(identity.Name)) {
				originalValue = string.Format("{0}_{1}", identity.Name, originalValue);
			}

			if (!protectedValue.Equals(originalValue)) {
				throw new HttpSecureHiddenInputException("A required security token was not supplied or was invalid.");
			}
		}
	}

	public class HttpSecureHiddenInputException : Exception
	{
		public HttpSecureHiddenInputException(string message) : base(message) { }
	}
}