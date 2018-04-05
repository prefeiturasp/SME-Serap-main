using GestaoAvaliacao.Util;
using System;
using System.Web.Mvc;

namespace GestaoAvaliacao.App_Start
{
    public class PaginateAttribute : ActionFilterAttribute
    {
        #region Public methods

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.PagerActionFilter = GetPager(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var xRecordsCount = ((Pager)filterContext.Controller.ViewBag.PagerActionFilter).RecordsCount.ToString();
            filterContext.HttpContext.Response.Headers.Add("TotalItens", xRecordsCount);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the "pager" filled, according the header.
        /// </summary>
        /// <param name="filterContext">Current context of action.</param>
        /// <returns>Filled "pager".</returns>
        private Pager GetPager(ActionExecutingContext filterContext)
        {
            Pager pager = new Pager();

            var currentPage = filterContext.HttpContext.Request.Headers.Get("CurrentPage");
            pager.CurrentPage = currentPage != null ? Convert.ToInt32(currentPage) : 0;

            var pageSize = filterContext.HttpContext.Request.Headers.Get("PageSize");
            pager.PageSize = pageSize != null ? Convert.ToInt32(pageSize) : pager.PageSize;

            return pager;
        }


        #endregion
    }
}