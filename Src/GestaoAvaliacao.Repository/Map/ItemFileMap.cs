using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemFileMap : EntityBaseMap<ItemFile>
    {
        public ItemFileMap()
        {
            ToTable("ItemFile");

            HasOptional(p => p.Thumbnail)
                .WithMany()
                .HasForeignKey(p => p.Thumbnail_Id);

            HasOptional(p => p.ConvertedFile)
                .WithMany()
                .HasForeignKey(p => p.ConvertedFile_Id);
        }
    }
}