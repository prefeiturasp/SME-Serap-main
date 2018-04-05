using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetBatchLogBusiness
	{
        AnswerSheetBatchLog Get(long id);
        AnswerSheetBatchLog GetByBatchFile_Id(long id);
        AnswerSheetBatchLog Save(AnswerSheetBatchLog entity);
        AnswerSheetBatchLog Update(long Id, AnswerSheetBatchLog entity);
	}
}
