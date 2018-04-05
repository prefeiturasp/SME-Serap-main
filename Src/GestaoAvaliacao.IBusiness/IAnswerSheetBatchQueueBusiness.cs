using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetBatchQueueBusiness
    {
        AnswerSheetBatchQueue Save(AnswerSheetBatchQueue entity);
        AnswerSheetBatchQueue Update(long Id, AnswerSheetBatchQueue entity);
        IEnumerable<AnswerSheetBatchQueueResult> Search(AnswerSheetBatchQueueFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetBatchQueueResult> GetTop(AnswerSheetBatchQueueFilter filter);
        IEnumerable<AnswerSheetBatchQueue> GetAnswerSheetBatchBySituation(EnumBatchQueueSituation Situation, int rows);
        void UpdateZipProcessing(AnswerSheetBatchQueue entity);
        AnswerSheetBatchQueue Delete(long id, Guid ent_id);
        AnswerSheetBatchQueue DeleteError(long id, Guid ent_id);
    }
}
