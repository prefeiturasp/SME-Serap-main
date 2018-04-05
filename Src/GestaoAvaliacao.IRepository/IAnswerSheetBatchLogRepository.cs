using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IRepository
{
    public interface IAnswerSheetBatchLogRepository
	{
        AnswerSheetBatchLog Get(long id);
        AnswerSheetBatchLog GetByBatchFile_Id(long id);
        AnswerSheetBatchLog Save(AnswerSheetBatchLog entity);
        void Update(AnswerSheetBatchLog entity);
    }
}
