using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (SessionFacade.UsuarioLogado.Grupo != null)
            {
                if ((EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Individual && SessionFacade.UsuarioLogado.alu_id > 0)
                {
                    return RedirectToAction("HomeAluno", "ElectronicTest");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult IndexList()
        {
            return View("partials/_indexList");
        }
    }
}