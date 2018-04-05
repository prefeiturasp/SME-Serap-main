using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IBlockBusiness
    {
        /// <summary>
        /// Busca os itens da prova
        /// </summary>
        /// <param name="testId">Id da prova</param>
        /// <returns>Lista de projection com informações de cada item da prova</returns>
        Task<IEnumerable<ItemWithSkillAndAlternativeProjection>> GetItemsWithSkillAndCorrectAlternativeBytest(long testId);
        IEnumerable<ItemWithOrderAndRevoked> GetTestItemBlocks(Int64 TestId);
        IEnumerable<Block> GetBlocksByItensTests(List<long> tests);
        IEnumerable<Block> GetTestBlocks(Int64 TestId);
        IEnumerable<Item> GetBlockItens(Int64 Id);
        IEnumerable<BlockKnowledgeArea> GetBlockKnowledgeAreas(long Id);
        void RemoveBlockItem(Int64 Id, Int64 ItemId);
        Block Save(Block block, Guid UsuId, EnumSYS_Visao vision);
        Block Update(Block block, Guid UsuId, EnumSYS_Visao vision);
        Block SaveKnowLedgeAreaOrder(Block block);
        IEnumerable<Block> GetBookletItems(Int64 BookletId);
        int CountItemTest(long Id);
        IEnumerable<StudentCorrectionAnswerGrid> GetTestQuestions(long Id);
    }
}