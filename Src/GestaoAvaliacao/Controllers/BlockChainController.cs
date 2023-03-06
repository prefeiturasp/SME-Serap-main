using GestaoAvaliacao.App_Start;
using System.Web.Mvc;
using GestaoAvaliacao.IBusiness;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class BlockChainController : Controller
    {
        private readonly IBlockChainBusiness blockChainBusiness;

        public BlockChainController(IBlockChainBusiness blockChainBusiness)
        {
            this.blockChainBusiness = blockChainBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}