using GestaoAvaliacao.Entities;
using System;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.IBusiness
{
    public interface IBlockChainBusiness
    {
        BlockChain Save(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision);
        BlockChain Update(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision);
    }
}
