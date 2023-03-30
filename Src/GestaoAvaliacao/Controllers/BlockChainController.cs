using GestaoAvaliacao.App_Start;
using System.Web.Mvc;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using GestaoAvaliacao.Entities;
using System.Collections.Generic;
using System.Linq;
using GestaoEscolar.IBusiness;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class BlockChainController : Controller
    {
        private readonly IBlockChainBusiness blockChainBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IItemSkillBusiness itemSkillBusiness;

        public BlockChainController(IBlockChainBusiness blockChainBusiness,
            IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
            IItemSkillBusiness itemSkillBusiness)
        {
            this.blockChainBusiness = blockChainBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.itemSkillBusiness = itemSkillBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Custom

        private IEnumerable<Skill> ReturnSkills(long itemId)
        {
            var itemSkills = itemSkillBusiness.GetSkillsByItemId(itemId);

            if (itemSkills != null && itemSkills.Any())
            {
                return itemSkills.Select(i => new Skill
                {
                    Id = i.Skill.Id,
                    Description = i.Skill.Description,
                    Code = i.Skill.Code
                }).OrderBy(i => i.Id);
            }

            return null;
        }

        #endregion

        #region Read

        /// <summary>
        /// Retorna a cadeia de blocos já criadas na prova pelo Id
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTestBlockChains(long testId)
        {
            try
            {
                var blocos = blockChainBusiness.GetTestBlockChains(testId).ToList();

                if (!blocos.Any())
                {
                    return Json(
                        new
                        {
                            success = false, type = ValidateType.alert.ToString(),
                            message = "Não existe(m) bloco(s) de prova criado(s)."
                        }, JsonRequestBehavior.AllowGet);
                }

                var retorno = blocos.Select(x => new
                {
                    x.Id,
                    x.Description,
                    ItensCount = x.BlockChainItems?.Count ?? 0,
                    ItensId = x.BlockChainItems?.Select(i => i.Id)
                }).ToList();

                return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao buscar blocos da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os cadernos já criadas na prova pelo Id
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCadernosProva(long testId)
        {
            try
            {
                var blocos = blockChainBusiness.ObterCadernosPorProva(testId).ToList();

                if (!blocos.Any())
                {
                    return Json(
                        new
                        {
                            success = false,
                            type = ValidateType.alert.ToString(),
                            message = "Não existe(m) caderno(s) de prova criado(s)."
                        }, JsonRequestBehavior.AllowGet);
                }

                var retorno = blocos.Select(x => new
                {
                    x.Id,
                    x.Description,
                    Test_Id = testId,
                    BlocosCount = x.Blocos.Distinct().Count(),
                    Blocos = x.Blocos.Distinct()
                }).ToList();

                return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao buscar os cadernos da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os itens já inseridos na cadeia de blocos pelo Id.
        /// </summary>
        /// <param name="blockChainId"></param>
        /// <param name="page"></param>
        /// <param name="pageItens"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBlockChainItems(long blockChainId, int page, int pageItens)
        {
            try
            {
                var blockChainItems = blockChainBusiness.GetBlockChainItems(blockChainId, page, pageItens);

                if (blockChainItems == null || !blockChainItems.Any())
                    return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);

                var retorno = blockChainItems.Select(x => new
                {
                    x.Id,
                    Code = x.ItemCode,
                    x.ItemVersion,
                    Revoked = x.Revoked ?? false,
                    x.Statement,
                    Periodo = x.ItemCurriculumGrades?.FirstOrDefault() != null
                        ? tipoCurriculoPeriodoBusiness.GetDescription(x.ItemCurriculumGrades.First().TypeCurriculumGradeId, 0, 0, 0)
                        : " - ",
                    BaseTextId = x.BaseText != null ? (int?)x.BaseText.Id : null,
                    BaseTextDescription = x.BaseText?.Description,
                    ItemLevel = x.ItemLevel != null
                        ? new
                        {
                            x.ItemLevel.Description,
                            x.ItemLevel.Value
                        }
                        : null,
                    Skills = ReturnSkills(x.Id),
                    Order = x.BlockItems?.FirstOrDefault(i => i.Item_Id == x.Id) != null
                        ? x.BlockItems.FirstOrDefault(i => i.Item_Id == x.Id)?.Order
                        : 0,
                    DisciplineDescription = x.EvaluationMatrix.Discipline.Description,
                    x.KnowledgeArea_Id,
                    x.KnowledgeArea_Description,
                    x.KnowledgeArea_Order,
                    x.ItemCodeVersion
                }).OrderBy(x => x.KnowledgeArea_Order).ThenBy(x => x.Order).ToList();

                return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar recuperar os itens do bloco." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

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
                    blockChainId = entity.Id, TestSituation = EnumTestSituation.Pending
                }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método utilizado na remoção de um item unitário de um bloco.
        /// </summary>
        /// <param name="blockChainId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpDelete]
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
        [HttpDelete]
        public JsonResult DeleteBlockChainItems(long id)
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