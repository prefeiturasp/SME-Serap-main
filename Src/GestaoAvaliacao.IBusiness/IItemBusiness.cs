using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IItemBusiness
	{
		Item GetItemByItemCode(string itemCode);
		bool VerifyItemCodeAlreadyExists(string itemCode, long? itemId = null);
        Item Save(long Id, Item entity, List<File> files = null);
		Item Update(long Id, Item entity, List<File> files = null);
		Item Delete(long Id);
		Item _GetMatrixBytem(long itemId);
		Item _GetSimpleMatrixBytem(long itemId);
		Item _GetBaseTextBytem(long itemId);
		Item _GetItemById(long Id);
		Item _GetGradeByItem(long Id);
		Item _GetItemSummaryById(long Id);
        List<Item> GetItemSummaryById(List<long> idsTest, List<long> ids);
        Item _GetAddItemInfos(long Id);
		IEnumerable<Item> GetItems(List<long> ItemIds);
        IEnumerable<Item> _GetItemVersions(int itemCodeVersion);
        IEnumerable<Item> GetItemVersions(int itemCodeVersion, int itemVersion);
        IEnumerable<Item> _GetItemsByBaseText(long baseTextId);
		IEnumerable<ItemGroupBaseText> _GetItemGroupBaseTexts(bool? lastVersion = null, params long[] baseTextId);
		IEnumerable<ItemBlockResult> _BlockSearchItem(BlockItemFilter filter, ref Pager pager);
		IEnumerable<ItemResult> _SearchItems(ItemFilter filter, ref Pager pager);
		bool ExistsItemBlock(long Id);
		File Upload(Uploader file, string VirtualDirectory, string PhysicalDirectory);
		List<ItemReportItemType> _GetItemType(int Id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItemLevel> _GetItemLevel(int Id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItem> _GetItem(Guid EntityId, long typeLevelEducation);
		List<ItemReportItemCurriculumGrade> _GetItemCurriculumGrade(int id, int situacao, Guid EntityId, long typeLevelEducation);
		List<ItemReportItemSituation> _GetItemSituation(int id, string inicio, string fim, Guid EntityId, long typeLevelEducation);

        byte[] GetItemPreview(long id, string url, List<Parameter> parameters);
        byte[] GetItemPreviewByBaseText(long id, string url, List<Parameter> parameters);
        Item RevokeItem(long Item_Id, bool Revoked);
        Item SaveChangeItem(Item item, long TestId, long itemIdAntigo, long blockId);

    }
}
