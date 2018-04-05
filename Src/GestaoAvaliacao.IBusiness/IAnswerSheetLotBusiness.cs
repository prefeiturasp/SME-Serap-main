using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetLotBusiness
	{
		IEnumerable<AnswerSheetLotDTO> GetTestLot(ref Pager pager, AnswerSheetLotFilter filter);
        IEnumerable<AnswerSheetLot> GetLotList(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLotDTO> GetLotFiles(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLotDTO> GetAdheredTests(AnswerSheetLotFilter filter, ref Pager pager);
        IEnumerable<AnswerSheetLot> GetByParentId(long ParentId);
        AnswerSheetLot GetById(long Id);
        IEnumerable<AnswerSheetLot> GetByExecutionState(EnumServiceState state);
        IEnumerable<AnswerSheetLot> GetParentByExecutionState(EnumServiceState state);
        IEnumerable<Test> GetTestList(long Id);
        int GetTestCount(long Id);
        AnswerSheetLotHistory GetLotFolderSize(long Id, SYS_Usuario usuarioLogado);
        AnswerSheetLot Save(AnswerSheetLot entity, List<AnswerSheetLot> list);
        AnswerSheetLot Update(AnswerSheetLot entity, bool service, bool updateSubLot = true);
        AnswerSheetLot GenerateAgain(AnswerSheetLot entity, Guid userId);
        AnswerSheetLot Delete(AnswerSheetLot entity);

    }
}
