using System;
using System.Collections.Generic;
using System.Linq;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using Item = GestaoAvaliacao.Entities.Item;

namespace GestaoAvaliacao.Business
{
    public class BlockChainBusiness : IBlockChainBusiness
    {
        private readonly IBlockChainRepository blockChainRepository;
        private readonly ITestRepository testRepository;
        private readonly IBlockChainBlockRepository blockChainBlockRepository;
        private readonly IBlockRepository blockRepository;

        public BlockChainBusiness(IBlockChainRepository blockChainRepository, ITestRepository testRepository,
            IBlockChainBlockRepository blockChainBlockRepository, IBlockRepository blockRepository)
        {
            this.blockChainRepository = blockChainRepository;
            this.testRepository = testRepository;
            this.blockRepository = blockRepository;
            this.blockChainBlockRepository = blockChainBlockRepository;
        }

        #region Custom

        private static Validate Validate(BlockChain entity, Validate valid, Guid usuId, EnumSYS_Visao vision)
        {
            valid.Message = null;

            var duplicateOrder = entity.BlockChainItems?.GroupBy(x => x.Order)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key).ToList();

            if (duplicateOrder != null && duplicateOrder.Any())
                valid.Message = "Existe mais de um item com a mesma ordem.";

            if (!(entity.Test.UsuId == usuId || (vision == EnumSYS_Visao.Administracao && entity.Test.TestType.Global)))
                valid.Message = "Apenas o proprietário da prova pode alterá-la";

            if (!string.IsNullOrEmpty(valid.Message))
            {
                const string br = "<br/>";

                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        #endregion

        public BlockChain Save(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision)
        {
            if (blockChain.Test_Id != null)
            {
                var test = testRepository.GetObjectWithTestType(Convert.ToInt64(blockChain.Test_Id));

                blockChain.Test = test;
                blockChain.Test_Id = test.Id;
            }

            blockChain.Validate = Validate(blockChain, blockChain.Validate, usuId, vision);

            if (!blockChain.Validate.IsValid) 
                return blockChain;

            blockChain = blockChainRepository.Save(blockChain);
            blockChain.Validate.Type = ValidateType.Save.ToString();

            return blockChain;
        }

        public BlockChain Update(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision)
        {
            if (blockChain.Test_Id != null)
            {
                var test = testRepository.GetObjectWithTestType(Convert.ToInt64(blockChain.Test_Id));
                blockChain.Test = test;
                blockChain.Test_Id = test.Id;
            }

            blockChain.Validate = Validate(blockChain, blockChain.Validate, usuId, vision);

            if (!blockChain.Validate.IsValid) 
                return blockChain;

            blockChainRepository.Update(blockChain);
            blockChain.Validate.Type = ValidateType.Update.ToString();

            return blockChain;
        }

        public void RemoveBlockChainItem(long blockChainId, long itemId)
        {
            blockChainRepository.RemoveBlockChainItem(blockChainId, itemId);
        }

        public BlockChain DeleteBlockChainItems(long id)
        {
            var entity = new BlockChain { Id = id };

            blockChainRepository.DeleteBlockChainItems(id);

            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Itens removidos com sucesso.";

            return entity;
        }

        public IEnumerable<BlockChain> GetTestBlockChains(long testId)
        {
            return blockChainRepository.GetTestBlockChains(testId);
        }

        public IEnumerable<Block> ObterCadernosPorProva(long testId)
        {
            return blockChainRepository.ObterCadernosPorProva(testId);
        }

        public IEnumerable<Item> GetBlockChainItems(long blockChainId, int page, int pageItems)
        {
            return blockChainRepository.GetBlockChainItems(blockChainId, page, pageItems);
        }

        public void DeleteByTestId(long testId)
        {
            blockChainRepository.DeleteByTestId(testId);
        }

        public void UpdateBlockByTestId(long testId)
        {
            var blockChainsBlockDb = blockChainBlockRepository.GetTestBlockChainsBlock(testId);
            var blockChainsDb = blockChainRepository.GetTestBlockChains(testId).ToList();
            var blocksDb = blockRepository.GetTestBlocks(testId).ToList();

            foreach (var blockChainBlock in blockChainsBlockDb)
            {
                var blockChainDb = blockChainsDb.FirstOrDefault(c => c.Id == blockChainBlock.BlockChain_Id);

                if (blockChainDb == null)
                    continue;

                var blockDb = blocksDb.FirstOrDefault(c => c.Id == blockChainBlock.Block_Id);

                if (blockDb == null)
                    continue;

                blockRepository.Update(blockDb);
            }
        }
    }
}
