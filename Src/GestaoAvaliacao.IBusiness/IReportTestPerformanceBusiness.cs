using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportTestPerformanceBusiness
    {
        List<TestAverageDreDTO> GetTestAverageByTestGroupByDre(long testId, long? discipline_id);
        List<TestAverageSchoolDTO> GetTestAverageByTestDreGroupBySchool(long testId, long? discipline_id, Guid dre_id);
        List<TestAveragePerformanceDTO> GetTestAveragePerformanceGeneral(long test_Id, long? discipline_id);
        TestAverageViewModel ObterDresDesempenho(long provaId, long? discipline_id, SYS_Usuario usuario, SYS_Grupo grupo);     
        TestAverageViewModel ObterEscolasDesempenho(long provaId, long? discipline_id, Guid DreId, SYS_Usuario usuario, SYS_Grupo grupo);
        EntityFile ExportReport(List<TestAveragePerformanceDTO> lista, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario);
    }
}
