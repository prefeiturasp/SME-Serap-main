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
using System.Net;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ItemController : Controller
    {
        private readonly IItemBusiness itemBusiness;
        private readonly IFileBusiness fileBusiness;
        private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IItemSkillBusiness itemSkillBusiness;
        private readonly IItemSituationBusiness itemSituationBusiness;
        private readonly ICorrelatedSkillBusiness correlatedSkillBusiness;
        private readonly IKnowledgeAreaBusiness knowledgeAreaBusiness;
        private readonly IDisciplineBusiness disciplineBusiness;
        private readonly IEvaluationMatrixBusiness evaluationMatrixBusiness;
        private readonly ISubjectBusiness subjectBusiness;
        private readonly IItemFileBusiness itemFilesBusiness;
        private readonly IItemAudioBusiness itemAudioBusiness;
        private readonly IBlockBusiness blockBusiness;        

        public ItemController(IItemBusiness itemBusiness, IFileBusiness fileBusiness, IACA_TipoNivelEnsinoBusiness levelEducationBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
            IItemSkillBusiness itemSkillBusiness, IItemSituationBusiness itemSituationBusiness, ICorrelatedSkillBusiness correlatedSkillBusiness, IKnowledgeAreaBusiness knowledgeAreaBusiness,
            IDisciplineBusiness disciplineBusiness, IEvaluationMatrixBusiness evaluationMatrixBusiness, ISubjectBusiness subjectBusiness, IItemFileBusiness itemFilesBusiness,
            IBlockBusiness blockBusiness, IItemAudioBusiness itemAudioBusiness)
        {
            this.itemBusiness = itemBusiness;
            this.fileBusiness = fileBusiness;
            this.levelEducationBusiness = levelEducationBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.itemSkillBusiness = itemSkillBusiness;
            this.itemSituationBusiness = itemSituationBusiness;
            this.correlatedSkillBusiness = correlatedSkillBusiness;
            this.knowledgeAreaBusiness = knowledgeAreaBusiness;
            this.disciplineBusiness = disciplineBusiness;
            this.evaluationMatrixBusiness = evaluationMatrixBusiness;
            this.subjectBusiness = subjectBusiness;
            this.itemFilesBusiness = itemFilesBusiness;
            this.blockBusiness = blockBusiness;
            this.itemAudioBusiness = itemAudioBusiness;
        }

        [ActionAuthorizeAttribute(Permission.CreateOrUpdate)]
        public ActionResult Form()
        {
            return View();
        }

        [ActionAuthorizeAttribute(Permission.CreateOrUpdate)]
        public ActionResult List()
        {
            return View();
        }

        #region Read

        #region Banco de Itens

        [HttpGet]
        public JsonResult GetBaseTextItems(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetBaseTextBytem(itemId);

                if (entity != null && entity.BaseText != null)
                {
                    var ret = new
                    {
                        Id = entity.BaseText.Id,
                        Description = entity.BaseText.Description,
                        Items = itemBusiness._GetItemsByBaseText(entity.BaseText.Id).Where(x => x.LastVersion).Select(x => x.Id).ToList()
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var ret = new
                    {
                        Id = 0,
                        Description = "O Item não possui texto base.",
                        Items = itemId
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar texto base do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetItemSummaryById(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetItemSummaryById(itemId);

                if (entity != null)
                {
                    var entBase = itemBusiness._GetBaseTextBytem(itemId);
                    var ret = new
                    {
                        Id = entity.Id,
                        ItemCode = entity.ItemCode,
                        ItemVersion = entity.ItemVersion,
                        proficiency = entity.proficiency,
                        Statement = entity.Statement,
                        BaseText = (entBase != null && entBase.BaseText != null && entBase.BaseText.Description != null) ? entBase.BaseText.Description : "O Item não possui texto base.",
                        ItemSkills = entity.ItemSkills.Where(i => i.State == (Byte)EnumState.ativo).Select(its => new
                        {
                            Id = its.Id,
                            OriginalSkill = its.OriginalSkill,
                            Skill = new
                            {
                                Id = its.Skill.Id,
                                Description = its.Skill.Description,
                                LastLevel = its.Skill.LastLevel,
                                Code = its.Skill.Code,
                                Parent = its.Skill.Parent != null ? its.Skill.Parent.Id : 0,
                                ModelSkillLevel = new
                                {
                                    Id = its.Skill.ModelSkillLevel.Id,
                                    Description = its.Skill.ModelSkillLevel.Description,
                                    Level = its.Skill.ModelSkillLevel.Level
                                }
                            }
                        }).ToList(),
                        Alternatives = entity.Alternatives.OrderBy(i => i.Order).Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description,
                            Order = a.Order,
                            Correct = a.Correct,
                            Justificative = a.Justificative,
                            Numeration = a.Numeration,
                            State = a.State
                        }).ToList()
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetItemSummaryByTestItens(long test_id)
        {
            try
            {
                List<long> lstTests = new List<long>();
                lstTests.Add(test_id);

                List<Block> blocks = blockBusiness.GetBlocksByItensTests(lstTests).ToList();
                var blockItem = blocks.FirstOrDefault().BlockItems;
                List<long> lstItens = blockItem.Select(p => p.Item_Id).ToList();
                var lstEntityItens = itemBusiness.GetItemSummaryById(lstTests, lstItens);

                var itemVideos = itemFilesBusiness.GetVideosByLstItemId(lstItens);

                var itemAudios = itemAudioBusiness.GetAudiosByLstItemId(lstItens);

                if (lstEntityItens != null && lstEntityItens.Count() > 0)
                {
                    var ret = lstEntityItens.Select(entity => new
                    {
                        Id = entity.Id,
                        ItemCode = entity.ItemCode,
                        ItemVersion = entity.ItemVersion,
                        proficiency = entity.proficiency,
                        Statement = entity.Statement,
                        ItemOrder = blockItem.FirstOrDefault(p => p.Item_Id == entity.Id).Order + 1,
                        BaseText = entity.BaseText == null ? "<p>O item não possui texto base.</p>" : entity.BaseText.Description,
                        Discipline_id = entity.EvaluationMatrix.Discipline.Id,
                        Discipline_name = entity.EvaluationMatrix.Discipline.Description,
                        ItemSkills = entity.ItemSkills.Where(i => i.State == (Byte)EnumState.ativo).Select(its => new
                        {
                            Id = its.Id,
                            OriginalSkill = its.OriginalSkill,
                            Skill = new
                            {
                                Id = its.Skill.Id,
                                Description = its.Skill.Description,
                                LastLevel = its.Skill.LastLevel,
                                Code = its.Skill.Code,
                                Parent = its.Skill.Parent != null ? its.Skill.Parent.Id : 0
                            }
                        }).ToList(),
                        Alternatives = entity.Alternatives.OrderBy(i => i.Order).Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description,
                            Order = a.Order,
                            Correct = a.Correct,
                            Justificative = a.Justificative,
                            Numeration = a.Numeration,
                            State = a.State
                        }).ToList(),
                        Videos = itemVideos.Where(p => p.Item_Id == entity.Id).ToList(),
                        Audios = itemAudios.Where(p => p.Item_Id == entity.Id).ToList()
                    });

                    var jsonResult = Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetItemSummaryByIdTest(long test_id, long itemId)
        {
            try
            {
                var entity = itemBusiness._GetItemSummaryById(itemId);

                if (entity != null)
                {
                    var entBase = itemBusiness._GetBaseTextBytem(itemId);
                    var ret = new
                    {
                        Id = entity.Id,
                        ItemCode = entity.ItemCode,
                        ItemVersion = entity.ItemVersion,
                        proficiency = entity.proficiency,
                        Statement = entity.Statement,
                        ItemOrder = ((entity.BlockItems.FirstOrDefault(p => p.Item_Id == itemId && p.Block.Test_Id == test_id).Order + 1)),
                        BaseText = (entBase != null && entBase.BaseText != null && entBase.BaseText.Description != null) ? entBase.BaseText.Description : "O Item não possui texto base.",
                        Discipline_id = entity.EvaluationMatrix.Discipline.Id,
                        Discipline_name = entity.EvaluationMatrix.Discipline.Description,
                        ItemSkills = entity.ItemSkills.Where(i => i.State == (Byte)EnumState.ativo).Select(its => new
                        {
                            Id = its.Id,
                            OriginalSkill = its.OriginalSkill,
                            Skill = new
                            {
                                Id = its.Skill.Id,
                                Description = its.Skill.Description,
                                LastLevel = its.Skill.LastLevel,
                                Code = its.Skill.Code,
                                Parent = its.Skill.Parent != null ? its.Skill.Parent.Id : 0,
                                ModelSkillLevel = new
                                {
                                    Id = its.Skill.ModelSkillLevel.Id,
                                    Description = its.Skill.ModelSkillLevel.Description,
                                    Level = its.Skill.ModelSkillLevel.Level
                                }
                            }
                        }).ToList(),
                        Alternatives = entity.Alternatives.OrderBy(i => i.Order).Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description,
                            Order = a.Order,
                            Correct = a.Correct,
                            Justificative = a.Justificative,
                            Numeration = a.Numeration,
                            State = a.State
                        }).ToList(),

                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAddItemInfos(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetAddItemInfos(itemId);

                if (entity != null && entity.BaseText != null && entity.EvaluationMatrix != null)
                {
                    var ret = new
                    {
                        BaseText = new
                        {
                            Id = entity.BaseText.Id,
                            Description = entity.BaseText.Description,
                            Source = entity.BaseText.Source,
                            Files = fileBusiness.GetFilesByOwner(entity.BaseText.Id, itemId, EnumFileType.BaseText)
                        },
                        EvaluationMatrix = new
                        {
                            Id = entity.EvaluationMatrix.Id,
                            Discipline = new
                            {
                                Id = entity.EvaluationMatrix.Discipline.Id
                            }
                        }
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "O Item não possui texto base." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult SearchItems(ItemFilter filter)
        {
            try
            {
                if (filter == null)
                    filter = new ItemFilter();

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
                var lista = itemBusiness._SearchItems(filter, ref pager);

                var ids = lista.Select(i => i.ItemId).ToArray();
                var idsBaseText = lista.Where(i => i.BaseTextId.HasValue).Select(i => i.BaseTextId.Value).Distinct().ToArray();

                var listItemSkill = itemSkillBusiness.GetSkillByItemIds(ids);
                bool? lastVersion = null;
                if (!filter.ShowVersion)
                    lastVersion = true;
                var listBaseText = itemBusiness._GetItemGroupBaseTexts(lastVersion, idsBaseText);

                if (lista != null && lista.Count() > 0)
                {
                    var ret = lista.Select(entity => new
                    {
                        Item = entity,
                        Skills = listItemSkill.Where(e => e.Item.Id.Equals(entity.ItemId)).Select(i => new
                        {
                            Id = i.Skill.Id,
                            Description = i.Skill.Description,
                            Code = i.Skill.Code
                        }).OrderBy(i => i.Id),
                        BaseTextHasItems = entity.BaseTextId != null && (listBaseText.FirstOrDefault(i => i.BaseTextId.Equals(entity.BaseTextId.Value)).Quantidade > 1)
                    }).ToList();

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Itens não encontrados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar itens pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cadastro de Itens

        [HttpGet]
        public JsonResult GetMatrixByItem(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetMatrixBytem(itemId);

                if (entity.EvaluationMatrix == null)
                    return Json(
                        new
                        {
                            success = false, type = ValidateType.alert.ToString(),
                            message = "Matriz não encontrada para o item."
                        }, JsonRequestBehavior.AllowGet);

                var ret = entity.EvaluationMatrix?.Discipline != null ? new
                {
                    entity.EvaluationMatrix.Discipline.Id,
                    entity.EvaluationMatrix.Discipline.Description,
                    EvaluationMatrix = new
                    {
                        entity.EvaluationMatrix.Id,
                        entity.EvaluationMatrix.Description,
                        ModelEvaluationMatrix = entity.EvaluationMatrix.ModelEvaluationMatrix != null ? new
                        {
                            entity.EvaluationMatrix.ModelEvaluationMatrix.Id,
                            entity.EvaluationMatrix.ModelEvaluationMatrix.Description
                        } : null
                    },
                    TypeLevelEducation = levelEducationBusiness.GetCustom(entity.EvaluationMatrix.Discipline.TypeLevelEducationId),
                    entity.ItemNarrated,
                    entity.KnowledgeArea
                } : null;

                return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar matriz do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSimpleMatrixByItem(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetSimpleMatrixBytem(itemId);

                if (entity.EvaluationMatrix != null)
                {
                    var ret = entity.EvaluationMatrix != null && entity.EvaluationMatrix.Discipline != null ? new
                    {
                        Id = entity.EvaluationMatrix.Discipline.Id,
                        Description = entity.EvaluationMatrix.Discipline.Description,
                        EvaluationMatrix = new
                        {
                            Id = entity.EvaluationMatrix.Id,
                            Description = entity.EvaluationMatrix.Description,
                        },
                    } : null;

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz não encontrada para o item." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar matriz do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBaseTextByItem(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetBaseTextBytem(itemId);

                if (entity != null && entity.BaseText != null)
                {
                    var ret = new
                    {
                        Id = entity.BaseText.Id,
                        Description = entity.BaseText.Description,
                        Source = entity.BaseText.Source,
                        BaseTextOrientation = entity.BaseText.BaseTextOrientation,
                        InitialOrientation = entity.BaseText.InitialOrientation,
                        InitialStatement = entity.BaseText.InitialStatement,
                        NarrationInitialStatement = entity.BaseText.NarrationInitialStatement,
                        NarrationStudentBaseText = entity.BaseText.NarrationStudentBaseText,
                        StudentBaseText = entity.BaseText.StudentBaseText,
                        Files = fileBusiness.GetFilesByOwner(entity.BaseText.Id, itemId, EnumFileType.BaseText)
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "O Item não possui texto base." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar texto base do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetItemById(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetItemById(itemId);

                if (entity != null)
                {
                    var ret = new
                    {
                        Id = entity.Id,
                        IsRestrict = entity.IsRestrict,
                        Revoked = entity.Revoked,
                        Statement = new
                        {
                            Description = entity.Statement,
                            Files = fileBusiness.GetFilesByOwner(entity.Id, entity.Id, EnumFileType.Statement)
                        },
                        ItemCode = entity.ItemCode,
                        ItemVersion = entity.ItemVersion,
                        ItemCodeVersion = entity.ItemCodeVersion,
                        Versions = itemBusiness._GetItemVersions(entity.ItemCodeVersion).Select(x => new
                        {
                            codigo = x.ItemCode,
                            versao = x.ItemVersion,
                            aplicado = string.Empty,
                            criacao = x.CreateDate.ToString("dd/MM/yyyy"),
                            provas = x.BlockItems == null ? string.Empty : string.Join(", ", x.BlockItems.Select(p => p.Block.Test.Id))
                        }),
                        Keywords = entity.Keywords,
                        Tips = entity.Tips,
                        TRICasualSetting = entity.TRICasualSetting,
                        TRIDifficulty = entity.TRIDifficulty,
                        TRIDiscrimination = entity.TRIDiscrimination,
                        descriptorSentence = entity.descriptorSentence,
                        proficiency = entity.proficiency,
                        ItemSituation = entity.ItemSituation != null ? new
                        {
                            Id = entity.ItemSituation.Id,
                            Description = entity.ItemSituation.Description
                        } : null,
                        ItemType = entity.ItemType != null ? new
                        {
                            Id = entity.ItemType.Id,
                            Description = entity.ItemType.Description,
                            IsDefault = entity.ItemType.IsDefault,
                            QuantityAlternative = entity.ItemType.QuantityAlternative != null ? entity.ItemType.QuantityAlternative : 0
                        } : null,
                        ItemLevel = entity.ItemLevel != null ? new
                        {
                            Id = entity.ItemLevel.Id,
                            Description = entity.ItemLevel.Description,
                            Value = entity.ItemLevel.Value
                        } : null,
                        ItemSkills = entity.ItemSkills.Where(i => i.State == (Byte)EnumState.ativo).Select(its => new
                        {
                            Id = its.Id,
                            OriginalSkill = its.OriginalSkill,
                            Skill = new
                            {
                                Id = its.Skill.Id,
                                Description = its.Skill.Code + " - " + its.Skill.Description,
                                LastLevel = its.Skill.LastLevel,
                                Parent = its.Skill.Parent != null ? its.Skill.Parent.Id : 0,
                                ModelSkillLevel = new
                                {
                                    Id = its.Skill.ModelSkillLevel.Id,
                                    Description = its.Skill.ModelSkillLevel.Description,
                                    Level = its.Skill.ModelSkillLevel.Level
                                }
                            }
                        }).OrderBy(x => x.Skill.ModelSkillLevel.Level).ToList(),
                        Alternatives = entity.Alternatives.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
                        {
                            Id = a.Id,
                            Description = a.Description,
                            Order = a.Order,
                            Correct = a.Correct,
                            Justificative = new
                            {
                                Description = a.Justificative,
                                Files = fileBusiness.GetFilesByOwner(a.Id, entity.Id, EnumFileType.Justificative)
                            },
                            Numeration = a.Numeration,
                            Files = fileBusiness.GetFilesByOwner(a.Id, entity.Id, EnumFileType.Alternative),
                            TCTBiserialCoefficient = a.TCTBiserialCoefficient,
                            TCTDificulty = a.TCTDificulty,
                            TCTDiscrimination = a.TCTDiscrimination,
                            State = a.State
                        }).OrderBy(i => i.Order).ToList(),
                        ItemCurriculumGrades = entity.ItemCurriculumGrades.Where(i => i.State == (Byte)EnumState.ativo).Select(icg => new
                        {
                            Id = icg.TypeCurriculumGradeId,
                            Description = tipoCurriculoPeriodoBusiness.GetDescription(icg.TypeCurriculumGradeId, 0, 0, 0)
                        }).ToList(),
                        ItemNarrated = entity.ItemNarrated,
                        StudentStatement = entity.StudentStatement,
                        NarrationStudentStatement = entity.NarrationStudentStatement,
                        NarrationAlternatives = entity.NarrationAlternatives,
                        CreateDate = entity.CreateDate,
                        UpdateDate = entity.UpdateDate,
                        State = entity.State,
                        Subsubject = entity.SubSubject,
                        Videos = itemFilesBusiness.GetVideosByItemId(itemId).Select(v => new
                        {
                            File = new
                            {
                                Id = v.FileId,
                                Name = v.Name,
                                Path = v.Path
                            },
                            ConvertedFile = new
                                {
                                    Id = v?.ConvertedFileId,
                                    Name = v?.ConvertedFileName,
                                    Path = v?.ConvertedFilePath
                                },
                            Thumbnail = new
                            {
                                Id = v.ThumbnailId,
                                Name = v.ThumbnailName,
                                Path = v.ThumbnailPath
                            },
                        }).ToList(),
                        Audios = itemAudioBusiness.GetAudiosByItemId(itemId).Select(a => new
                        {
                            File = new
                            {
                                Id = a.FileId,
                                Name = a.Name,
                                Path = a.Path
                            }
                        }).ToList()
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetGradeByItem(long itemId)
        {
            try
            {
                var entity = itemBusiness._GetGradeByItem(itemId);

                if (entity != null)
                {
                    var ret = entity.ItemCurriculumGrades.Where(i => i.State == (Byte)EnumState.ativo).Select(g => new
                    {
                        Id = g.Id,
                        TypeCurriculumGradeId = g.TypeCurriculumGradeId,
                        Description = levelEducationBusiness.Get(g.TypeCurriculumGradeId).tne_nome
                    }).ToList();

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Ano(s) não encontrado." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar ano(s) pesquisado(s)." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ValidateItemCode(string ItemCode, long? itemId)
        {
            try
            {
                bool invalid = false;

                var codeAlreadyExists = itemBusiness.VerifyItemCodeAlreadyExists(ItemCode, itemId);

                if (codeAlreadyExists)
                    invalid = true;

                return Json(new { success = true, invalid = invalid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar pesquisar código." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        [Route("loadallknowledgeareaactive")]
        [HttpGet]
        public JsonResult LoadAllKnowledgeAreaActive(string description)
        {
            try
            {
                return Json(knowledgeAreaBusiness.LoadAllKnowledgeAreaActive(description, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("loaddisciplinebyknowledgearea")]
        [HttpGet]
        public JsonResult LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas)
        {
            try
            {
                return Json(disciplineBusiness.LoadDisciplineByKnowledgeArea(description, knowledgeAreas, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("loadmatrizbydiscipline")]
        [HttpGet]
        public JsonResult LoadMatrizByDiscipline(string description, string discipline)
        {
            try
            {
                return Json(evaluationMatrixBusiness.LoadMatrizByDiscipline(description, discipline, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("loadallsubjects")]
        [HttpGet]
        public JsonResult LoadAllSubjects(string description)
        {
            try
            {
                return Json(subjectBusiness.LoadAllSubjects(description, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("loadsubsubjectbysubject")]
        [HttpGet]
        public JsonResult LoadSubsubjectBySubject(string description, string subjects)
        {
            try
            {
                return Json(subjectBusiness.LoadSubsubjectBySubject(description, subjects, SessionFacade.UsuarioLogado.Usuario.ent_id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetItemVersions(int itemCodeVersion, int itemVersion)
        {
            try
            {
                var lista = itemBusiness.GetItemVersions(itemCodeVersion, itemVersion);
                if (lista != null)
                {
                    var ret = lista.Select(item => new
                    {
                        Id = item.Id,
                        Statement = item.Statement,
                        ItemVersion = item.ItemVersion,
                        ItemCode = item.ItemCode,
                        ItemCodeVersion = item.ItemCodeVersion,
                        BaseText = new
                        {
                            Id = item.BaseText.Id,
                            Description = item.BaseText.Description
                        }
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Versões do item não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Erro ao tentar consultar versões do item." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        #region Cadastro de Itens

        [HttpPost]
        public JsonResult Save(Item item, List<EntityFile> files, List<ItemFile> itemFiles, List<ItemAudio> itemAudios)
        {
            Item entity = new Item();
            object auxItem = null;

            try
            {
                item.ItemFiles = itemFiles;
                item.ItemAudios = itemAudios;

                if (item.Id > 0)
                {
                    item.ItemSituation = item.ItemSituation_Id > 0 ? itemSituationBusiness.GetItemSituationById(item.ItemSituation_Id) : null;

                    if ((files != null && files.Count > 0) || item.ItemSituation == null || !item.ItemSituation.AllowVersion)
                        entity = itemBusiness.Update(item.Id, item, files);
                    else
                        entity = itemBusiness.Update(item.Id, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            entity.Id,
                            entity.IsRestrict,
                            entity.ItemCode,
                            entity.ItemCodeVersion,
                            entity.ItemVersion,
                            entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                a.Id,
                                a.Description,
                                a.Order,
                                a.Correct,
                                numeration = a.Numeration,
                                a.Justificative,
                                a.TCTDiscrimination,
                                a.TCTDificulty,
                                a.TCTBiserialCoefficient,
                                a.State
                            }).ToList(),
                            Versions = itemBusiness._GetItemVersions(entity.ItemCodeVersion).Select(x => new
                            {
                                codigo = x.ItemCode,
                                versao = x.ItemVersion,
                                aplicado = string.Empty,
                                criacao = x.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            }),
                            BaseText_Id = entity.BaseText?.Id ?? 0,
                            entity.ItemNarrated,
                            entity.StudentStatement,
                            entity.NarrationStudentStatement,
                            entity.NarrationAlternatives
                        };
                    }
                }
                else
                {
                    if (files != null && files.Count > 0)
                        entity = itemBusiness.Save(0, item, files);
                    else
                        entity = itemBusiness.Save(0, item);

                    if (entity.Validate.IsValid)
                    {
                        auxItem = new
                        {
                            entity.Id,
                            entity.IsRestrict,
                            entity.ItemCode,
                            entity.ItemVersion,
                            entity.ItemCodeVersion,
                            entity.Statement,
                            Alternatives = entity.Alternatives.Select(a => new
                            {
                                a.Id,
                                a.Description,
                                a.Order,
                                a.Correct,
                                numeration = a.Numeration,
                                a.Justificative,
                                a.TCTDiscrimination,
                                a.TCTDificulty,
                                a.TCTBiserialCoefficient,
                                a.State
                            }).ToList(),
                            Versions = new
                            {
                                codigo = entity.ItemCode,
                                versao = entity.ItemVersion,
                                aplicado = string.Empty,
                                criacao = entity.CreateDate.ToString("dd/MM/yyyy"),
                                provas = string.Empty
                            },
                            BaseText_Id = entity.BaseText_Id ?? 0,
                            entity.ItemNarrated,
                            entity.StudentStatement,
                            entity.NarrationStudentStatement,
                            entity.NarrationAlternatives
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} item.", item.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, Item = auxItem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long Id)
        {
            Item entity = new Item();

            try
            {
                entity = itemBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o item.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveChangeItem(Item item, long test_id, long itemIdAntigo,  long blockId)
        {
            Item entity = new Item();
            try
            {
                entity = itemBusiness.SaveChangeItem(item, test_id, itemIdAntigo, blockId);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao tentar atualizar o item na prova.");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveChangeBlockChainItem(Item item, long testId, long itemIdAntigo, long blockChainId)
        {
            var entity = new Item();
            try
            {
                entity = itemBusiness.SaveChangeBlockChainItem(item, testId, itemIdAntigo, blockChainId);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar atualizar o item na prova.";

                LogFacade.SaveError(ex);
            }

            return Json(
                new
                {
                    success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message
                }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Preview Print
        [HttpGet]
        public ActionResult PreviewPrintItem(long id)
        {
            try
            {
                return new FileContentResult(itemBusiness.GetItemPreview(id, Request.Url.Authority.ToString(), ApplicationFacade.Parameters), "application/pdf");
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Erro ao gerar o documento.");
            }
        }

        [HttpGet]
        public ActionResult PreviewPrintBaseText(long id)
        {
            try
            {
                return new FileContentResult(itemBusiness.GetItemPreviewByBaseText(id, Request.Url.Authority.ToString(), ApplicationFacade.Parameters), "application/pdf");
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Erro ao gerar o documento.");
            }
        }
        #endregion

        #endregion
    }
}