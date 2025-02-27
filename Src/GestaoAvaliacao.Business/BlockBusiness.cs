using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class BlockBusiness : IBlockBusiness
    {
        private readonly IBlockRepository blockRepository;
        private readonly ITestRepository testRepository;

        public BlockBusiness(IBlockRepository blockRepository, ITestRepository testRepository)
        {
            this.blockRepository = blockRepository;
            this.testRepository = testRepository;
        }

        #region Custom

        private Validate Validate(Block entity, Validate valid, Guid UsuId, EnumSYS_Visao vision)
        {
            valid.Message = null;
            if (entity.BlockItems != null)
            {
                var duplicateOrder = entity.BlockItems.GroupBy(x => x.Order).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicateOrder != null && duplicateOrder.Any())
                    valid.Message = "Existe mais de um item com a mesma ordem.";
            }

            if (!((entity.Test.UsuId == UsuId) || (vision == EnumSYS_Visao.Administracao && entity.Test.TestType.Global)))
                valid.Message = "Apenas o proprietário da prova pode alterá-la";


            if (!string.IsNullOrEmpty(valid.Message))
            {
                string br = "<br/>";
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

        #region Read

        /// <summary>
        /// Busca os itens da prova
        /// </summary>
        /// <param name="testId">Id da prova</param>
        /// <returns>Lista de projection com informações de cada item da prova</returns>
        public async Task<IEnumerable<ItemWithSkillAndAlternativeProjection>> GetItemsWithSkillAndCorrectAlternativeBytest(long testId)
        {
            return await blockRepository.GetItemsWithSkillAndCorrectAlternativeBytest(testId);
        }

        public IEnumerable<ItemWithOrderAndRevoked> GetTestItemBlocks(Int64 TestId)
        {
            return blockRepository.GetTestItemBlocks(TestId);
        }

        public IEnumerable<Block> GetBlocksByItensTests(List<long> tests)
        {
            return blockRepository.GetBlocksByItensTests(tests);
        }

        public IEnumerable<Block> GetTestBlocks(Int64 TestId)
        {
            return blockRepository.GetTestBlocks(TestId);
        }

        public IEnumerable<Item> GetBlockItens(Int64 Id, int page, int pageItens)
        {
            return blockRepository.GetBlockItens(Id, page, pageItens);
        }

        public IEnumerable<Item> GetBlockItensWithBlockChain(long Id, int page, int pageItens)
        {
            return blockRepository.GetBlockItensWithBlockChain(Id, page, pageItens);
        }

        public IEnumerable<BlockKnowledgeArea> GetBlockKnowledgeAreas(long Id)
        {
            return blockRepository.GetBlockKnowledgeAreas(Id);
        }

        public IEnumerable<Block> GetBookletItems(Int64 BookletId)
        {
            return blockRepository.GetBookletItems(BookletId);
        }

        public int CountItemTest(long Id)
        {
            return blockRepository.CountItemTest(Id);
        }

        public int CountItemTestBIB(long Id)
        {
            return blockRepository.CountItemTestBIB(Id);
        }

        #endregion

        #region Write

        public Block Save(Block entity, Guid UsuId, EnumSYS_Visao vision)
        {
            if (entity.Test_Id != null)
            {
                Test test = testRepository.GetObjectWithTestType(Convert.ToInt64(entity.Test_Id));
                entity.Test = test;
                entity.Test_Id = test.Id;
            }

            entity.Validate = Validate(entity, entity.Validate, UsuId, vision);
            if (entity.Validate.IsValid)
            {
                entity = blockRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
            }

            return entity;
        }

        public Block Update(Block entity, Guid UsuId, EnumSYS_Visao vision)
        {
            if (entity.Test_Id != null)
            {
                Test test = testRepository.GetObjectWithTestType(Convert.ToInt64(entity.Test_Id));
                entity.Test = test;
                entity.Test_Id = test.Id;
            }

            entity.Validate = Validate(entity, entity.Validate, UsuId, vision);
            if (entity.Validate.IsValid)
            {
                blockRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
            }

            return entity;
        }

        public Block SaveKnowLedgeAreaOrder(Block block)
        {
            return blockRepository.SaveKnowLedgeAreaOrder(block);
        }

        public void RemoveBlockItem(Int64 Id, Int64 ItemId)
        {
            blockRepository.RemoveBlockItem(Id, ItemId);
        }

        public IEnumerable<StudentCorrectionAnswerGrid> GetTestQuestions(long Id)
        {
            return blockRepository.GetTestQuestions(Id);
        }

        public Block Delete(long Id)
        {
            Block entity = new Block { Id = Id };

            blockRepository.Delete(Id);

            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Caderno excluído com sucesso.";

            return entity;
        }

        public Block DeleteBlockItems(long Id)
        {
            Block entity = new Block { Id = Id };

            blockRepository.DeleteItems(Id);

            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Items removidos com sucesso.";

            return entity;
        }

       

        #endregion
    }
}