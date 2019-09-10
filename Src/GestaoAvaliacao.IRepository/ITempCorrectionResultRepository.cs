using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITempCorrectionResultRepository
    {
        Task<List<TempCorrectionResult>> _GetNotProcessed();
        Task<TempCorrectionResult> GetEntity(TempCorrectionResult entity);
        Task<TempCorrectionResult> Insert(TempCorrectionResult entity);
        Task<long> Count(TempCorrectionResult entity);
        void InsertMany(List<TempCorrectionResult> entity);
        Task<long> _GetCount();
        Task<TempCorrectionResult> Replace(TempCorrectionResult entity);
        Task<TempCorrectionResult> GetByTestAndTeam(long test_id, long tur_id);
    }
}
