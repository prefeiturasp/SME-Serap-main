using System.Web.Mvc;

namespace GestaoAvaliacao.API
{
    public static class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
