using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IAnswerSheetBatchFilesRepository
	{
        IEnumerable<AnswerSheetBatchResult> SearchBatchFiles(ref Pager pager, AnswerSheetBatchFilter filter, string nomeAluno, string turma);
        IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(long batchId, bool sent, int rows);
        IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(EnumBatchSituation situation, int rows);
        IEnumerable<AnswerSheetBatchFiles> GetFiles(long batchId, bool excludeErrorFiles);
        AnswerSheetBatchFileResult GetStudentFile(long testId, long studentId, long sectionId);
        void SaveList(List<AnswerSheetBatchFiles> list);
        void Update(AnswerSheetBatchFiles entity);
        void UpdateList(List<AnswerSheetBatchFiles> list);
        void UpdateSentList(List<AnswerSheetBatchFiles> list);
        void DeleteList(List<AnswerSheetBatchFiles> list);
		AnswerSheetBatchFiles Get(long Id);
		int GetFilesCount(long batchId);
        int GetFilesNotSentCount(long batchId);
        AnswerSheetBatchFiles GetFile(long Id, long fileId);
        AnswerSheetBatchFileCountResult GetCountBatchInformation(AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(ref Pager pager, AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(ref Pager pager, AnswerSheetBatchFilter filter);
        AnswerSheetFollowUpIdentification GetIdentificationReportInfo(AnswerSheetBatchFilter filter);
    }
}
