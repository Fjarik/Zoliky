using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

namespace API.Tools.Annotations
{
	public class CustomAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
	{
		public HttpResponseMessage GetForbiddenMessage(HttpActionContext context)
		{
			if (context == null) {
				return new HttpRequestMessage().CreateErrorResponse(
																	HttpStatusCode.Forbidden,
																	"Na tuto akci nemáte dostatečné opravnění");
			}
			return context.ControllerContext.Request.CreateErrorResponse(
																		 HttpStatusCode.Forbidden,
																		 "Na tuto akci nemáte dostatečné opravnění");
		}
	}
}