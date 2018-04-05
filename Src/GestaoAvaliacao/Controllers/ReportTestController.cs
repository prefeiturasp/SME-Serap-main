using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Models;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ReportTestController : Controller
	{
		#region Dependences
		readonly ITUR_TurmaTipoCurriculoPeriodoBusiness turmaTipoCurriculoPeriodoBusiness;
		readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
		readonly ITestBusiness testBusiness;
		readonly IAdherenceBusiness adherenceBusiness;

		public ReportTestController(ITUR_TurmaTipoCurriculoPeriodoBusiness turmaTipoCurriculoPeriodoBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
			ITestBusiness testBusiness, IAdherenceBusiness adherenceBusiness)
		{
			this.turmaTipoCurriculoPeriodoBusiness = turmaTipoCurriculoPeriodoBusiness;
			this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
			this.testBusiness = testBusiness;
			this.adherenceBusiness = adherenceBusiness;
		}
		#endregion

        // GET: ReportTest/PerformanceItem
		public ActionResult PerformanceItem()
		{
			return View();
		}

        // GET: ReportTest/PerformanceSkill
		public ActionResult PerformanceSkill()
		{
			return View();
		}

        // GET: ReportTest/PerformanceSchool
		public ActionResult PerformanceSchool()
		{
			return View();
		}

        // GET: ReportTest/PerformanceSchool
        public ActionResult GraphicPerformanceSchool()
        {
            return View();
        }

        #region Filtros
        
        [HttpGet]
		public JsonResult GetYearsBySchool(int esc_id)
		{
			try
			{
				var curriculoPeriodo = turmaTipoCurriculoPeriodoBusiness.GetYearsBySchool(esc_id);

				var retorno = curriculoPeriodo.Select(e => new DropDownReturnModel
				{
					Id = e.ToString(),
					Description = string.Concat(e, "º ANO")
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os anos da escola" }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTestBySchoolYear(int esc_id)
		{
			try
			{
				var curriculoPeriodo = turmaTipoCurriculoPeriodoBusiness.GetYearsBySchool(esc_id);

				var retorno = curriculoPeriodo.Select(e => new DropDownReturnModel
				{
					Id = e.ToString(),
					Description = string.Concat(e, "º ANO")
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os anos da escola" }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetAllYears()
		{
			try
			{
				var curriculoPeriodo = tipoCurriculoPeriodoBusiness.LoadWithNivelEnsino(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

				var retorno = curriculoPeriodo.Select(e => new DropDownReturnModel
				{
					Id = string.Format("{0}_{1}", e.tne_id, e.tcp_ordem.ToString()),
					Description = string.Format("{0} - {1}", e.tcp_descricao, e.tne_nome)
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os anos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTestByYear(string year, DateTime ApplicationStartDate, DateTime ApplicationEndDate)
		{
			try
			{
				var tests = testBusiness.TestByUser(new Entities.TestFilter()
				{
					ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id,
					global = true,
					gru_id = SessionFacade.UsuarioLogado.Grupo.gru_id,
					pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id,
					usuId = SessionFacade.UsuarioLogado.Usuario.usu_id,
					vis_id = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()),
					tne_id_ordem = year,
					ApplicationStartDate = ApplicationStartDate,
					ApplicationEndDate = ApplicationEndDate
				});

				var retorno = tests.Select(e => new DropDownReturnModel
				{
					Id = e.Id.ToString(),
					Description = e.Description
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTestBySchool(int esc_id, DateTime ApplicationStartDate, DateTime ApplicationEndDate)
		{
			try
			{
				var tests = testBusiness.TestByUser(new Entities.TestFilter()
				{
					ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id,
					global = false,
					gru_id = SessionFacade.UsuarioLogado.Grupo.gru_id,
					pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id,
					usuId = SessionFacade.UsuarioLogado.Usuario.usu_id,
					vis_id = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()),
					esc_id = esc_id,
					ApplicationStartDate = ApplicationStartDate,
					ApplicationEndDate = ApplicationEndDate
				});

				var retorno = tests.Select(e => new DropDownReturnModel
				{
					Id = e.Id.ToString(),
					Description = e.Description
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetDResByTest(long TestId, string year)
		{
			try
			{
				var tne_id_crp_orderm = year.Split('_');
				var dres = adherenceBusiness.GetAdheredDreSimple(TestId, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo,
					int.Parse(tne_id_crp_orderm[0]), int.Parse(tne_id_crp_orderm[1]));

				var retorno = dres.Select(e => new DropDownReturnModel
				{
					Id = e.EntityId.ToString(),
					Description = e.Description
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as DREs." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSchoolsByTest(long TestId, string year, Guid uad_id)
		{
			try
			{
				var tne_id_crp_orderm = year.Split('_');
				var dres = adherenceBusiness.GetAdheredSchoolSimple(TestId, uad_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo,
					int.Parse(tne_id_crp_orderm[0]), int.Parse(tne_id_crp_orderm[1]));

				var retorno = dres.Select(e => new DropDownReturnModel
				{
					Id = e.EntityId.ToString(),
					Description = e.Description
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as escolas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSectionsByTest(long TestId, string year, int esc_id)
		{
			try
			{
				var tne_id_crp_orderm = year.Split('_');
				var dres = adherenceBusiness.GetAdheredSectionSimple(TestId, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo,
					int.Parse(tne_id_crp_orderm[0]), int.Parse(tne_id_crp_orderm[1]), esc_id);

				var retorno = dres.Select(e => new DropDownReturnModel
				{
					Id = e.EntityId.ToString(),
					Description = e.Description
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as turmas." }, JsonRequestBehavior.AllowGet);
			}
		}

        #endregion

        #region Métodos mocks (temporários) seguir formatação de objeto de retorno

        [HttpGet]
        public JsonResult GetReportByItemPerformance(long TestId, string Year, Guid? uad_id, int? esc_id, long? tur_id)
        {
            try
            {

                List<ItemReport> itemList = new List<ItemReport>();
                itemList.Add(new ItemReport { Id = 1, Code = 1, Name = "Item 1" });
                itemList.Add(new ItemReport { Id = 2, Code = 2, Name = "Item 2" });
                itemList.Add(new ItemReport { Id = 3, Code = 3, Name = "Item 3" });
                itemList.Add(new ItemReport { Id = 4, Code = 4, Name = "Item 4" });
                itemList.Add(new ItemReport { Id = 5, Code = 5, Name = "Item 5" });

                //mocks
                var grid = new float [5] { 99.25f, 2, 54.23f, 8.30f, 3.34f }; //média da rede
                var dre = new float [5] { 1.25f, 2, 3.23f, 1.32f, 5 }; //média da dre
                List<PerformanceItemReport> schoolList = new List<PerformanceItemReport>();
                schoolList.Add(new PerformanceItemReport { Id = 1, Name = "Ruth 1", Average = new float[5] { 42.25f, 2, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 2, Name = "Ruth 2", Average = new float[5] { 32.25f, 3, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 3, Name = "Ruth 3", Average = new float[5] { 63.25f, 9, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 4, Name = "Ruth 4", Average = new float[5] { 89.25f, 6, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 5, Name = "Ruth 5", Average = new float[5] { 12.25f, 2, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 6, Name = "Ruth 6", Average = new float[5] { 31.25f, 3, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 7, Name = "Ruth 7", Average = new float[5] { 04.25f, 4, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 8, Name = "Ruth 8", Average = new float[5] { 43.25f, 9, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 9, Name = "Ruth 9", Average = new float[5] { 73.25f, 5, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 10, Name = "Ruth 10", Average = new float[5] { 21.25f, 1, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 11, Name = "Ruth 11", Average = new float[5] { 91.25f, 3, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 12, Name = "Ruth 12", Average = new float[5] { 10.25f, 6, 3.23f, 1.32f, 5 } });
                schoolList.Add(new PerformanceItemReport { Id = 14, Name = "Ruth 13", Average = new float[5] { 11.25f, 23, 3.23f, 1.32f, 5 } });

                return Json(new
                {
                    success = true,
                    lista = new
                    {
                        Items = itemList,
                        AverageDre = dre,
                        AverageGrid = grid,
                        Schools = schoolList
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json( new { success = false, type = ValidateType.error.ToString(), message = "Erro gerar relatório por items." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetReportBySchoolPerformance(long TestId, string Year, Guid? uad_id, int? esc_id, long? tur_id)
        {
            try
            {

                List<PerformanceSchoolReport> schoolList = new List<PerformanceSchoolReport>();
                schoolList.Add(new PerformanceSchoolReport { Id = 1, Name = "Ruth 1", Dre = "Dre 1", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 2, Name = "Ruth 2", Dre = "Dre 2", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 3, Name = "Ruth 3", Dre = "Dre 3", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 4, Name = "Ruth 4", Dre = "Dre 4", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 5, Name = "Ruth 5", Dre = "Dre 5", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 6, Name = "Ruth 6", Dre = "Dre 6", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 7, Name = "Ruth 7", Dre = "Dre 7", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 8, Name = "Ruth 8", Dre = "Dre 8", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 9, Name = "Ruth 9", Dre = "Dre 9", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 10, Name = "Ruth 10", Dre = "Dre 10", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 11, Name = "Ruth 11", Dre = "Dre 11", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 12, Name = "Ruth 12", Dre = "Dre 12", AveragePositive = 65f, AverageNegative = 35f });
                schoolList.Add(new PerformanceSchoolReport { Id = 14, Name = "Ruth 13", Dre = "Dre 13", AveragePositive = 65f, AverageNegative = 35f });

                return Json(new
                {
                    success = true,
                    lista = new
                    {
                        AverageDre = 42.25f,
                        AverageGrid = 42.25f,
                        Schools = schoolList
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro gerar relatório por items." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetReportBySkillPerformance(long TestId, string Year, Guid? uad_id, int? esc_id, long? tur_id)
        {
            try
            {

                List<SkillReport> skillList = new List<SkillReport>();
                skillList.Add(new SkillReport { Id = 1, Name = "Ruth 1", Average = 5f, AverageGrid = 65f, AverageDre = 35f, Items = new int[1] {1} });
                skillList.Add(new SkillReport { Id = 2, Name = "Ruth 2", Average = 65f, AverageGrid = 65f, AverageDre = 35f, Items = new int[4] { 3, 1, 4, 45 } });
                skillList.Add(new SkillReport { Id = 3, Name = "Ruth 3", Average = 43f, AverageGrid = 65f, AverageDre = 35f, Items = new int[6] { 1, 2, 3, 4, 5, 6 } });
                skillList.Add(new SkillReport { Id = 4, Name = "Ruth 4", Average = 66f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 5, Name = "Ruth 5", Average = 97f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 6, Name = "Ruth 6", Average = 26f, AverageGrid = 65f, AverageDre = 35f, Items = new int[5] { 8, 24, 32, 46, 72 } });
                skillList.Add(new SkillReport { Id = 7, Name = "Ruth 7", Average = 12f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 8, Name = "Ruth 8", Average = 89f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 9, Name = "Ruth 9", Average = 78f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 10, Name = "Ruth 10", Average = 54f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 11, Name = "Ruth 11", Average = 23f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 12, Name = "Ruth 12", Average = 56f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });
                skillList.Add(new SkillReport { Id = 14, Name = "Ruth 13", Average = 33f, AverageGrid = 65f, AverageDre = 35f, Items = new int[2] { 1, 2 } });

                return Json(new
                {
                    success = true,
                    lista = new
                    {
                        AverageStudents = 42.25f,
                        AverageSkills = 42.25f,
                        Skills = skillList
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro gerar relatório por items." }, JsonRequestBehavior.AllowGet);
            }
        }
        }

        //mock classes
        public class ItemReport
        {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        }

        public class PerformanceItemReport
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public float[] Average { get; set; }
        }

        public class PerformanceSchoolReport
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dre { get; set; }
        public float AveragePositive { get; set; }
        public float AverageNegative { get; set; }
        }

        public class SkillReport
        {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Items { get; set; }
        public float Average { get; set; }
        public float AverageGrid { get; set; }
        public float AverageDre { get; set; }
        }

        #endregion
}
 