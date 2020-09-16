using GestaoAvaliacao.Util;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.TestsSectionStatusCorrectionBusiness
{
    internal interface IAjusteTestSectionStatusCorrectionRepository
    {
        Task UpdateStatusAsync(long testId, long turId, EnumStatusCorrection status);
    }
}