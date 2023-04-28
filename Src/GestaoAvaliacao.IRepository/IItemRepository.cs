using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemRepository
	{
		Item GetItemByItemCode(string itemCode);
		Item Save(Item entity);
		Item Update(Item entity);
		Item Get(long id);
		IEnumerable<Item> Load(ref Pager pager);
		void Delete(long id);
		Item _GetMatrixBytem(long itemId);
		Item _GetSimpleMatrixBytem(long itemId);
		Item _GetBaseTextBytem(long itemId);
		Item _GetItemById(long Id);
		Item _GetItemSummaryById(long Id);
        IEnumerable<Item> GetItemSummaryById(List<long> idsTest, List<long> ids);
        Item _GetGradeByItem(long Id);
		Item _GetAddItemInfos(long Id);
		IEnumerable<Item> GetItems(List<long> ItemIds);
		IEnumerable<Item> GetItemsApi(List<long> ItemIds);
		IEnumerable<long> GetIdsItemsApi(ref Pager pager, int areaConhecimentoId, long? matrizId = null);

        IEnumerable<Item> _GetItemsByBaseText(long baseTextId);
		IEnumerable<ItemGroupBaseText> _GetItemGroupBaseTexts(bool? lastVersion = null, params long[] baseTextId);
		IEnumerable<ItemBlockResult> _BlockSearchItem(BlockItemFilter filter, ref Pager pager);
        IEnumerable<Item> _GetItemVersions(int itemCodeVersion);
        IEnumerable<Item> GetItemVersions(int itemCodeVersion, int itemVersion);
        IEnumerable<ItemResult> _SearchItems(ItemFilter filter, ref Pager pager);
		bool ExistsItemBlock(long Id);
        int GetMaxCode();
        bool VerifyItemCodeAlreadyExists(string itemCode, long? itemId = null);
        int GetMaxVersionByItemCode(int itemCodeVersions);
		void UpdateVersion(long Id);
		void UpdateBaseText(long Id, long IdBaseText);
		Item GetToPreview(long id);
		IEnumerable<Item> GetToPreviewByBaseText(long id);

		List<ItemReportItemType> _GetItemType(int Id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItemLevel> _GetItemLevel(int Id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItem> _GetItem(Guid EntityId, long typeLevelEducation);
		List<ItemReportItemCurriculumGrade> _GetItemCurriculumGrade(int id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItemSituation> _GetItemSituation(int id, string inicio, string fim, Guid EntityId, long typeLevelEducation);
        Item RevokeItem(Item Item);
        void SaveChangeItem(Item item, long TestId, long itemIdAntigo, long blockId);
        void SaveChangeBlockChainItem(Item item, long testId, long itemIdAntigo, long blockChainId);
    }
}
