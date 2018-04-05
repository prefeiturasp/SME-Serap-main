using GestaoAvaliacao.Util;
using System.Web.Mvc;

namespace GestaoAvaliacao.App_Start
{
    public static class ControllerExtension
    {
        public static Pager GetPager(this Controller controller)
        {
            return (Pager)controller.ViewBag.PagerActionFilter;
        }
    }
}