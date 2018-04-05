using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers.Integration
{
    [Authorize]
	public class CurriculumGradeController : Controller
	{
		private readonly IACA_CurriculoPeriodoBusiness curriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;

        public CurriculumGradeController(IACA_CurriculoPeriodoBusiness curriculumGradeBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness)
		{
			this.curriculumGradeBusiness = curriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult SearchCurriculumGrade(int cur_id)
		{
			try
			{
				IEnumerable<ACA_CurriculoPeriodo> lista = curriculumGradeBusiness.Load(SessionFacade.UsuarioLogado.Usuario.ent_id, cur_id);

				if (lista != null && lista.Count() > 0)
				{
					var curriculumGradeList = lista.Select(x => new
					{
						Id = x.crp_id,
						Description = x.crp_descricao,
						Ordem = x.crp_ordem,
						Status = false,
						IdBD = 0,
                        TypeCurriculumGradeId = x.tcp_id
					});

					return Json(new { success = true, lista = curriculumGradeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Anos não encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar anos." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult LoadByLevelEducationModality(int LevelEducationId, int Modality)
        {
            try
            {
                IEnumerable<ACA_TipoCurriculoPeriodo> lista = tipoCurriculoPeriodoBusiness.LoadByLevelEducationModality(LevelEducationId, Modality);

                if (lista != null && lista.Count() > 0)
                {
                    var typeCurriculumGradeList = lista.Select(x => new
                    {
                        Id = x.tcp_id,
                        Description = x.tcp_descricao,
                    });
                    return Json(new { success = true, lista = typeCurriculumGradeList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

		#endregion
	}
}