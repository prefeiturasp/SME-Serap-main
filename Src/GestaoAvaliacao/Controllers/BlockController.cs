using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class BlockController : Controller
    {
        private readonly IBlockBusiness blockBusiness;
        private readonly IItemSkillBusiness itemSkillBusiness;
        private readonly IItemBusiness itemBusiness;
        private readonly ICorrelatedSkillBusiness correlatedSkillBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IRequestRevokeBusiness requestRevokeBusiness;

        public BlockController(IBlockBusiness blockBusiness, IItemSkillBusiness itemSkillBusiness, IItemBusiness itemBusiness, ICorrelatedSkillBusiness correlatedSkillBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness, IRequestRevokeBusiness requestRevokeBusiness)
        {
            this.blockBusiness = blockBusiness;
            this.itemSkillBusiness = itemSkillBusiness;
            this.itemBusiness = itemBusiness;
            this.correlatedSkillBusiness = correlatedSkillBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.requestRevokeBusiness = requestRevokeBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexList()
        {
            return View("partials/_indexList");
        }

        public ActionResult IndexForm()
        {
            return View("partials/_indexForm");
        }

        #region Custom

        private IEnumerable<Skill> ReturnSkills(long ItemId)
        {
            IEnumerable<ItemSkill> itemSkills = itemSkillBusiness.GetSkillsByItemId(ItemId);
            if (itemSkills != null && itemSkills.Any())
            {
                return itemSkills.Select(i => new Skill
                {
                    Id = i.Skill.Id,
                    Description = i.Skill.Description,
                    Code = i.Skill.Code
                }).OrderBy(i => i.Id);
            }
            else
                return null;
        }

        #endregion

        #region Read

        /// <summary>
        /// Retorna os blocos já criados na prova pelo Id
        /// </summary>
        /// <param name="TestId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTestBlocks(Int64 Id)
        {
            try
            {
                IEnumerable<Block> blocos = blockBusiness.GetTestBlocks(Id);
                if (blocos != null && blocos.Count() > 0)
                {
                    var retorno = blocos.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        ItensCount = x.BlockItems != null ? x.BlockItems.Count : 0,
                        ItensId = x.BlockItems.Select(i => i.Id),
                        QtdeKnowledgeArea = x.Test.KnowledgeAreaBlock ? x.BlockItems.Select(p => p.KnowledgeArea_Id).Distinct().Count() : 0
                    }).ToList();

                    return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existe(m) bloco(s) de prova criado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao buscar blocos da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os itens já inseridos no bloco pelo Id. Utilizado no gerenciamento de itens.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBlockItens(Int64 Id, int page, int pageItens)
        {
            try
            {
                IEnumerable<Item> blockItens = blockBusiness.GetBlockItens(Id, page, pageItens);

                if (blockItens != null && blockItens.Count() > 0)
                {
                    var retorno = blockItens.Select(x => new
                    {
                        Id = x.Id,
                        Code = x.ItemCode,
                        ItemVersion = x.ItemVersion,
                        Revoked = x.Revoked ?? false,
                        Statement = x.Statement,
                        Periodo = x.ItemCurriculumGrades != null && x.ItemCurriculumGrades.FirstOrDefault() != null ? tipoCurriculoPeriodoBusiness.GetDescription(x.ItemCurriculumGrades.FirstOrDefault().TypeCurriculumGradeId, 0, 0, 0) : " - ",
                        BaseTextId = (x.BaseText != null) ? (int?)x.BaseText.Id : (int?)null,
                        BaseTextDescription = x.BaseText != null ? x.BaseText.Description : null,
                        ItemLevel = x.ItemLevel != null ? new
                        {
                            Description = x.ItemLevel.Description,
                            Value = x.ItemLevel.Value
                        } : null,
                        Skills = ReturnSkills(x.Id),
                        Order = x.BlockItems != null && x.BlockItems.FirstOrDefault(i => i.Item_Id == x.Id) != null ? x.BlockItems.FirstOrDefault(i => i.Item_Id == x.Id).Order : 0,
                        DisciplineDescription = x.EvaluationMatrix.Discipline.Description,
                        KnowledgeArea_Id = x.KnowledgeArea_Id,
                        KnowledgeArea_Description = x.KnowledgeArea_Description,
                        KnowledgeArea_Order = x.KnowledgeArea_Order,
                        ItemCodeVersion = x.ItemCodeVersion
                    }).OrderBy(x => x.KnowledgeArea_Order).ThenBy(x => x.Order).ToList();

                    return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar recuperar os itens do bloco." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna as áreas de conhecimento inseridas no bloco pelo Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBlockKnowledgeAreas(long Id)
        {
            try
            {
                IEnumerable<BlockKnowledgeArea> blockKnowledgeAreas = blockBusiness.GetBlockKnowledgeAreas(Id);
                return Json(new { success = true, lista = blockKnowledgeAreas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar recuperar as áreas de conhecimento do bloco." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna os itens de acordo com a busca realizada. Disciplina sempre é passado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Paginate]
        public JsonResult SearchBlockItem(BlockItemFilter filter)
        {
            try
            {
                if (filter == null)
                    filter = new BlockItemFilter();

                #region Correlated Skill

                if (filter.SkillId != null && filter.SkillLastLevel)
                {
                    List<Skill> skills = correlatedSkillBusiness.LoadCorrelatedSkills(Convert.ToInt64(filter.SkillId));
                    filter.DisciplineId = null;
                    filter.EvaluationMatrixId = null;
                    filter.CorrelatedSkills = string.Format("{0},{1}", filter.SkillId, String.Join(",", skills.Select(s => s.Id)));
                }

                #endregion

                Pager pager = this.GetPager();
                var lista = itemBusiness._BlockSearchItem(filter, ref pager);

                if (lista != null && lista.Count() > 0)
                {
                    var retorno = lista.Select(x => new
                    {
                        Id = x.ItemId,
                        Code = x.ItemCode,
                        ItemVersion = x.ItemVersion,
                        Revoked = x.Revoked ?? false,
                        Statement = x.Statement,
                        Periodo = tipoCurriculoPeriodoBusiness.GetDescription(x.TypeCurriculumGradeId, 0, 0, 0),
                        BaseTextId = x.BaseTextId,
                        BaseTextDescription = x.BaseTextDescription,
                        ItemLevel = new
                        {
                            Description = !string.IsNullOrEmpty(x.ItemLevelDescription) ? x.ItemLevelDescription : string.Empty,
                            Value = x.ItemLevelValue != null ? x.ItemLevelValue : 0
                        },
                        Skills = ReturnSkills(x.ItemId),
                        Order = x.Order != null ? x.Order : 0,
                        DisciplineDescription = x.DisciplineDescription,
                        KnowledgeArea_Id = x.KnowledgeArea_Id
                    }).ToList();

                    var jsonResult = Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Itens não encontrados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar itens pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retorna todos os itens de acordo com o array de itens enviado. É utilizado no avançar da inclusão de itens e no Grafico de desempenho
        /// </summary>
        /// <param name="ItemIds"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetItems(List<long> ItemIds)
        {
            try
            {
                IEnumerable<Item> items = itemBusiness.GetItems(ItemIds);

                if (items != null && items.Count() > 0)
                {
                    var retorno = items.Select(x => new
                    {
                        Id = x.Id,
                        Code = x.ItemCode,
                        ItemLevel = x.ItemLevel,
                        Skills = ReturnSkills(x.Id)
                    }).ToList();

                    return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Problema no retorno dos item(ns) selecionados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro no retorno dos item(ns) selecionados." }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBlocksByItensTests(long? test_id)
        {
            try
            {
                if (test_id.HasValue)
                {
                    List<long> lstTests = new List<long>();
                    lstTests.Add(test_id.Value);

                    IEnumerable<Block> ret = blockBusiness.GetBlocksByItensTests(lstTests);

                    if (ret != null && ret.Count() > 0)
                    {
                        return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foram encontrados dados." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foram encontrados dados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro no retorno dos dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Salva um bloco completo. Já interpreta um bloco novo de um atualizado, só acrescentar o Id do bloco na entidade.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ItemIds"></param>
        /// <param name="TestId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(Block entity)
        {
            try
            {

                if (entity.Id > 0)
                {
                    blockBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.usu_id,
                        (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));
                }
                else
                {
                    entity = blockBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.usu_id,
                        (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                entity.Validate.IsValid = false;
                entity.Validate.Message = string.Format("Erro ao {0} bloco.", entity.Id > 0 ? "alterar" : "salvar");
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, blockid = entity.Id, TestSituation = EnumTestSituation.Pending }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveKnowLedgeAreaOrder(Block entity)
        {
            try
            {
                blockBusiness.SaveKnowLedgeAreaOrder(entity);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                entity.Validate.IsValid = false;
                entity.Validate.Message = string.Format("Erro ao {0} bloco.", entity.Id > 0 ? "alterar" : "salvar");
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, blockid = entity.Id, TestSituation = EnumTestSituation.Pending }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método utilizado na remoção de um item unitário no gerenciamento de itens.
        /// </summary>
        /// <param name="BlockId"></param>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveBlockItem(Int64 BlockId, Int64 ItemId)
        {
            Block entity = new Block { Id = BlockId };

            try
            {
                blockBusiness.RemoveBlockItem(entity.Id, ItemId);
                entity.Validate.Message = "Item do bloco excluído com sucesso.";
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                entity.Validate.IsValid = false;
                entity.Validate.Message = "Erro ao tentar excluir o item do bloco.";
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateItemsRevoked(long BlockItem_Id, long Test_Id, string Justification, long? RequestRevoke_Id, EnumSituation ItemSituation)
        {
            try
            {
                RequestRevoke entity = new RequestRevoke();

                entity = requestRevokeBusiness.UpdateItemsRevoked(BlockItem_Id, Test_Id, Justification, RequestRevoke_Id, ItemSituation, SessionFacade.UsuarioLogado.Usuario.usu_id);

                return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateRequestRevokedByTestBlockItem(long Test_Id, long BlockItem_Id, EnumSituation Situation)
        {
            try
            {
                List<RequestRevoke> entity = new List<RequestRevoke>();
                entity = requestRevokeBusiness.UpdateRequestRevokedByTestBlockItem(Test_Id, BlockItem_Id, Situation);

                return Json(new { success = true, message = EnumExtensions.GetDescription(Situation) },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                LogFacade.SaveError(e);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RevokeItem(long Item_Id, long BlockItem_Id, long Test_Id, bool Revoke, EnumSituation ItemSituation)
        {
            try
            {
                this.UpdateRequestRevokedByTestBlockItem(Test_Id, BlockItem_Id, ItemSituation);

                Item entity = new Item();
                entity = itemBusiness.RevokeItem(Item_Id, Revoke);

                return Json(new { success = true, message = EnumExtensions.GetDescription(ItemSituation) },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult DeleteBlockItems(long Id)
        {
            Block entity = new Block();

            try
            {
                entity = blockBusiness.DeleteBlockItems(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir os itens.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}