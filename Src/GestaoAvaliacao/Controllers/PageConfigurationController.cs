using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Entities;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class PageConfigurationController : Controller
    {
        private readonly IPageConfigurationBusiness pageConfigurationBusiness;

        public PageConfigurationController(IPageConfigurationBusiness pageConfigurationBusiness)
        {
            this.pageConfigurationBusiness = pageConfigurationBusiness;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult Get(int Id)
        {
            PageConfiguration pageConfiguration = pageConfigurationBusiness.Get(Id);
            return Json(new { success = true, pageConfiguration = pageConfiguration }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Find(int Id)
        {
            try
            {
                PageConfiguration pageConfiguration = pageConfigurationBusiness.Find(Id);

                if (pageConfiguration.Id > 0)
                {
                    var ret = new
                    {
                        Id = pageConfiguration.Id,
                        CategoryCombo = new
                        {
                            Id = pageConfiguration.Category,
                            Description = EnumHelper.GetDescriptionFromEnumValue((PageConfigurationCategory)pageConfiguration.Category)
                        },
                        Category = pageConfiguration.Category,
                        Title = pageConfiguration.Title,
                        Description = pageConfiguration.Description,
                        ButtonDescription = pageConfiguration.ButtonDescription,
                        Link = pageConfiguration.Link,
                        FileIllustrativeImage = new
                        {
                            Id = pageConfiguration.FileIllustrativeImage_Id,
                            Path = pageConfiguration.CaminhoIcone
                        },
                        FileVideo = new
                        {
                            Id = pageConfiguration.FileVideo_Id,
                            Path = pageConfiguration.CaminhoVideo
                        },
                        Featured = pageConfiguration.Featured
                    };

                    return Json(new { success = true, pageConfiguration = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Configuração da página não encontrada." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a configuração da página." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(CacheProfile = "Cache1Day")]
        public JsonResult LoadAll()
        {
            try
            {
                IEnumerable<PageConfiguration> pageConfiguration = pageConfigurationBusiness.LoadAll();

                UsuarioLogado user = SessionFacade.UsuarioLogado;
                 if (pageConfigurationBusiness.VerificaPerfilAcessoAdminSerapEstudantes(user.Grupo.gru_id))
                {
                    List<PageConfiguration> pageConfigurationList = pageConfiguration.ToList();
                    var LinkAdminSeraEstudantes = pageConfigurationBusiness.ObterLinkAdminSeraEstudantes();
                    pageConfigurationList.Add(LinkAdminSeraEstudantes);
                    pageConfiguration = pageConfigurationList.AsEnumerable();
                } 

                if (pageConfiguration.Count() > 0)
                {
                    var ret = pageConfiguration.Select(x => new                    
                    {
                        Id = x.Id,
                        CategoryCombo = new
                        {
                            Id = x.Category,
                            Description = EnumHelper.GetDescriptionFromEnumValue((PageConfigurationCategory)x.Category)
                        },
                        Category = x.Category,
                        Title = x.Title,
                        Description = x.Description,
                        ButtonDescription = x.ButtonDescription,
                        Link = x.Link,
                        FileIllustrativeImage = new
                        {
                            Id = x.FileIllustrativeImage_Id,
                            Path = x.CaminhoIcone
                        },
                        FileVideo = new
                        {
                            Id = x.FileVideo_Id,
                            Path = x.CaminhoVideo
                        },
                        Featured = x.Featured
                    });

                    var pageConfigurationTextoPrincipal = ret.Where(p=>p.Category == (short)PageConfigurationCategory.MainText);
                    var pageConfigurationLinkAcessoExternoDestaque = ret.Where(p => p.Category == (short)PageConfigurationCategory.ExternalAccess && p.Featured);
                    var pageConfigurationLinkAcessoExterno = ret.Where(p => p.Category == (short)PageConfigurationCategory.ExternalAccess && !p.Featured);
                    var pageConfigurationFerramentaDestaque = ret.Where(p => p.Category == (short)PageConfigurationCategory.FeaturedTool);
                    var pageConfigurationFerramenta = ret.Where(p => p.Category == (short)PageConfigurationCategory.GeneralTool);
                    var pageConfigurationVideoDestaque = ret.Where(p => p.Category == (short)PageConfigurationCategory.Video && p.Featured).FirstOrDefault();
                    var pageConfigurationVideo = ret.Where(p => p.Category == (short)PageConfigurationCategory.Video);

                    return Json(new { success = true, pageConfigurationTextoPrincipal = pageConfigurationTextoPrincipal, pageConfigurationLinkAcessoExternoDestaque = pageConfigurationLinkAcessoExternoDestaque,
                        pageConfigurationLinkAcessoExterno = pageConfigurationLinkAcessoExterno, pageConfigurationFerramentaDestaque = pageConfigurationFerramentaDestaque, pageConfigurationFerramenta = pageConfigurationFerramenta,
                        pageConfigurationVideoDestaque = pageConfigurationVideoDestaque, pageConfigurationVideo = pageConfigurationVideo
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Configuração da página não encontrada." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a configuração da página." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<PageConfiguration> lista = pageConfigurationBusiness.Load(ref pager);

                if (lista != null && lista.Count() > 0)
                {
                    var pageConfigurationList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        CategoryDescription = EnumHelper.GetDescriptionFromEnumValue((PageConfigurationCategory)x.Category),
                        MostrarDestaque = ((PageConfigurationCategory)x.Category == PageConfigurationCategory.Video || (PageConfigurationCategory)x.Category == PageConfigurationCategory.ExternalAccess),
                        Destaque = x.Featured
                    });

                    return Json(new { success = true, lista = pageConfigurationList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Configuração da página não encontrada." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a configuração da página pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Search(string titulo, string categoria)
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<PageConfiguration> lista = (titulo != null || categoria != null) ? pageConfigurationBusiness.Search(titulo, categoria, ref pager) : pageConfigurationBusiness.Load(ref pager);

                if (lista != null && lista.Count() > 0)
                {
                    var pageConfigurationList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        CategoryDescription = EnumHelper.GetDescriptionFromEnumValue((PageConfigurationCategory)x.Category),
                        MostrarDestaque = ((PageConfigurationCategory)x.Category == PageConfigurationCategory.Video || (PageConfigurationCategory)x.Category == PageConfigurationCategory.ExternalAccess),
                        Destaque = x.Featured
                    });

                    return Json(new { success = true, lista = pageConfigurationList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar configuração da página." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCategoryList()
        {
            try
            {
                var listSituation = new List<PageConfigurationCategory>();

                listSituation.Add(PageConfigurationCategory.MainText);
                listSituation.Add(PageConfigurationCategory.ExternalAccess);
                listSituation.Add(PageConfigurationCategory.FeaturedTool);
                listSituation.Add(PageConfigurationCategory.GeneralTool);
                listSituation.Add(PageConfigurationCategory.Video);

                var ret = listSituation.Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v)
                }).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de categoria não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de categoria." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(PageConfiguration entity)
        {
            try
            {
                if (entity.Id > 0)
                {
                    entity = pageConfigurationBusiness.Update(entity);
                }
                else
                {
                    entity = pageConfigurationBusiness.Save(entity);
                }

                HttpResponse.RemoveOutputCacheItem("/PageConfiguration/LoadAll");
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} a configuração da página.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            PageConfiguration entity = new PageConfiguration();

            try
            {
                entity = pageConfigurationBusiness.Delete(Id);

                HttpResponse.RemoveOutputCacheItem("/PageConfiguration/LoadAll");
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir a configuração da página.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}