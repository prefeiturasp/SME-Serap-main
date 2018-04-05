using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetBatchBusiness
	{
		AnswerSheetBatch Get(long id);
		AnswerSheetBatch GetSimple(long id);
		AnswerSheetBatch Find(AnswerSheetBatchFilter filter);
		AnswerSheetBatchResult GetSchoolSectionInformation(int? SchoolId, long? SectionId);
		AnswerSheetBatch Save(AnswerSheetBatch entity);
		AnswerSheetBatch Update(long Id, AnswerSheetBatch entity);
		long GetStudentId(long SectionId, int? mtu_numeroChamada = null, long? alu_id = null);
        AnswerSheetBatchResult GetSectionInformation(long SectionId);
		AnswerSheetBatch UpdateOwnerEntities(long Id, AnswerSheetBatch entity);
	}
}
