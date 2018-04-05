using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class RequestRevokeBusiness : IRequestRevokeBusiness
	{
		#region Dependences
		readonly IRequestRevokeRepository requestRevokeRepository; 
		#endregion

		#region Constructor
		public RequestRevokeBusiness(IRequestRevokeRepository requestRevokeRepository)
		{
			this.requestRevokeRepository = requestRevokeRepository;
		}
		#endregion

		#region Persist
        public RequestRevoke UpdateItemsRevoked(long BlockItem_Id, long Test_Id, string RevokeReason, long? RequestRevoke_Id, EnumSituation ItemSituation, Guid UsuId)
        {
            RequestRevoke requestRevoke = new RequestRevoke
            {
                Justification = RevokeReason,
                Situation = ItemSituation,
                BlockItem_Id = BlockItem_Id,
                Test_Id = Test_Id,
                UsuId = UsuId,
                Id = (long)RequestRevoke_Id
            };

            requestRevoke = requestRevokeRepository.UpdateBlockItemsRevoked(requestRevoke);

            return requestRevoke;
        }

        public List<RequestRevoke> UpdateRequestRevokedByTestBlockItem(long Test_Id, long BlockItem_Id, EnumSituation Situation)
        {
            return requestRevokeRepository.UpdateRequestRevokedByTestBlockItem(Test_Id, BlockItem_Id, Situation);
        }
		#endregion

		#region Read
		public IEnumerable<long> GetRevokedItemsByTest(long test_id)
		{
			return this.requestRevokeRepository.GetRevokedItemsByTest(test_id);
		}

        public IEnumerable<RequestRevokeDTO> GetRequestRevoke(int blockItem_Id)
        {
            return requestRevokeRepository.GetRequestRevoke(blockItem_Id);
        }
		#endregion
	}
}
