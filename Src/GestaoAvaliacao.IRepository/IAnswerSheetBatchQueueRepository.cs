using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IAnswerSheetBatchQueueRepository
    {
        AnswerSheetBatchQueue Save(AnswerSheetBatchQueue entity);
        AnswerSheetBatchQueue Update(long Id, AnswerSheetBatchQueue entity);
        IEnumerable<AnswerSheetBatchQueueResult> Search(AnswerSheetBatchQueueFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetBatchQueueResult> GetTop(AnswerSheetBatchQueueFilter filter);
        IEnumerable<AnswerSheetBatchQueue> GetAnswerSheetBatchBySituation(EnumBatchQueueSituation Situation, int rows);
        void UpdateZipProcessing(AnswerSheetBatchQueue entity);
        void Delete(AnswerSheetBatchQueue entity);
        IEnumerable<AnswerSheetBatchFiles> SelectFilesError(AnswerSheetBatchQueue entity);
    }
}
