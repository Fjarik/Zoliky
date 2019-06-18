using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ZolikyWeb.Tools
{
	public static class SimpleCheckBoxExtension
	{

		public static MvcHtmlString CheckBoxSimple<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression)
		{
			return CheckBoxSimple(htmlHelper, expression, null);
		}

		public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, bool isChecked)
		{
			return CheckBoxSimple(htmlHelper, name, isChecked, null);
		}

		public static MvcHtmlString CheckBoxSimple<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			object htmlAttributes)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

			return CheckBoxSimple(htmlHelper,
				ExpressionHelper.GetExpressionText(expression),
				metadata.Model is bool b && b,
				htmlAttributes);
		}

		public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, bool isChecked,
												   object htmlAttributes)
		{
			string checkBoxWithHidden = htmlHelper.CheckBox(name, isChecked, htmlAttributes).ToHtmlString().Trim();
			string pureCheckBox =
				checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1, StringComparison.Ordinal));
			return new MvcHtmlString(pureCheckBox);
		}
	}
}