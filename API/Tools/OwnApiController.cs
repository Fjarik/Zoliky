using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using DataAccess.Managers.New.Interfaces;
using SharedLibrary.Interfaces;

namespace API.Tools
{
	public class OwnApiController<TManager> : ApiController where TManager : class, IManager
	{
		protected TManager Mgr => HttpContext.Current.GetOwinContext().Get<TManager>();

		protected TAnotherManager GetManager<TAnotherManager>() where TAnotherManager : class, IManager
		{
			return HttpContext.Current.GetOwinContext().Get<TAnotherManager>();
		}
	}

	public class OwnApiController<TEntity, TManager> : OwnApiController<TManager>
		where TEntity : class, IDbEntity
		where TManager : class, IManager
	{
		[HttpGet]
		[Route("get")]
		public virtual async Task<IHttpActionResult> Get([FromUri] int id)
		{
			var res = await ((Mgr as IManager<TEntity>)?.GetByIdAsync(id));
			return Ok(res);
		}
	}
}