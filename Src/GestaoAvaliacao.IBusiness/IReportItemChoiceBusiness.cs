using GestaoAvaliacao.Entities.DTO;
using System;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportItemChoiceBusiness
    {
        /// <summary>
        /// Busca as médias de escolha por alternativa de cada item em uma prova
        /// </summary>
        /// <param name="test_Id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Dto com o percentual de escolha de cada alternativa em cada item da prova</returns>
        Task<ItemPercentageChoiceByAlternativeWithOrderResult.Test> GetItemPercentageChoiceByAlternative(long test_Id, long? discipline_id, Guid? dre_id, int? esc_id);

        /// <summary>
        /// Exporta o relatório para CSV
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="separator">Separador do CSV</param>
        /// <param name="virtualDirectory">Diretório virtual</param>
        /// <param name="physicalDirectory">Diretório físico</param>
        /// <returns>Dados para download do CSV</returns>
        Task<EntityFile> ExportReport(int test_id, long? discipline_id, Guid? dre_id, int? esc_id, string separator, string virtualDirectory, string physicalDirectory);
    }
}
