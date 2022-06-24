using System;

namespace GestaoAvaliacao.Util
{
	public class AnswerSheetBatchResult
	{
		public long Id { get; set; }
		public string Description { get; set; }
		public Guid SupAdmUnitId { get; set; }
		public string SupAdmUnitName { get; set; }
		public string SupAdmUnitCode { get; set; }
		public int SchoolId { get; set; }
		public string SchoolName { get; set; }
		public string UadSigla { get; set; }
		public long SectionId { get; set; }
		public string SectionName { get; set; }
		public string SectionCode { get; set; }

		public int SchoolId_BatchFile { get; set; }
		public long SectionId_BatchFile { get; set; }

		public long StudentId { get; set; }
		public string StudentName { get; set; }
		public long FileId { get; set; }
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public string FileError { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public byte Processing { get; set; }
		public byte OwnerEntity { get; set; }
		public byte? Situation { get; set; }
		public bool Sent { get; set; }
		public int Total { get; set; }
		public long? Test_Id { get; set; }
		public int Errors { get; set; }
		public int Warnings { get; set; }
		public int Success { get; set; }
		public int PendingIdentification { get; set; }
		public int NotIdentified { get; set; }
		public string Resolution { get; set; }
		public Validate Validate { get; set; }
	}

	public class AnswerSheetBatchQueueResult
	{
		public long Id { get; set; }
		public long File_Id { get; set; }
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public long? AnswerSheetBatch_Id { get; set; }
		public Guid? SupAdmUnit_Id { get; set; }
		public string SupAdmUnitName { get; set; }
		public int? School_Id { get; set; }
		public string SchoolName { get; set; }
		public int? CountFiles { get; set; }
		public EnumBatchQueueSituation Situation { get; set; }
		public string Description { get; set; }
		public Guid? CreatedBy_Id { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public string UserName { get; set; }

		public int? Pending { get; set; }
		public int? Success { get; set; }
		public int? Error { get; set; }
		public int? Warning { get; set; }
		public int? PendingIdentification { get; set; }
		public int? NotIdentified { get; set; }
		public int? Absent { get; set; }
	}

	public class AnswerSheetBatchFileResult
	{
		public long Id { get; set; }
		public long? AnswerSheetBatch_Id { get; set; }
		public long FileId { get; set; }
		public string FileName { get; set; }
		public string FileOriginalName { get; set; }
		public string FileContentType { get; set; }
		public string FilePath { get; set; }
	}

	public class AnswerSheetBatchFileCountDTO
	{
		public int Situation { get; set; }
		public int SituationCount { get; set; }
		public int TotalResult { get; set; }
	}

	public class AnswerSheetBatchFileCountResult
	{
		public int Total { get; set; }
		public int Errors { get; set; }
		public int Warnings { get; set; }
		public int Success { get; set; }
		public int PendingIdentification { get; set; }
		public int NotIdentified { get; set; }
		public int Pending { get; set; }
		public int Absents { get; set; }
	}

	public class AnswerSheetFollowUpIdentificationResult
	{
		public object Id { get; set; }
		public string Name { get; set; }
		public int TotalSent { get; set; }
		public int TotalPendingIdentification { get; set; }
		public int TotalIdentified { get; set; }
		public int TotalNotIdentified { get; set; }
		public int TotalResolutionNotOk { get; set; }
		public int TotalResult { get; set; }
		public EnumBatchSituation? Situation { get; set; }
		public DateTime? CreateDate { get; set; }
		public string FilePath { get; set; }
		public long? TestId { get; set; }
		public string Resolution { get; set; }
	}

	public class AnswerSheetFollowUpIdentification
	{
		public Guid? SupAdmUnit_Id { get; set; }
		public string SupAdmUnitName { get; set; }
		public string SupAdmUnitInitials { get; set; }
		public int? SchoolId { get; set; }
		public string SchoolName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	public class AnswerSheetStudentInformation
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string SchoolName { get; set; }
		public long SectionId { get; set; }
		public string SectionName { get; set; }
		public string TeacherName { get; set; }
		public int NumberId { get; set; }
		public Guid FileName { get { return Guid.NewGuid(); } }
		public string RelativePath { get; set; }
		public string StoragePath { get; set; }
	}

	public class AnswerSheetBatchItems
	{
		public long Item_Id { get; set; }
		public long Id { get; set; }
		public bool Correct { get; set; }
		public string Numeration { get; set; }
		public int Order { get; set; }
		public string ItemCode { get; set; }
		public int? KnowledgeArea_Id { get; set; }
		public string KnowledgeArea_Description { get; set; }
		public bool Ignore { get; set; }
	}

	public class GenerateTestDTO
	{
		public Validate Validate { get; set; }
		public long TestId { get; set; }
		public TestFileDTO File { get; set; }
		public TestFileDTO FileAnswerSheet { get; set; }
		public TestFileDTO FileFeedback { get; set; }
		public string Html { get; set; }

	}

	public class TestFileDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public string GenerationData { get; set; }
	}

	public class TestShowVideoAudioFilesDto
    {
        public long TestId { get; set; }
        public bool ShowVideoFiles { get; set; }
        public bool ShowAudioFiles { get; set; }
    }

	public class DisciplineItem
	{
		public long Item_Id { get; set; }
		public long Discipline_Id { get; set; }
	}

	#region API
	public class AdherenceResult
	{
		public long id { get; set; }
		public bool success { get; set; }
		public string type { get; set; }
		public string message { get; set; }
	}
	public class CorrectionResult
	{
		public bool success { get; set; }
		public string type { get; set; }
		public string message { get; set; }
		public long alu_id { get; set; }
		public long item_id { get; set; }
	}	
	#endregion
}
