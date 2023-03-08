using GestaoAvaliacao.App_Start;
using System.Web.Mvc;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using GestaoAvaliacao.Entities;

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

        #region Write

        /// <summary>
        /// Salva um bloco completo. Já interpreta um bloco novo de um atualizado, só acrescentar o Id do bloco na entidade.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(BlockChain entity)
        {
            try
            {
                if (entity.Id > 0)
                {
                    blockChainBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.usu_id,
                        (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                            SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));
                }
                else
                {
                    entity = blockChainBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.usu_id,
                        (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                            SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                entity.Validate.IsValid = false;
                entity.Validate.Message = $"Erro ao {(entity.Id > 0 ? "alterar" : "salvar")} bloco.";
            }

            return Json(
                new
                {
                    success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message,
                    blockid = entity.Id, TestSituation = EnumTestSituation.Pending
                }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método utilizado na remoção de um item unitário de um bloco.
        /// </summary>
        /// <param name="blockChainId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveBlockChainItem(long blockChainId, long itemId)
        {
            var entity = new BlockChain { Id = blockChainId };

            try
            {
                blockChainBusiness.RemoveBlockChainItem(entity.Id, itemId);
                entity.Validate.Message = "Item do bloco excluído com sucesso.";
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                entity.Validate.IsValid = false;
                entity.Validate.Message = "Erro ao tentar excluir o item do bloco.";
            }

            return Json(
                new
                {
                    success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message
                }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método utilizado para remoção dos itens de um bloco
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteBlockItems(long id)
        {
            var entity = new BlockChain();

            try
            {
                entity = blockChainBusiness.DeleteBlockChainItems(id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir os itens.";
                LogFacade.SaveError(ex);
            }

            return Json(
                new
                {
                    success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message
                }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}