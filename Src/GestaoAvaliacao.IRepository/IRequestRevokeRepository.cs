using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IRequestRevokeRepository
	{
		IEnumerable<long> GetRevokedItemsByTest(long test_id);
        IEnumerable<RequestRevokeDTO> GetRequestRevoke(int blockItem_Id);
        RequestRevoke UpdateBlockItemsRevoked(RequestRevoke requestRevoke);
        List<RequestRevoke> UpdateRequestRevokedByTestBlockItem(long Test_Id, long BlockItem_Id, EnumSituation Situation);
	}
}
