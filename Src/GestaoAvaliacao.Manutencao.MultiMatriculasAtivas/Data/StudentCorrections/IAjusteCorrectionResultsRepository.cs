using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.StudentCorrections
{
    internal interface IAjusteCorrectionResultsRepository
    {
        Task DeleteCorrectionResults(long testId, long turmaId, Guid entidadeId);
    }
}