namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainBlockRepository
    {
        void DeleteByBlockId(long blockId);
        void DeleteByBlockChainId(long blockChainId);
    }
}
