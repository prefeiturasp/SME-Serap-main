using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System.Web.Mvc;

namespace GestaoAvaliacao.App_Start
{
    public class ActionAuthorizeAttribute : ActionFilterAttribute
	{
		#region Properties
		public Permission permission { get; set; }

		#endregion

		#region Methods
		public ActionAuthorizeAttribute(Permission permission)
		{
			this.permission = permission;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			if (!(SessionFacade.IsModuleAuthorized(string.Concat(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, '/',
					filterContext.ActionDescriptor.ActionName), this.permission)))
			{
				filterContext.HttpContext.Response.Redirect("/");
			}
		} 
		#endregion
	}
}