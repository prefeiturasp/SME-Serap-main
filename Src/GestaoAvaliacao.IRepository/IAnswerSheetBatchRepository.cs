using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.IRepository
{
    public interface IAnswerSheetBatchRepository
	{
		AnswerSheetBatch Get(long id);
		AnswerSheetBatch GetSimple(long id);
		AnswerSheetBatch Find(AnswerSheetBatchFilter filter);
		AnswerSheetBatchResult GetSchoolSectionInformation(int SchoolId, long SectionId);
		AnswerSheetBatch Save(AnswerSheetBatch entity);
		void Update(AnswerSheetBatch entity);
		long GetStudentId(long SectionId, int? mtu_numeroChamada, long? alu_id);
        void UpdateOwnerEntities(AnswerSheetBatch entity);
	}
}
