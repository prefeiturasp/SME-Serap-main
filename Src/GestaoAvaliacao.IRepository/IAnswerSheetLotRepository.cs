using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IAnswerSheetLotRepository
	{
        IEnumerable<AnswerSheetLotDTO> GetTestLot(ref Pager pager, AnswerSheetLotFilter filter);
        IEnumerable<AnswerSheetLot> GetLotList(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLotDTO> GetLotFiles(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLotDTO> GetAdheredTests(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLot> GetByParentId(long ParentId);
        AnswerSheetLot GetById(long Id);
        AnswerSheetLot GetByTest(long TestId);
        IEnumerable<AnswerSheetLot> GetByExecutionState(EnumServiceState state);
        IEnumerable<AnswerSheetLot> GetParentByExecutionState(EnumServiceState state);
        IEnumerable<Test> GetTestList(long Id);
        int GetTestCount(long Id);     
        AnswerSheetLot Save(AnswerSheetLot entity);
        AnswerSheetLot SaveLot(AnswerSheetLot entity, List<AnswerSheetLot> list);
        AnswerSheetLot Update(AnswerSheetLot entity);
        void UpdateLot(AnswerSheetLot entity);
        AnswerSheetLot Delete(AnswerSheetLot entity);
        void DeleteLot(long parent_Id);
        IEnumerable<long> GetFiles(long parent_Id);
    }
}
