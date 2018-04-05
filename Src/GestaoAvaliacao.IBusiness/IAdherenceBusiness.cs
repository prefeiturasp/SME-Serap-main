using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using CoreSSO = MSTech.CoreSSO.Entities;

namespace GestaoAvaliacao.IBusiness
{
	public interface IAdherenceBusiness
	{
		IEnumerable<AdherenceGrid> LoadSchool(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, ref Pager pager, Guid uad_id, int esc_id, int ttn_id, long test_id, int crp_ordem);
		IEnumerable<AdherenceGrid> LoadSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, int esc_id, int ttn_id, long test_id, int crp_ordem);
		IEnumerable<AdherenceGrid> LoadStudent(long tur_id, long test_id);
		IEnumerable<AdherenceGrid> LoadSelectedStudent(long tur_id, long test_id);
		void GetTotalSelected(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, long test_id, out int totalSchool, out int totalSelectedSchool, out int totalSelectedSection);
		Adherence SwitchAllAdhrered(Guid usuId, long test_id, bool AllAdhered, EnumSYS_Visao vis_id);
		IEnumerable<AdherenceGrid> LoadSelectedSchool(ref Pager pager, Guid uad_id, int esc_id, int ttn_id, long test_id, int crp_ordem, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
		IEnumerable<AdherenceGrid> LoadSelectedSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, int esc_id, int ttn_id, long test_id, int crp_ordem);
		Adherence Select(long test_id, Guid usuId, long Entityid, EnumAdherenceEntity typeEntity, EnumAdherenceSelection typeSelection, Guid pes_id, Guid ent_id, EnumSYS_Visao vis_id, int ttn_id, int year, long parentId);
		List<Adherence> Select(long test_id, Guid usuId, List<long> EntitiesId, EnumAdherenceSelection typeSelection, Guid pes_id, Guid ent_id, EnumSYS_Visao vis_id, int ttn_id, int year);
		Adherence GetByTest(long test_id, EnumAdherenceEntity typeEntity, long EntityId);
		Adherence Insert(Adherence entity);
		IEnumerable<AdherenceDTO> GetSectionsToAnswerSheetLot(long test_id, long TestType_id, bool AllAdhered, int? esc_id, Guid? uad_id);
		IEnumerable<AdheredEntityDTO> GetAdheredDreSimple(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem);
		IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimple(long TestId, Guid uad_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem);
		IEnumerable<AdheredEntityDTO> GetAdheredSectionSimple(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem, int esc_id);
        IEnumerable<AdheredEntityDTO> GetAdheredSectionBySchool(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem, int esc_id);

		IEnumerable<AdheredEntityDTO> LoadDreSimpleAdherence(long TestId, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
		IEnumerable<AdheredEntityDTO> LoadSchoolSimpleAdherence(long test_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo, Guid uad_id);
		List<TeamsDTO> GetInfoTeams(List<long?> turmas);
		IEnumerable<TeamsDTO> GetSectionByTestAndTcpId(List<long> test_id, Guid? uad_id, int? esc_id, long? tcp_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
        IEnumerable<AdheredEntityDTO> GetAdheredDreSimpleReportItem(IEnumerable<long> lstTest, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem);
        IEnumerable<AdheredEntityDTO> GetAdheredSchoolSimpleReportItem(List<long> lstTest, Guid uad_id, CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo group, int tne_id, int crp_ordem);
    }
}
