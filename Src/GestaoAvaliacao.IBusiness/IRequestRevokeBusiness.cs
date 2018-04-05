using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IRequestRevokeBusiness
	{
		IEnumerable<long> GetRevokedItemsByTest(long test_id);
        IEnumerable<RequestRevokeDTO> GetRequestRevoke(int blockItem_Id);
        RequestRevoke UpdateItemsRevoked(long BlockItem_Id, long Test_Id, string RevokeReason, long? RequestRevoke_Id, EnumSituation ItemSituation, Guid UsuId);
        List<RequestRevoke> UpdateRequestRevokedByTestBlockItem(long Test_Id, long BlockItem_Id, EnumSituation Situation);
	}
}
