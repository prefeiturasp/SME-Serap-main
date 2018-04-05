using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
	public class ParameterController : Controller
	{
		private readonly IParameterBusiness parameterBusiness;

		public ParameterController(IParameterBusiness parameterBusiness)
		{
			this.parameterBusiness = parameterBusiness;
		}

        public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult GetByKey(string key)
		{
			try
			{
				Parameter parameter = parameterBusiness.GetByKey(key, SessionFacade.UsuarioLogado.Usuario.ent_id);
				return Json(new { success = true, parameter = parameter }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao encontrar o parâmetro pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult GetParametersImage()
		{
			try
			{
				IEnumerable<Parameter> lista = parameterBusiness.GetParametersImage(SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var parametersList = lista.Select(x => new
					{
						Id = x.Id,
						Key = x.Key,
						value = x.Value
					});

					return Json(new { success = true, parameter = parametersList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Parâmetros da imagem não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao encontrar os parâmetros de imagem." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult GetParameters(long Id)
		{
			try
			{
				var lista = parameterBusiness.GetParamsByPage(Id);

				if (lista != null && lista.Count() > 0)
				{
					var ret = lista.GroupBy(k => new { k.ParameterCategory.Id, k.ParameterCategory.Description, k.ParameterPage.pageObligatory, k.ParameterPage.pageVersioning }).Select(g => new
					{
						Id = g.Key.Id,
						Description = g.Key.Description,
						pageVersioning = g.Key.pageVersioning,
						pageObligatory = g.Key.pageObligatory,
						Parameters = lista.Where(par => par.ParameterCategory.Id == g.Key.Id).Select(p => new
						{
							p.Id,
							p.Description,
							p.Versioning,
							p.Obligatory,
							p.Value,
							ParameterType = p.ParameterType.Id,
							p.Key,
                            p.State
						})
					});

					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existem parâmetros para esta página." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar parâmetros." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        [OutputCache(CacheProfile = "Cache1Day")]
        public JsonResult GetParametersUploadFile()
        {
            try
            {
                var paramZipFiles = parameterBusiness.GetParamByKey(EnumParameterKey.ZIP_FILES.GetDescription(), SessionFacade.UsuarioLogado.Usuario.ent_id);
                var zipFiles = paramZipFiles != null ? paramZipFiles.Value.TrimEnd(Constants.StringArraySeparator).Split(Constants.StringArraySeparator).ToList() : null;

                var paramImageFiles = parameterBusiness.GetParamByKey(EnumParameterKey.IMAGE_FILES.GetDescription(), SessionFacade.UsuarioLogado.Usuario.ent_id);
                var imageFiles = paramImageFiles != null ? paramImageFiles.Value.TrimEnd(Constants.StringArraySeparator).Split(Constants.StringArraySeparator).ToList() : null;

                var ret = new List<string>();
                ret.AddRange(zipFiles);
                ret.AddRange(imageFiles);

                return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar parâmetros." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(List<Parameter> entities)
		{
			Parameter entity = new Parameter();

			try
			{
				if (entities.Count > 0)
				{
					entity = parameterBusiness.Update(entities);

					if (entity.Validate.IsValid)
					{
						//Remover cache ao salvar parametro com sucesso
						var urlToRemove = Url.Action("GetParameters", "Parameter");
						Response.RemoveOutputCacheItem(urlToRemove);

						//Remover cache ao salvar parametro com sucesso
						var urlToRemoveImage = Url.Action("GetParametersImage", "Parameter");
						Response.RemoveOutputCacheItem(urlToRemoveImage);

                        var urlToRemoveUploadFile = Url.Action("GetParametersUploadFile", "Parameter");
                        Response.RemoveOutputCacheItem(urlToRemoveUploadFile);

                        ApplicationFacade.Parameters = null;
						ApplicationFacade.LimparGruposEPermissoes = true;
					}
				}
				else
				{
					entity.Validate.IsValid = false;
					entity.Validate.Type = ValidateType.alert.ToString();
					entity.Validate.Message = "Parâmetro(s) não encontrado(s).";
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao alterar o parâmetro.";

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}