using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoAvaliacao.Business
{
	public class AdherenceBusiness : IAdherenceBusiness
	{
		private readonly IAdherenceRepository adherenceRepository;
		private readonly ITestRepository testRepository;
		private readonly IESC_EscolaBusiness escolaBusiness;
		private readonly ITUR_TurmaBusiness turmaBusiness;
		private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
		private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;
        private readonly ITestTypeRepository testTypeRepository;
        private readonly ITestTypeDeficiencyRepository testTypeDeficiencyRepository;

        public AdherenceBusiness(IAdherenceRepository adherenceRepository, ITestRepository testRepository, IESC_EscolaBusiness escolaBusiness, 
			ITUR_TurmaBusiness turmaBusiness, ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, 
			ITestCurriculumGradeBusiness testCurriculumGradeBusiness, ITestTypeRepository testTypeRepository, ITestTypeDeficiencyRepository testTypeDeficiencyRepository)
		{
			this.adherenceRepository = adherenceRepository;
			this.testRepository = testRepository;
			this.escolaBusiness = escolaBusiness;
			this.turmaBusiness = turmaBusiness;
			this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
            this.testTypeRepository = testTypeRepository;
            this.testTypeDeficiencyRepository = testTypeDeficiencyRepository;
        }

		#region Read

		public IEnumerable<AdherenceGrid> LoadSchool(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, ref Pager pager, Guid uad_id, int esc_id, int ttn_id, long test_id, int crp_ordem)
		{
			Test test = testRepository.GetObject(test_id);
			IEnumerable<AdherenceGrid> retorno = null;

			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());

			DataTable dt = null;
			IEnumerable<string> uads = null;
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					retorno = adherenceRepository.LoadSchoolGrid(user.ent_id, ref pager, uad_id, esc_id, test.AllAdhered, test.Id, test.TestType_Id, ttn_id, crp_ordem);
					break;
				case EnumSYS_Visao.Gestao:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));

					retorno = adherenceRepository.LoadSchoolGrid(user.ent_id, ref pager, uad_id, esc_id, test.AllAdhered, test.Id, test.TestType_Id, ttn_id, crp_ordem, uadGestor: uads);
					break;
				case EnumSYS_Visao.UnidadeAdministrativa:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));

					retorno = adherenceRepository.LoadSchoolGrid(user.ent_id, ref pager, uad_id, esc_id, test.AllAdhered, test.Id, test.TestType_Id, ttn_id, crp_ordem, uadCoordenador: uads);
					break;
				case EnumSYS_Visao.Individual:
					retorno = adherenceRepository.LoadSchoolGrid(user.ent_id, ref pager, uad_id, esc_id, test.AllAdhered, test.Id, test.TestType_Id, ttn_id, crp_ordem, pes_id: user.pes_id);
					break;
				default:
					break;
			}

			return retorno;
		}

		public IEnumerable<AdherenceGrid> LoadSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, int esc_id, int ttn_id, long test_id, int crp_ordem)
		{
			IEnumerable<AdherenceGrid> retorno = null;

			Test test = testRepository.GetObject(test_id);

			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
				case EnumSYS_Visao.Gestao:
				case EnumSYS_Visao.UnidadeAdministrativa:
					retorno = adherenceRepository.LoadSectionGrid(esc_id, test.Id, test.TestType_Id, test.AllAdhered, ttn_id, crp_ordem);
					break;
				case EnumSYS_Visao.Individual:
					retorno = adherenceRepository.LoadSectionGrid(esc_id, test.Id, test.TestType_Id, test.AllAdhered, ttn_id, crp_ordem, user.pes_id, user.ent_id);
					break;
				default:
					break;
			}


			return retorno;
		}

		public IEnumerable<AdherenceGrid> LoadStudent(long tur_id, long test_id)
		{
			var test = testRepository.GetObject(test_id);
			var targetToStudentsWithDeficiencies = testTypeRepository.GetTestTypeTargetToStudentsWithDeficiencies(test.TestType_Id);
			var deficienciesToFilter = targetToStudentsWithDeficiencies
				? testTypeDeficiencyRepository.GetDeficienciesIds(test.TestType_Id)
				: null;

			return adherenceRepository.LoadStudent(tur_id, test.Id, test.AllAdhered, test.ApplicationStartDate, deficienciesToFilter);
		}

		public IEnumerable<AdherenceGrid> LoadSelectedSchool(ref Pager pager, Guid uad_id, int esc_id, int ttn_id, long test_id, int crp_ordem, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
		{
			var test = testRepository.GetObject(test_id);
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			DataTable dt = null;
			IEnumerable<string> uads = null;
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return adherenceRepository.LoadOnlySelectedSchool(test_id, ref pager, test.AllAdhered, uad_id, esc_id, ttn_id, crp_ordem);
				case EnumSYS_Visao.Gestao:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
					return adherenceRepository.LoadOnlySelectedSchool(test_id, ref pager, test.AllAdhered, uad_id, esc_id, ttn_id, crp_ordem, uadGestor: uads);
				case EnumSYS_Visao.UnidadeAdministrativa:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
					return adherenceRepository.LoadOnlySelectedSchool(test_id, ref pager, test.AllAdhered, uad_id, esc_id, ttn_id, crp_ordem, uadCoordenador: uads);
				case EnumSYS_Visao.Individual:
					return adherenceRepository.LoadOnlySelectedSchool(test_id, ref pager, test.AllAdhered, uad_id, esc_id, ttn_id, crp_ordem,
						user.pes_id, user.ent_id);
				default:
					return null;
			}
		}
		public IEnumerable<TeamsDTO> GetSectionByTestAndTcpId(List<long> test_id, Guid? uad_id, int? esc_id, long? tcp_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			DataTable dt = null;
			IEnumerable<string> uads = null;
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return adherenceRepository.GetSectionByTestAndTcpId(test_id, uad_id, esc_id, tcp_id);
				case EnumSYS_Visao.Gestao:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
					return adherenceRepository.GetSectionByTestAndTcpId(test_id, uad_id, esc_id, tcp_id, uadGestor: uads);
				case EnumSYS_Visao.UnidadeAdministrativa:
					dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
					return adherenceRepository.GetSectionByTestAndTcpId(test_id, uad_id, esc_id, tcp_id, uadCoordenador: uads);
				case EnumSYS_Visao.Individual:
					return adherenceRepository.GetSectionByTestAndTcpId(test_id, uad_id, esc_id, tcp_id,
						user.pes_id, user.ent_id);
				default:
					return null;
			}
		}

		public IEnumerable<AdherenceGrid> LoadSelectedSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, int esc_id, int ttn_id, long test_id, int crp_ordem)
		{
			var test = testRepository.GetObject(test_id);

			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
				case EnumSYS_Visao.Gestao:
				case EnumSYS_Visao.UnidadeAdministrativa:
					return adherenceRepository.LoadOnlySelectedSection(test_id, esc_id, test.AllAdhered, ttn_id, crp_ordem);
				case EnumSYS_Visao.Individual:
					return adherenceRepository.LoadOnlySelectedSection(test_id, esc_id, test.AllAdhered, ttn_id, crp_ordem, user.pes_id);
				default:
					return null;
			}
		}

		public IEnumerable<AdherenceGrid> LoadSelectedStudent(long tur_id, long test_id)
		{
			Test test = testRepository.GetObject(test_id);
			return adherenceRepository.LoadSelectedStudent(tur_id, test.Id, test.AllAdhered, test.ApplicationStartDate);
		}

		public void GetTotalSelected(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, long test_id, out int totalSchool, out int totalSelectedSchool, out int totalSelectedSection)
		{
			totalSchool = escolaBusiness.GetTotalSchool(user, grupo);

			var test = testRepository.GetObject(test_id);
			var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());

			if (visao == EnumSYS_Visao.Administracao)
				adherenceRepository.GetTotalByTest(test_id, test.AllAdhered, out totalSelectedSchool, out totalSelectedSection, visao);
			else if (visao == EnumSYS_Visao.Gestao || visao == EnumSYS_Visao.UnidadeAdministrativa)
			{
				var dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
				var uads = dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
				adherenceRepository.GetTotalByTest(test_id, test.AllAdhered, out totalSelectedSchool, out totalSelectedSection, visao,
					uad_ids: uads);
			}
			else
				adherenceRepository.GetTotalByTest(test_id, test.AllAdhered, out totalSelectedSchool, out totalSelectedSection, visao, pes_id: user.pes_id, ent_id: user.ent_id);

			if (test.AllAdhered)
			{
				int totalSection = turmaBusiness.GetTotalSection(user, grupo);

				totalSelectedSchool = totalSchool - totalSelectedSchool;
				totalSelectedSection = totalSection - totalSelectedSection;
			}
		}

		public Adherence GetByTest(long test_id, EnumAdherenceEntity typeEntity, long EntityId)
		{
			return adherenceRepository.GetByTest(test_id, typeEntity, EntityId);
		}

		public IEnumerable<AdherenceDTO> GetSectionsToAnswerSheetLot(long test_id, long TestType_id, bool AllAdhered, int? esc_id, Guid? uad_id)
		{
			return adherenceRepository.GetSectionsToAnswerSheetLot(test_id, TestType_id, AllAdhered, esc_id, uad_id);
		}

		public IEnumerable<AdheredEntityDTO> GetAdheredDreSimple(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem)
		{
			var test = testRepository.GetObject(TestId);

			var vision = group != null ? (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()) : EnumSYS_Visao.Administracao;
			var gru_id = group != null ? group.gru_id : Guid.Empty;
			var usu_id = user != null ? user.usu_id : Guid.Empty;
			var pes_id = user != null ? user.pes_id : Guid.Empty;
			var ent_id = user != null ? user.ent_id : Guid.Empty;

			return adherenceRepository.GetAdheredDreSimple(TestId, vision, gru_id,
				usu_id, pes_id, ent_id, tne_id, crp_ordem, test.AllAdhered);
		}

        public IEnumerable<AdheredEntityDTO> GetAdheredDreSimpleReportItem(IEnumerable<long> lstTest, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem)
        {
            var vision = group != null ? (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()) : EnumSYS_Visao.Administracao;
            var gru_id = group != null ? group.gru_id : Guid.Empty;
            var usu_id = user != null ? user.usu_id : Guid.Empty;
            var pes_id = user != null ? user.pes_id : Guid.Empty;
            var ent_id = user != null ? user.ent_id : Guid.Empty;

            return adherenceRepository.GetAdheredDreSimpleReportItem(lstTest, vision, gru_id,
                usu_id, pes_id, ent_id, tne_id, crp_ordem);
        }

        public IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimple(long TestId, Guid uad_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem)
		{
			var test = testRepository.GetObject(TestId);
			var vision = group != null ? (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()) : EnumSYS_Visao.Administracao;
			var gru_id = group != null ? group.gru_id : Guid.Empty;
			var usu_id = user != null ? user.usu_id : Guid.Empty;
			var pes_id = user != null ? user.pes_id : Guid.Empty;
			var ent_id = user != null ? user.ent_id : Guid.Empty;

			return adherenceRepository.GetAdheredSchoolSimple(TestId, vision, gru_id, usu_id, pes_id, ent_id, tne_id, crp_ordem, test.AllAdhered, uad_id);
		}
        public IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimpleReportItem(List<long> lstTest, Guid uad_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem)
        {
            var vision = group != null ? (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()) : EnumSYS_Visao.Administracao;
            var gru_id = group != null ? group.gru_id : Guid.Empty;
            var usu_id = user != null ? user.usu_id : Guid.Empty;
            var pes_id = user != null ? user.pes_id : Guid.Empty;
            var ent_id = user != null ? user.ent_id : Guid.Empty;

            return adherenceRepository.GetAdheredSchoolSimpleReportItem(lstTest, vision, gru_id, usu_id, pes_id, ent_id, tne_id, crp_ordem, uad_id);
        }

        public IEnumerable<AdheredEntityDTO> GetAdheredSectionSimple(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem, int esc_id)
		{
			var test = testRepository.GetObject(TestId);
			return adherenceRepository.GetAdheredSectionSimple(TestId, (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()), group.gru_id,
				user.usu_id, user.pes_id, user.ent_id, tne_id, crp_ordem, test.AllAdhered, esc_id);
		}

        public IEnumerable<AdheredEntityDTO> GetAdheredSectionBySchool(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem, int esc_id)
        {
            var test = testRepository.GetObject(TestId);
            return adherenceRepository.GetAdheredSectionBySchool(TestId, (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), group.vis_id.ToString()), group.gru_id,
                user.usu_id, user.pes_id, user.ent_id, tne_id, crp_ordem, test.AllAdhered, esc_id);
        }

        public IEnumerable<AdheredEntityDTO> LoadDreSimpleAdherence(long test_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo)
        {
            EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());
            DataTable dt = null;
            var test = testRepository.GetObject(test_id);

			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
					return adherenceRepository.LoadDreSimpleAdherence(test_id, user.ent_id, test.AllAdhered);
				case EnumSYS_Visao.Gestao:
					dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					if (dt != null && dt.Rows.Count > 0)
					{
						return adherenceRepository.LoadDreSimpleAdherence(test_id, user.ent_id, test.AllAdhered, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")));
					}
					else { break; }
				case EnumSYS_Visao.UnidadeAdministrativa:
					dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					if (dt != null && dt.Rows.Count > 0)
					{
						return adherenceRepository.LoadDRESimpleCoordinatorAdherence(test_id, user.ent_id, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")), test.AllAdhered);
					}
					else { break; }
				case EnumSYS_Visao.Individual:
					return adherenceRepository.LoadDRESimpleTeacherAdherence(test_id, user.ent_id, user.pes_id, test.AllAdhered);
				default:
					break;
			}
			return null;
		}

		public IEnumerable<AdheredEntityDTO> LoadSchoolSimpleAdherence(long test_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, Guid uad_id)
		{
			EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), grupo.vis_id.ToString());

			var test = testRepository.GetObject(test_id);

			switch (visao)
			{
				case EnumSYS_Visao.Administracao:
				case EnumSYS_Visao.Gestao:
					return adherenceRepository.LoadSchoolSimpleAdherence(test_id, user.ent_id, uad_id, test.AllAdhered);
				case EnumSYS_Visao.UnidadeAdministrativa:
					DataTable dt = SYS_UsuarioGrupoUABO.GetSelect(user.usu_id, grupo.gru_id);
					return adherenceRepository.LoadSchoolSimpleAdherence(test_id, user.ent_id, uad_id, test.AllAdhered, dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'")));
				case EnumSYS_Visao.Individual:
					return adherenceRepository.LoadSchoolSimpleTeacherAdherence(test_id, user.ent_id, user.pes_id, uad_id, test.AllAdhered);
				default:
					break;
			}

			return null;
		}

		public List<TeamsDTO> GetInfoTeams(List<long?> turmas)
		{
			return adherenceRepository.GetInfoTeams(turmas);
		}

		#endregion

		#region Write
		public Adherence SwitchAllAdhrered(Guid usuId, long test_id, bool AllAdhered, EnumSYS_Visao vis_id)
		{
			Adherence retorno = new Adherence();
			var test = testRepository.GetObjectWithTestType(test_id);
			retorno.Validate = Validate(test, usuId, vis_id);

			if (retorno.Validate.IsValid)
				this.ValidateCorrectionByTest(test, AllAdhered, ref retorno);


			if (retorno.Validate.IsValid)
			{
				test.AllAdhered = AllAdhered;
				testRepository.SwitchAllAdhrered(test);

				adherenceRepository.RemoveByTest(test_id);

				retorno.Validate.Type = ValidateType.Update.ToString();
				retorno.Validate.Message = "Prova salva com sucesso.";
			}

			return retorno;
		}

		public Adherence Select(long test_id, Guid usuId, long Entityid, EnumAdherenceEntity typeEntity, EnumAdherenceSelection typeSelection, Guid pes_id, Guid ent_id, EnumSYS_Visao vis_id,
			int ttn_id, int year, long parentId)
		{
			Adherence retorno = new Adherence();
			var test = testRepository.GetObjectWithTestType(test_id);

			retorno.Validate = Validate(test, usuId, vis_id);

			if (retorno.Validate.IsValid)
				return this.Adherence(retorno, Entityid, typeEntity, typeSelection, pes_id, ent_id, vis_id, ttn_id, year, test, parentId);

			return retorno;
		}

		public List<Adherence> Select(long test_id, Guid usuId, List<long> EntitiesId, EnumAdherenceSelection typeSelection, Guid pes_id, Guid ent_id, EnumSYS_Visao vis_id,
			int ttn_id, int year)
		{
			List<Adherence> retorno = new List<Adherence>();
			Adherence validation = new Adherence();
			var test = testRepository.GetObjectWithTestType(test_id);

			validation.Validate = Validate(test, usuId, vis_id);

			if (!validation.Validate.IsValid)
			{
				retorno.Add(validation);
				return retorno;
			}

			foreach (var Entityid in EntitiesId)
				retorno.Add(this.Adherence(new Entities.Adherence(), Entityid, EnumAdherenceEntity.School, typeSelection, pes_id, ent_id, vis_id, ttn_id, year, test));

			return retorno;
		}

		public Adherence Insert(Adherence entity)
		{
			adherenceRepository.Insert(entity);
			return entity;
		}

		#endregion

		#region Private Methods

		private Adherence Adherence(Adherence retorno, long Entityid, EnumAdherenceEntity typeEntity, EnumAdherenceSelection typeSelection,
			Guid pes_id, Guid ent_id, EnumSYS_Visao vis_id, int ttn_id, int year, Test test, long parentId = 0)
		{
			if (typeEntity == EnumAdherenceEntity.School)
				this.ValidateCorrectionBySchool(Convert.ToInt32(Entityid), test, typeSelection, ref retorno);
			else if (typeEntity == EnumAdherenceEntity.Section)
				this.ValidateCorrectionBySection(Entityid, test, typeSelection, ref retorno);
			else
				this.ValidateCorrectionBySection(parentId, test, typeSelection, ref retorno);

			if (retorno.Validate.IsValid)
			{
				if (typeEntity == EnumAdherenceEntity.School)
					RulesSchool(test, Convert.ToInt32(Entityid), typeSelection, pes_id, vis_id, ent_id, ttn_id, year);
				else if (typeEntity == EnumAdherenceEntity.Section)
					RulesSection(test, Entityid, typeSelection, pes_id, vis_id, ent_id);
				else
					RulesStudent(test, Entityid, typeSelection, pes_id, vis_id, ent_id, parentId);
			}

			return retorno;
		}
		private void PreparReturn(ref IEnumerable<AdherenceGrid> retorno, EnumAdherenceEntity entityType, long test_id)
		{
			if (retorno != null && retorno.Count() > 0)
			{
				Test test = testRepository.GetObject(test_id);
				var adesao = adherenceRepository.GetByTest(test_id, entityType, retorno.Select(a => a.esc_id));

				foreach (var item in retorno.Where(a => adesao.Any(i => i.EntityId.Equals(a.esc_id))))
					item.TypeSelection = adesao.First(a => a.EntityId.Equals(item.esc_id)).TypeSelection;

				//As escolas que não estão na tabela de aderidas estão com o status que estiver na tabela da prova
				EnumAdherenceSelection selection = test.AllAdhered ? EnumAdherenceSelection.Selected : EnumAdherenceSelection.NotSelected;
				foreach (var item in retorno.Where(a => a.TypeSelection == null))
					item.TypeSelection = selection;
			}
		}

		private Validate Validate(Test test, Guid usuId, EnumSYS_Visao vis_id)
		{
			Validate valid = new Validate();

			if (test == null)
				valid.Message = "Não foi encontrada a prova solicitada.";
			else
			{
				if (!usuId.Equals(test.UsuId) && !test.TestType.Global && vis_id != EnumSYS_Visao.Administracao)
					valid.Message = "Usuário não possui permissão para realizar essa ação.";

				if (DateTime.Today > test.ApplicationEndDate)
					valid.Message = "Não é possível aderir escolas para provas que já foram aplicadas. Por favor, altere o cronograma da prova para uma data futura.";
			}

			if (!string.IsNullOrEmpty(valid.Message))
			{
				valid.IsValid = false;

				if (valid.Code <= 0)
					valid.Code = 400;

				valid.Type = ValidateType.alert.ToString();
			}
			else
				valid.IsValid = true;

			return valid;
		}

		private void ValidateCorrectionBySchool(int esc_id, Test test, EnumAdherenceSelection typeSelection, ref Adherence retorno)
		{
			//Caso for uma remoção de adesão
			if (typeSelection == EnumAdherenceSelection.NotSelected)
			{
				var turmas = testSectionStatusCorrectionBusiness.GetBySchool(test.Id, esc_id);

				//Se alguma turma ja iniciou a correção
				if (turmas != null && turmas.Any(a => a.StatusCorrection != EnumStatusCorrection.Pending))
				{
					retorno.Validate.Message = "Existe(m) turma(s) que já foi iniciada a correção, não pode ser removida a adesão";
					retorno.Validate.IsValid = false;
					retorno.Validate.Type = ValidateType.alert.ToString();
				}
			}
		}

		private void ValidateCorrectionBySection(long tur_id, Test test, EnumAdherenceSelection typeSelection, ref Adherence retorno)
		{
			if (typeSelection == EnumAdherenceSelection.NotSelected || typeSelection == EnumAdherenceSelection.Blocked)
			{
				var turma = testSectionStatusCorrectionBusiness.Get(test.Id, tur_id);

				//Se alguma turma ja iniciou a correção
				if (turma != null && turma.StatusCorrection != EnumStatusCorrection.Pending)
				{
					retorno.Validate.Message = "Existe(m) turma(s) que já foi iniciada a correção, não pode ser removida a adesão";
					retorno.Validate.IsValid = false;
					retorno.Validate.Type = ValidateType.alert.ToString();
				}

			}
		}

		private void ValidateCorrectionByTest(Test test, bool AllAdhered, ref Adherence retorno)
		{
			if (!AllAdhered)
			{
				var turmas = testSectionStatusCorrectionBusiness.GetByTest(test.Id);

				//Se alguma turma ja iniciou a correção
				if (turmas != null && turmas.Any(a => a.StatusCorrection != EnumStatusCorrection.Pending))
				{
					retorno.Validate.Message = "Existe(m) turma(s) que já foi iniciada a correção, não pode ser removida a adesão";
					retorno.Validate.IsValid = false;
					retorno.Validate.Type = ValidateType.alert.ToString();
				}

			}
		}

		/*
		 * Regra: Tem que salvar sempre o status oposto ao que está na prova (caso esteja "Todas as escolas da rede" selecionado, salvar apenas as que não estão selecionadas...
		 *	Ao marcar uma escola, tem que marcar todas as turmas daquela escola
		 *	Ao desmarcar, tem que desmarcar todas as turmas
		 */
		private void RulesSchool(Test test, int EntityId, EnumAdherenceSelection typeSelection, Guid pes_id, EnumSYS_Visao vis_id, Guid ent_id, int ttn_id, int year)
		{
			var years = testCurriculumGradeBusiness.GetSimple(test.Id, EntityId);

			if (year > 0)
				years = years.Where(t => t.crp_ordem == year);

			var turmas = turmaBusiness.LoadByGrade(EntityId, ttn_id, ent_id, pes_id, vis_id, years.Select(t => t.tcp_id.HasValue ? t.tcp_id.Value : 0)).Select(i => i.tur_id);

			//Se for todos selecionados na prova e for checado para selecionado a escola, remover todos do banco
			if ((test.AllAdhered && typeSelection == EnumAdherenceSelection.Selected) || (!test.AllAdhered && typeSelection == EnumAdherenceSelection.NotSelected))
			{
				adherenceRepository.RemoveById(test.Id, Convert.ToInt64(EntityId), EnumAdherenceEntity.School);
				adherenceRepository.RemoveByIds(test.Id, turmas, EnumAdherenceEntity.Section);

				foreach (long turma in turmas)
				{
					var alunos = adherenceRepository.LoadStudent(turma, test.Id, test.AllAdhered, test.ApplicationStartDate).Where(p => p.TypeSelection != EnumAdherenceSelection.Blocked).Select(q => q.alu_id);
					adherenceRepository.RemoveByIds(test.Id, alunos, EnumAdherenceEntity.Student);
				}
			}
			else
			{
				adherenceRepository.Save(test.Id, Convert.ToInt64(EntityId), EnumAdherenceEntity.School, typeSelection);
				adherenceRepository.Save(test.Id, turmas, EnumAdherenceEntity.Section, typeSelection, EntityId);
				foreach (long turma in turmas)
				{
					var alunos = adherenceRepository.LoadStudent(turma, test.Id, test.AllAdhered, test.ApplicationStartDate).Where(p => p.TypeSelection != EnumAdherenceSelection.Blocked).Select(q => q.alu_id);
					adherenceRepository.Save(test.Id, alunos, EnumAdherenceEntity.Student, typeSelection, turma);
				}
			}
		}

		/*
		 * Regra: Tem que salvar sempre o status oposto ao que está na prova (caso esteja "Todas as escolas da rede" selecionado, salvar apenas as que não estão selecionadas...
		 *	Ao marcar uma turma:
		 *	1 - verificar se todas as turmas daquela escola foram selecionadas e marcar como selecionada a escola
		 *	2 - caso não tenha sido marcada todas as turmas, marcar a escola como parcialmente selecionada
         *	3 - coloca todos os alunos da turma como marcados
		 *	Ao desmarcar uma turma
		 *	1 - verificar se todas as turmas daquela escola foram desmarcadas e desmarcar a escola
		 *	2 - caso não tenha sido desmarcada todas as turmas, marcar a escola como parcialmente selecionada
         *	3 - coloca todos os alunos da turma como desmarcados.
		 */
		private void RulesSection(Test test, long EntityId, EnumAdherenceSelection typeSelection, Guid pes_id, EnumSYS_Visao vis_id, Guid ent_id)
		{
			var turma = turmaBusiness.Get(EntityId);

			var todasTurmas = vis_id == EnumSYS_Visao.UnidadeAdministrativa ?
				adherenceRepository.GetDisponibleSectionTest(turma.esc_id, test.Id, test.TestType_Id, test.AllAdhered, 0, 0, pes_id, ent_id) :
				adherenceRepository.GetDisponibleSectionTest(turma.esc_id, test.Id, test.TestType_Id, test.AllAdhered, 0, 0);

			var turmaSelecionadas = adherenceRepository.GetByTest(test.Id, EnumAdherenceEntity.Section, ParentId: turma.esc_id);

			var alunos = adherenceRepository.LoadStudent(EntityId, test.Id, test.AllAdhered, test.ApplicationStartDate).Where(p => p.TypeSelection != EnumAdherenceSelection.Blocked).Select(q => q.alu_id);

			if ((test.AllAdhered && typeSelection == EnumAdherenceSelection.Selected) || (!test.AllAdhered && typeSelection == EnumAdherenceSelection.NotSelected))
			{
				adherenceRepository.RemoveById(test.Id, EntityId, EnumAdherenceEntity.Section);
				if (!turmaSelecionadas.Any(t => t.EntityId != turma.tur_id))
					adherenceRepository.RemoveById(test.Id, turma.esc_id, EnumAdherenceEntity.School);
				else
					adherenceRepository.Save(test.Id, turma.esc_id, EnumAdherenceEntity.School, EnumAdherenceSelection.Partial);

				adherenceRepository.RemoveByIds(test.Id, alunos, EnumAdherenceEntity.Student);
			}
			else
			{
				adherenceRepository.Save(test.Id, EntityId, EnumAdherenceEntity.Section, typeSelection, turma.esc_id);
				adherenceRepository.Save(test.Id, turma.esc_id, EnumAdherenceEntity.School,
					(todasTurmas != (turmaSelecionadas.Count() + 1) ? EnumAdherenceSelection.Partial : typeSelection));

				adherenceRepository.Save(test.Id, alunos, EnumAdherenceEntity.Student, typeSelection, EntityId);
			}
		}

		/*
		 * Regra: Tem que salvar sempre o status oposto ao que está na prova (caso esteja "Todas as escolas da rede" selecionado, salvar apenas as que não estão selecionadas...
		 *	Ao marcar um aluno:
		 *	1 - verificar se todos os alunos daquela turma foram selecionadas e marcar como selecionada a turma
		 *	2 - caso não tenha sido marcada todas os alunos, marcar a turma como parcialmente selecionada
		 *	Ao desmarcar um aluno:
		 *	1 - verificar se todas os alunos daquela escola foram desmarcadas e desmarcar a turma
		 *	1 - caso não tenha sido desmarcada todos os alunos, marcar a turma como parcialmente selecionada
		 */
		private void RulesStudent(Test test, long EntityId, EnumAdherenceSelection typeSelection, Guid pes_id, EnumSYS_Visao vis_id, Guid ent_id, long parentId)
		{
			EnumAdherenceSelection typeSelectionParent;
			var todosAlunos = adherenceRepository.LoadStudent(parentId, test.Id, test.AllAdhered, test.ApplicationStartDate);
			var alunosSelecionados = todosAlunos.ToList().FindAll(p => p.existAdherence);

			var turma = turmaBusiness.Get(parentId);

			var todasTurmas = vis_id == EnumSYS_Visao.UnidadeAdministrativa ?
				adherenceRepository.GetDisponibleSectionTest(turma.esc_id, test.Id, test.TestType_Id, test.AllAdhered, 0, 0, pes_id, ent_id) :
				adherenceRepository.GetDisponibleSectionTest(turma.esc_id, test.Id, test.TestType_Id, test.AllAdhered, 0, 0);

			var turmaSelecionadas = adherenceRepository.GetByTest(test.Id, EnumAdherenceEntity.Section, ParentId: turma.esc_id);


			if ((test.AllAdhered && typeSelection == EnumAdherenceSelection.Selected) || (!test.AllAdhered && typeSelection == EnumAdherenceSelection.NotSelected))
			{
				adherenceRepository.RemoveById(test.Id, EntityId, EnumAdherenceEntity.Student);
				if (!alunosSelecionados.Any(t => t.alu_id != EntityId))
				{
					typeSelectionParent = EnumAdherenceSelection.NotSelected;
					adherenceRepository.RemoveById(test.Id, parentId, EnumAdherenceEntity.Section);

					if (!turmaSelecionadas.Any(t => t.EntityId != turma.tur_id))
						adherenceRepository.RemoveById(test.Id, turma.esc_id, EnumAdherenceEntity.School);
					else
						adherenceRepository.Save(test.Id, turma.esc_id, EnumAdherenceEntity.School, EnumAdherenceSelection.Partial);
				}
				else
				{
					typeSelectionParent = EnumAdherenceSelection.Partial;
					adherenceRepository.Save(test.Id, parentId, EnumAdherenceEntity.Section, typeSelectionParent);
					adherenceRepository.Save(test.Id, turma.esc_id, EnumAdherenceEntity.School, typeSelectionParent);
				}
			}
			else
			{
				alunosSelecionados = alunosSelecionados.FindAll(p => p.TypeSelection == EnumAdherenceSelection.Selected);

				typeSelectionParent = (todosAlunos.Count() != (alunosSelecionados.Count + 1) ? EnumAdherenceSelection.Partial : (typeSelection == EnumAdherenceSelection.Blocked ? EnumAdherenceSelection.NotSelected : typeSelection));
				adherenceRepository.Save(test.Id, EntityId, EnumAdherenceEntity.Student, typeSelection, parentId);
				adherenceRepository.Save(test.Id, parentId, EnumAdherenceEntity.Section, typeSelectionParent);
				adherenceRepository.Save(test.Id, turma.esc_id, EnumAdherenceEntity.School, (todasTurmas != (turmaSelecionadas.Count() + 1) ? EnumAdherenceSelection.Partial : typeSelectionParent));

			}
		}

		#endregion

	}
}
