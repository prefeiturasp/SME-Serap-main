using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
	public interface IAdherenceRepository
	{
		IEnumerable<AdherenceGrid> LoadSchoolGrid(Guid ent_id, ref Pager pager, Guid uad_id, int esc_id, bool AllAdhered, long test_id, long testType_id,
			int ttn_id = 0, int crp_ordem = 0, IEnumerable<string> uadGestor = null, Guid? pes_id = null, IEnumerable<string> uadCoordenador = null);
		IEnumerable<Adherence> GetByTest(long test_id, EnumAdherenceEntity typeEntity, IEnumerable<int> idsEntity = null, long ParentId = 0);
		Adherence GetByTest(long test_id, EnumAdherenceEntity typeEntity, long EntityId);

		IEnumerable<AdherenceGrid> LoadSectionGrid(int esc_id, long test_id, long TestType_Id, bool AllAdhered, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null, Guid? ent_id = null);
		IEnumerable<AdherenceGrid> LoadStudent(long tur_id, long test_id, bool AllAdhered, DateTime dataAplicacao);
		IEnumerable<Guid> GetAdherenceStudentsWithDeficiency(IEnumerable<Guid> studentsPesIds, IEnumerable<Guid> deficienciesIds);
		int GetDisponibleSectionTest(int esc_id, long test_id, long TestType_Id, bool AllAdhered, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null, Guid? ent_id = null);
		IEnumerable<AdherenceGrid> LoadOnlySelectedSchool(long test_id, ref Util.Pager pager, bool AllSelected, Guid uad_id, int esc_id, int ttn_id = 0, int crp_ordem = 0, Guid? pes_id = null, Guid? ent_id = null, IEnumerable<string> uadGestor = null, IEnumerable<string> uadCoordenador = null);
		IEnumerable<AdherenceGrid> LoadOnlySelectedSection(long test_id, int esc_id, bool AllSelected, int ttn_id, int crp_ordem, Guid? pes_id = null);
		IEnumerable<AdherenceGrid> LoadSelectedStudent(long tur_id, long test_id, bool AllAdhered, DateTime dataAplicacao);
		void GetTotalByTest(long test_id, bool AllAdhered, out int totalSchool, out int totalSection, EnumSYS_Visao visao, Guid? pes_id = null,
			Guid? ent_id = null, IEnumerable<string> uad_ids = null);

		void RemoveByTest(long test_id);
		void Save(List<Adherence> adherences);
		void Insert(Adherence adherence);

		void RemoveByIds(long test_id, IEnumerable<long> ids, EnumAdherenceEntity entityType);
		void RemoveById(long test_id, long id, EnumAdherenceEntity entityType);

		void Save(long test_id, IEnumerable<long> ids, EnumAdherenceEntity entityType, EnumAdherenceSelection typeSelection, long parentId = 0);
		void Save(long test_id, long ids, EnumAdherenceEntity entityType, EnumAdherenceSelection typeSelection, long parentId = 0);
		IEnumerable<AdherenceDTO> GetSectionsToAnswerSheetLot(long test_id, long TestType_Id, bool AllAdhered, int? esc_id, Guid? uad_id);
		IEnumerable<AdheredEntityDTO> GetAdheredDreSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered);
		IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, Guid uad_id);
		IEnumerable<AdheredEntityDTO> GetAdheredSectionSimple(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, int esc_id);
        IEnumerable<AdheredEntityDTO> GetAdheredSectionBySchool(long TestId, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, bool AllAdhered, int esc_id);

		IEnumerable<AdheredEntityDTO> LoadDreSimpleAdherence(long test_id, Guid ent_id, bool allAdhered, IEnumerable<string> uad_id = null);
		IEnumerable<AdheredEntityDTO> LoadDRESimpleTeacherAdherence(long TestId, Guid ent_id, Guid pes_id, bool allAdhered);
		IEnumerable<AdheredEntityDTO> LoadDRESimpleCoordinatorAdherence(long TestId, Guid ent_id, IEnumerable<string> uad_id, bool allAdhered);

		IEnumerable<AdheredEntityDTO> LoadSchoolSimpleAdherence(long test_id, Guid ent_id, Guid uad_id, bool allAdhered, IEnumerable<string> esc_id = null);
		IEnumerable<AdheredEntityDTO> LoadSchoolSimpleTeacherAdherence(long test_id, Guid ent_id, Guid pes_id, Guid uad_id, bool allAdhered);

		List<TeamsDTO> GetInfoTeams(List<long?> turmas);
        IEnumerable<AdherenceStudentOfSchoolDTO> GetAdherenceStudentsOfSchools(long testId, long testTypeId, IEnumerable<int> schoolIds);
		IEnumerable<AdherenceStudentOfSectionDTO> GetAdherenceStudentsOfSections(IEnumerable<int> sectionIds);

		IEnumerable<TeamsDTO> GetSectionByTestAndTcpId(List<long> test_id, Guid? uad_id, int? esc_id, long? tcp_id,
							Guid? pes_id = null, Guid? ent_id = null, IEnumerable<string> uadGestor = null, IEnumerable<string> uadCoordenador = null);

        IEnumerable<AdheredEntityDTO> GetAdheredDreSimpleReportItem(IEnumerable<long> lstTest, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem);
        IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimpleReportItem(List<long> lstTest, EnumSYS_Visao visao, Guid gru_id, Guid usu_id, Guid pes_id, Guid ent_id, int tne_id, int crp_ordem, Guid uad_id);
    }
}
