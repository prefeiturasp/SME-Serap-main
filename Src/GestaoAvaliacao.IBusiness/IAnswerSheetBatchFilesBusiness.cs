using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetBatchFilesBusiness
	{
        IEnumerable<AnswerSheetBatchResult> SearchBatchFiles(ref Pager pager, AnswerSheetBatchFilter filter, string nomeAluno, string turma);
        IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(long batchId, bool sent, int rows);
        IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(EnumBatchSituation situation, int rows);
        IEnumerable<AnswerSheetBatchFiles> GetFiles(long batchId, bool excludeErrorFiles);
        void UpdateBatchFilesIdentified(IEnumerable<AnswerSheetBatch> answerSheetBatchList);
        int GetFilesCount(long batchId);
        int GetFilesNotSentCount(long batchId);
        AnswerSheetBatchFiles GetFile(long Id, long fileId);
        EntityFile GetStudentFile(long testId, long studentId, long sectionId, string physicalDirectory, string virtualDirectory, SYS_Usuario usuarioLogado);
        void SaveList(List<AnswerSheetBatchFiles> list);
        AnswerSheetBatchFiles Update(AnswerSheetBatchFiles entity);
        void UpdateList(List<AnswerSheetBatchFiles> list);
		void UpdateSentList(List<AnswerSheetBatchFiles> list);
		void DeleteList(List<AnswerSheetBatchFiles> list);
		AnswerSheetBatch SendToProcessing(AnswerSheetBatchFilter filter);
		AnswerSheetBatchFiles Get(long Id);
        AnswerSheetBatch SaveBatch(AnswerSheetBatch entity, string virtualDirectory, string physicalDirectory, SYS_Usuario usuarioLogado);
        AnswerSheetBatchQueue Unzip(EntityFile file, AnswerSheetBatchQueue entity, Guid EntityId, Guid CreatedBy_Id);
        Validate SavePendingIdentification(List<AnswerSheetBatchFiles> answerSheetBatchFiles, long AnswerSheetBatchQueue_Id, long? AnswerSheetBatch_Id, int? School_Id, Guid? SupAdmUnit_Id);
        void AssociateFilesToEntity(List<AnswerSheetBatchFiles> answerSheetBatchFiles, long answerSheetBatch_Id);
        EntityFile ExportAnswerSheetData(AnswerSheetBatchFilter filter, string separator, string virtualDirectory, string physicalDirectory);
        AnswerSheetBatchFileCountResult GetCountBatchInformation(AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(ref Pager pager, AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(AnswerSheetBatchFilter filter);
        IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(ref Pager pager, AnswerSheetBatchFilter filter);
        EntityFile ExportFollowUpIdentification(AnswerSheetBatchFilter filter, string separator, string virtualDirectory, string physicalDirectory);
        EntityFile ZipFollowUpIdentification(AnswerSheetBatchFilter filter, string virtualDirectory, string physicalDirectory);
        AnswerSheetFollowUpIdentification GetIdentificationReportInfo(AnswerSheetBatchFilter filter);
    }
}
