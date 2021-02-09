using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportItemPerformanceBusiness
    {
        List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceGeneral(long test_Id);
        List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceByDre(List<long> test_Id);
        List<TestAverageItemPerformanceDTO> GetTestAverageItemPerformanceBySchool(long test_Id, Guid dre_id);
        TestAverageItensViewModel ObterDresDesempenhoItem(long provaId, long? discipline_id, SYS_Usuario usuario, SYS_Grupo grupo);
        TestAverageItensViewModel ObterEscolasDesempenhoItem(long provaId, long? discipline_id, Guid DreId, SYS_Usuario usuario, SYS_Grupo grupo);
        EntityFile ExportReport(List<TestAverageItens> lista, List<TestAverageItemPerformanceDTO> mediasSME, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario, long? discipline_id);
        EntityFile ExportReportDre(List<DrePerformanceViewModel> lista, List<DisciplinePerformanceViewModel> mediasSME, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario, long? discipline_id);
        PerformanceItemViewModel GetPerformanceTree(long test_id, long subGroup_id, long tcp_id, SYS_Usuario usuario, SYS_Grupo grupo, Guid? dre_id, int? esc_id, Guid? uad_id, bool? export = false, bool? showBaseText = true);
    }
}
