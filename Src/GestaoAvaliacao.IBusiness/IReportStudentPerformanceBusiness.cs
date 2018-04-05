using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportStudentPerformanceBusiness
    {
        /// <summary>
        /// Busca informações de identificação do alu e relacionadas ao desempenho do aluno na prova
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="alu_id">Id do aluno</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <returns>DTO com todas as informações do aluno(identificação e desempenho na prova)</returns>
        Task<StudentInformationAndPerformanceTestResult.Student> GetStudentInformation(long test_id, long alu_id, Guid dre_id);

        /// <summary>
        /// Busca as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova 
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="team_id">Id da turma</param>
        /// <returns>DTO com as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova</returns>
        Task<UnitsInformationAndPerformanceTestDTO> GetUnitsInformation(long test_id, int esc_id, Guid dre_id, long team_id);

        /// <summary>
        /// Exporta o relatório para CSV
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="alu_id">Id do aluno</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="team_id">Id da turma</param>
        /// <param name="separator">Separador do CSV</param>
        /// <param name="virtualDirectory">Diretório virtual</param>
        /// <param name="physicalDirectory">Diretório físico</param>
        /// <param name="typeUser">Visão do usuário</param>
        /// <returns>Dados para download do CSV</returns>
        Task<EntityFile> ExportReport(int test_id, 
                                      long alu_id, 
                                      Guid dre_id, 
                                      int esc_id, 
                                      long team_id, 
                                      string separator, 
                                      string virtualDirectory, 
                                      string physicalDirectory,
                                      EnumSYS_Visao typeUser);
    }
}
